﻿using CardanoSharp.Koios.Client;
using CardanoSharp.Koios.Client.Contracts;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Utilities;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using CardanoSharp.Wallet.Models.Transactions;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Transaction;

public class BuildSimplePaymentTransactionCommand : ICommand
{
    public string? Network { get; init; } // testnet | mainnet
    public string? From { get; init; } // Address Bech32
    public string? SigningKey { get; init; } // Payment Signing Key Bech32 for From Address
    public string? To{ get; init; } // Address Bech32
    public ulong Lovelaces { get; init; } // Output values comes from one of either (Lovelaces | Ada | SendAll=true)
    public decimal Ada { get; init; } 
    public bool SendAll { get; init; } = false; 
    public uint Ttl { get; set; } // Slot for transaction expiry
    public int? MockWitnessCount { get; set; } // Slot for transaction expiry
    public string? Message { get; init; } // Onchain Metadata 674 standard
    public bool Submit { get; init; } // Submits Transaction to Koios node
    public string? OutFile { get; init; } // cardano-cli compatible transaction file (signed only)

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, network, lovelaces, errors) = Validate();
        if (!isValid || From is null || To is null) 
            return CommandResult.FailureInvalidOptions(string.Join(Environment.NewLine, errors));

        (var epochClient, var networkClient, var addressClient) = GetKoiosClients(network);
        var tip = (await networkClient.GetChainTip()).Content?.FirstOrDefault();
        if (tip is null) return CommandResult.FailureBackend("Unable to get chain tip");
        var protocolParams = (await epochClient.GetProtocolParameters(tip.Epoch.ToString())).Content?.FirstOrDefault();
        if (protocolParams is null) 
            return CommandResult.FailureBackend("Unable to get protocol parameters");
        var sourceAddressInfo = (await addressClient.GetAddressInformation(
            new AddressBulkRequest(){ Addresses = new List<string>(){ From } })).Content?.FirstOrDefault();
        if (sourceAddressInfo is null) 
            return CommandResult.FailureBackend($"Unable to get address info for --from address ${From}");
        if (sourceAddressInfo.UtxoSets is null || !sourceAddressInfo.UtxoSets.Any())
            return CommandResult.FailureInvalidOptions("--from address has no utxos");
        var sourceAddressUtxos = MapSourceAddressUtxos(sourceAddressInfo.UtxoSets);
        var consolidatedInputValue = MapConsolidatedTxInputValue(sourceAddressUtxos);
        var txOutput = SendAll 
            ? new PendingTransactionOutput(To, consolidatedInputValue)
            : new PendingTransactionOutput(To, new Balance(lovelaces, Array.Empty<NativeAssetValue>()));
        if (consolidatedInputValue.Lovelaces < txOutput.Value.Lovelaces) 
            return CommandResult.FailureInvalidOptions($"--from address has insufficient balance ({consolidatedInputValue.Lovelaces}) to pay {txOutput.Value.Lovelaces}");
        var changeValue = consolidatedInputValue.Subtract(txOutput.Value);
        var ttl = Ttl > 0 ? Ttl : (uint)tip.AbsSlot + TtlTipOffsetSlots;

        // Build Tx Body
        var txBodyBuilder = TransactionBodyBuilder.Create.SetTtl(ttl).SetFee(protocolParams.MinFeeB.Value);
        // Inputs
        foreach (var txInput in sourceAddressUtxos)
        {
            txBodyBuilder.AddInput(txInput.TxHash, txInput.OutputIndex);
        }
        // Outputs
        txBodyBuilder.AddOutput(new Address(txOutput.Address), txOutput.Value.Lovelaces, GetTokenBundleBuilder(txOutput.Value.NativeAssets));
        if (!changeValue.IsZero())
        {
            txBodyBuilder.AddOutput(new Address(From), changeValue.Lovelaces, GetTokenBundleBuilder(changeValue.NativeAssets));
        }
        // Build Whole Tx
        var txBuilder = TransactionBuilder.Create.SetBody(txBodyBuilder);
        // Key Witnesses if signing key is passed in
        if (!string.IsNullOrWhiteSpace(SigningKey))
        {
            var paymentSkey = TxUtils.GetPrivateKeyFromBech32SigningKey(SigningKey);
            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(paymentSkey.GetPublicKey(false), paymentSkey);
            txBuilder.SetWitnesses(witnesses);
        }
        // Metadata
        var auxDataBuilder = !string.IsNullOrWhiteSpace(Message)
            ? AuxiliaryDataBuilder.Create.AddMetadata(MessageStandardKey, BuildMessageMetadata(Message))
            : null;
        if (auxDataBuilder is not null)
        {
            txBuilder.SetAuxData(auxDataBuilder);
        }
        var tx = txBuilder.Build();
        if(MockWitnessCount is not null && MockWitnessCount > 0)
            tx.TransactionWitnessSet = new TransactionWitnessSet();
        // Fee Calculation
        var fee = tx.CalculateAndSetFee(protocolParams.MinFeeA, protocolParams.MinFeeB, MockWitnessCount ?? 0);
        tx.TransactionBody.TransactionOutputs.Last().Value.Coin -= fee;
        var txCborBytes = tx.Serialize();
        if (!string.IsNullOrWhiteSpace(OutFile))
        {
            var txTextEnvelope = new TextEnvelope(TxAlonzoJsonTypeField, "", txCborBytes.ToStringHex());
            await File.WriteAllTextAsync(OutFile, JsonSerializer.Serialize(txTextEnvelope, SerialiserOptions), ct);
        }
        if (Submit)
        {
            var txClient = BackendGateway.GetBackendClient<ITransactionClient>(network);
            using var stream = new MemoryStream(txCborBytes);
            var txSubmissionResponse = await txClient.Submit(stream).ConfigureAwait(false);
            if (!txSubmissionResponse.IsSuccessStatusCode)
            {
                if (txSubmissionResponse.Error is null || string.IsNullOrWhiteSpace(txSubmissionResponse.Error.Content))
                    return CommandResult.FailureBackend($"Koios backend response was unsuccessful");
                return CommandResult.FailureBackend(txSubmissionResponse.Error.Content);
            }
            if (txSubmissionResponse.Content is null)
            {
                return CommandResult.FailureBackend("Koios transaction submission response did not return a valid transaction ID");
            }
            var txId = txSubmissionResponse.Content.TrimStart('"').TrimEnd('"');
            var txHash = HashUtility.Blake2b256(tx.TransactionBody.Serialize(auxDataBuilder?.Build())).ToStringHex();
            var result = txId == txHash ? txId : $"Submission response tx-id: {txId} does not match expected: {txHash}";
            return CommandResult.Success(result);
        }
        return CommandResult.Success(txCborBytes.ToStringHex());
    }

    private (
        bool isValid,
        NetworkType derivedNetworkType,
        ulong lovelaces,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            validationErrors.Add("Invalid option --network must be either testnet or mainnet");
        }

        if (string.IsNullOrWhiteSpace(From))
        {
            validationErrors.Add("Invalid option --from address is required");
        }
        else if (!Bech32.IsValid(From))
        {
            validationErrors.Add("Invalid option --from address is not valid");
        }
        else
        {
            var fromAddress = new Address(From);
            if (networkType != fromAddress.NetworkType || !fromAddress.Prefix.StartsWith("addr"))
            {
                validationErrors.Add(
                    $"Invalid option --from address is not valid for the network {networkType.ToString().ToLower()}");
            }
        }

        if (string.IsNullOrWhiteSpace(To))
        {
            validationErrors.Add("Invalid option --to address is required");
        }
        else if (!Bech32.IsValid(To))
        {
            validationErrors.Add("Invalid option --to address is not valid");
        }
        else
        {
            var toAddress = new Address(To);
            if (networkType != toAddress.NetworkType || !toAddress.Prefix.StartsWith("addr"))
            {
                validationErrors.Add(
                    $"Invalid option --to address is not valid for the network {networkType.ToString().ToLower()}");
            }
        }

        var lovelaces = 0UL;
        if (Lovelaces == 0 && Ada == 0 && !SendAll)
        {
            validationErrors.Add("Invalid options either (--lovelaces | --ada | --send-all) must be supplied");
        }
        else if ((Lovelaces > 0 && SendAll) || (Ada > 0 && SendAll) || (Lovelaces > 0 && Ada > 0))
        {
            validationErrors.Add("Invalid options only one of (--lovelaces | --ada | --send-all) must be supplied");
        }
        else if (!SendAll && Lovelaces == 0 && Ada < 1)
        {
            validationErrors.Add("Invalid option --ada value must be at least 1");
        }
        else if (!SendAll && Ada == 0 && Lovelaces < 1000000)
        {
            validationErrors.Add("Invalid option --lovelaces value must be at least 1000000");
        }
        else // Use lovelaces as the main output variable
        {
            lovelaces = (Ada > 0) ? (ulong)(1000000 * Ada) : Lovelaces;
        }

        if (!string.IsNullOrWhiteSpace(SigningKey))
        {
            if (!Bech32.IsValid(SigningKey))
            {
                validationErrors.Add("Invalid option --signing-key is not a valid signing key");
            }
            else
            {
                _ = Bech32.Decode(SigningKey, out _, out var signingKeyPrefix);
                if (signingKeyPrefix != PaymentExtendedSigningKeyBech32Prefix 
                    && signingKeyPrefix != PaymentSigningKeyBech32Prefix 
                    && signingKeyPrefix != PaymentSharedSigningKeyBech32Prefix)
                {
                    validationErrors.Add(
                    "Invalid option --signing-key is not a valid payment signing key");
                }
            }
        }

        if (Ttl > 0 && Ttl < 4492800)
        {
            validationErrors.Add("Invalid option --ttl slot cannot occur before the Shelley HFC event");
        }

        if (!string.IsNullOrWhiteSpace(OutFile)
            && Path.IsPathFullyQualified(OutFile)
            && !Directory.Exists(Path.GetDirectoryName(OutFile)))
        {
            validationErrors.Add(
                $"Invalid option --out-file path {OutFile} does not exist");
        }
        return (!validationErrors.Any(), networkType, lovelaces, validationErrors);
    }

    private static (
        IEpochClient epochClient,
        INetworkClient networkClient,
        IAddressClient addressClient
        ) GetKoiosClients(NetworkType network) =>
        (BackendGateway.GetBackendClient<IEpochClient>(network),
        BackendGateway.GetBackendClient<INetworkClient>(network),
        BackendGateway.GetBackendClient<IAddressClient>(network));

    private static UnspentTransactionOutput[] MapSourceAddressUtxos(IEnumerable<AddressUtxoSet> addressUtxoSet)
    {
        return addressUtxoSet
            .Select(utxo => new UnspentTransactionOutput(
                utxo.TxHash ?? "n/a",
                utxo.TxIndex,
                new Balance(
                    ulong.Parse(utxo.Value ?? "0"),
                    utxo.AssetList.Select(
                        a => new NativeAssetValue(
                            a.PolicyId ?? "n/a",
                            a.AssetName ?? "n/a",
                            long.Parse(a.Quantity ?? "0")))
                    .ToArray())))
            .ToArray();
    }

    private static Balance MapConsolidatedTxInputValue(
        UnspentTransactionOutput[] sourceAddressUtxos,
        NativeAssetValue[]? nativeAssetsToMint = null)
    {
        if (nativeAssetsToMint is not null && nativeAssetsToMint.Length > 0)
        {
            return sourceAddressUtxos
                .Select(utxo => utxo.Value)
                .Concat(new[] { new Balance(0, nativeAssetsToMint) })
                .Sum();
        }
        return sourceAddressUtxos.Select(utxo => utxo.Value).Sum();
    }

    private static ITokenBundleBuilder GetTokenBundleBuilder(NativeAssetValue[] nativeAssetValues)
    {
        var tokenBuilder = TokenBundleBuilder.Create;
        foreach (var nativeAssetValue in nativeAssetValues)
        {
            tokenBuilder = tokenBuilder.AddToken(
                nativeAssetValue.PolicyId.HexToByteArray(), 
                nativeAssetValue.AssetName.HexToByteArray(), 
                nativeAssetValue.Quantity);
        }
        return tokenBuilder;
    }

    private static IDictionary<string, object>? BuildMessageMetadata(string? message)
    {
        if (string.IsNullOrWhiteSpace(message)) return null;
        return new Dictionary<string, object>
        {
            {  "msg", message.Length > MaxMetadataStringLength ? SplitString(message) : message }
        };

        static string[] SplitString(string? value)
        {
            if (value is null)
                return Array.Empty<string>();
            if (value.Length <= MaxMetadataStringLength)
                return new[] { value };

            var offsetLength = MaxMetadataStringLength - 1;
            var itemsLength = (value.Length + offsetLength) / MaxMetadataStringLength;
            var items = new string[itemsLength];
            for (var i = 0; i < itemsLength; i++)
            {
                var substringStartIndex = i * MaxMetadataStringLength;
                var substringLength = (substringStartIndex + MaxMetadataStringLength) <= value.Length
                    ? MaxMetadataStringLength
                    : value.Length % MaxMetadataStringLength; 
                var segment = value.Substring(substringStartIndex, substringLength);
                items[i] = segment;
            }
            return items;
        }
    }
}
