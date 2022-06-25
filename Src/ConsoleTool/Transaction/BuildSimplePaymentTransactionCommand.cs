using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
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
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Transaction;

public class BuildSimplePaymentTransactionCommand : ICommand
{
    public string? From { get; init; } // Address Bech32
    public string? SigningKey { get; init; } // Payment Signing Key Bech32 for From Address
    public string? To{ get; init; } // Address Bech32
    public ulong Lovelaces { get; init; } // Either Lovelaces or Ada
    public decimal Ada { get; init; } 
    public bool SendAll { get; init; }
    public string? Message { get; init; }
    public string? Network { get; init; }
    public bool Submit { get; init; }
    public string? OutFile { get; init; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, network, lovelaces, errors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(string.Join(Environment.NewLine, errors));
        }

        (var epochClient, var networkClient, var addressClient) = GetKoiosClients(network);
        var tip = (await networkClient.GetChainTip()).Content.First();
        var protocolParams = (await epochClient.GetProtocolParameters(tip.Epoch.ToString())).Content.First();
        var sourceAddressInfo = (await addressClient.GetAddressInformation(From)).Content;
        var sourceAddressUtxos = BuildSourceAddressUtxos(sourceAddressInfo.First().UtxoSets);
        var consolidatedInputValue = BuildConsolidatedTxInputValue(sourceAddressUtxos);
        var txOutput = SendAll 
            ? new PendingTransactionOutput(To, consolidatedInputValue)
            : new PendingTransactionOutput(To, new Balance(lovelaces, Array.Empty<NativeAssetValue>()));
        var changeValue = consolidatedInputValue.Subtract(txOutput.Value);

        // Build Tx Body
        var txBodyBuilder = TransactionBodyBuilder.Create
            .SetFee(0)
            .SetTtl((uint)tip.AbsSlot + TtlTipOffsetSlots); 
        // Inputs
        foreach (var txInput in sourceAddressUtxos)
        {
            txBodyBuilder.AddInput(txInput.TxHash, txInput.OutputIndex);
        }
        // Outputs
        txBodyBuilder.AddOutput(new Address(txOutput.Address), txOutput.Value.Lovelaces);
        if (!changeValue.IsZero())
        {
            txBodyBuilder.AddOutput(new Address(From), changeValue.Lovelaces, GetTokenBundleBuilder(changeValue.NativeAssets));
        }
        // Metadata
        var auxDataBuilder = !string.IsNullOrWhiteSpace(Message)
            ? AuxiliaryDataBuilder.Create.AddMetadata(MessageStandardKey, BuildMessageMetadata(Message))
            : null;

        // Build Whole Tx
        var txBuilder = TransactionBuilder.Create
            .SetBody(txBodyBuilder);
        // Key Witnesses if signing key is passed in
        if (!string.IsNullOrWhiteSpace(SigningKey))
        {
            var paymentSkey = TxUtils.GetPrivateKeyFromBech32SigningKey(SigningKey);
            var witnesses = TransactionWitnessSetBuilder.Create
                .AddVKeyWitness(paymentSkey.GetPublicKey(false), paymentSkey);
            txBuilder.SetWitnesses(witnesses);
        }
        if (auxDataBuilder is not null)
        {
            txBuilder.SetAuxData(auxDataBuilder);
        }
        var tx = txBuilder.Build();
        // Fee Calculation
        var fee = tx.CalculateFee(protocolParams.MinFeeA, protocolParams.MinFeeB);
        txBodyBuilder.SetFee(fee);
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
            var response = (await txClient.Submit(stream));
            var txHash = response.Content?.TrimStart('"').TrimEnd('"');
            var txId = HashUtility.Blake2b256(tx.TransactionBody.Serialize(auxDataBuilder?.Build())).ToStringHex();
            return CommandResult.Success(txHash ?? throw new ApplicationException($"Trasaction submission result is null but should be {txId}"));
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
                    $"Invalid option --from address is not valid for the network {networkType}");
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
                    $"Invalid option --to address is not valid for the network {networkType}");
            }
        }

        var lovelaces = 0UL;
        if (Lovelaces <= 0 && Ada <= 0 && !SendAll)
        {
            validationErrors.Add("Invalid options either (--lovelaces | --ada | --send-all) must be supplied");
        }
        else if ((Lovelaces > 0 || Ada > 0) && SendAll)
        {
            validationErrors.Add("Invalid options only one of (--lovelaces | --ada | --send-all) must be supplied");
        }
        else if (Lovelaces > 0 && Lovelaces < 1000000)
        {
            validationErrors.Add("Invalid option --lovelaces value must be at least 1000000");
        }
        else
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

    private static UnspentTransactionOutput[] BuildSourceAddressUtxos(IEnumerable<AddressUtxoSet> addressUtxoSet)
    {
        return addressUtxoSet
            .Select(utxo => new UnspentTransactionOutput(
                utxo.TxHash,
                utxo.TxIndex,
                new Balance(
                    ulong.Parse(utxo.Value),
                    utxo.AssetList.Select(
                        a => new NativeAssetValue(
                            a.PolicyId,
                            a.AssetName,
                            ulong.Parse(a.Quantity)))
                    .ToArray())))
            .ToArray();
    }

    private static Balance BuildConsolidatedTxInputValue(
        UnspentTransactionOutput[] sourceAddressUtxos,
        NativeAssetValue[]? nativeAssetsToMint = null)
    {
        if (nativeAssetsToMint != null && nativeAssetsToMint.Length > 0)
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
            if (value == null)
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
