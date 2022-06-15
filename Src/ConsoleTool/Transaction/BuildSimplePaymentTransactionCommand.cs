using CardanoSharp.Koios.Sdk;
using CardanoSharp.Koios.Sdk.Contracts;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Utilities;
using Cscli.ConsoleTool.Koios;

namespace Cscli.ConsoleTool.Transaction;

public class BuildSimplePaymentTransactionCommand : ICommand
{
    public string? From{ get; init; } // Address Bech32
    public string? SigningKey { get; init; } // Payment Signing Key Bech32 for From Address
    public string? To{ get; init; } // Address Bech32
    public ulong Lovelaces { get; init; }
    public string? Message { get; init; }
    public string? Network { get; init; }
    public bool Submit { get; set; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, network, errors) = Validate();
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
        var txOutput = new PendingTransactionOutput(To, new AggregateValue(Lovelaces, Array.Empty<NativeAssetValue>()));
        var txChangeOutput = consolidatedInputValue.Subtract(txOutput.Value);
        // Build Tx Body
        var txBodyBuilder = TransactionBodyBuilder.Create
            .SetFee(0)
            .SetTtl((uint)tip.AbsSlot + 7200); // 2 hours
        // Inputs/Outputs
        foreach (var txInput in sourceAddressUtxos)
        {
            txBodyBuilder.AddInput(txInput.TxHash, txInput.OutputIndex);
        }
        txBodyBuilder.AddOutput(new Address(txOutput.Address), txOutput.Value.Lovelaces);
        txBodyBuilder.AddOutput(new Address(From), txChangeOutput.Lovelaces);
        // Witnesses
        var paymentSkey = TxUtils.GetPrivateKeyFromBech32SigningKey(SigningKey);
        var auxDataBuilder = AuxiliaryDataBuilder.Create.AddMetadata(674, BuildMessageMetadata(Message));
        var witnesses = TransactionWitnessSetBuilder.Create
            .AddVKeyWitness(paymentSkey.GetPublicKey(false), paymentSkey);
        // Build full Tx
        var tx = TransactionBuilder.Create
            .SetBody(txBodyBuilder)
            .SetWitnesses(witnesses)
            .SetAuxData(auxDataBuilder)
            .Build();
        // Fee Calculation
        var fee = tx.CalculateFee(protocolParams.MinFeeA, protocolParams.MinFeeB);
        txBodyBuilder.SetFee(fee);
        tx.TransactionBody.TransactionOutputs.Last().Value.Coin -= fee;
        var txCborBytes = tx.Serialize();
        if (Submit)
        {
            var txClient = BackendGateway.GetBackendClient<ITransactionClient>(network);
            using var stream = new MemoryStream(txCborBytes);
            var txHash = (await txClient.Submit(stream)).Content;
            var txId = HashUtility.Blake2b256(tx.TransactionBody.Serialize(auxDataBuilder.Build())).ToStringHex();
            return CommandResult.Success(txHash ?? throw new ApplicationException($"Trasaction submission result is null but should be {txId}"));
        }
        return CommandResult.Success(txCborBytes.ToStringHex());
    }

    private (
        bool isValid,
        NetworkType derivedNetworkType,
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

        if (string.IsNullOrWhiteSpace(SigningKey))
        {
            validationErrors.Add("Invalid option --signing-key is required");
        }
        else if (!Bech32.IsValid(SigningKey))
        {
            validationErrors.Add("Invalid option --signing-key is not a valid signing key");
        }
        else
        {
            _ = Bech32.Decode(SigningKey, out _, out var signingKeyPrefix);
            if (signingKeyPrefix != "addr_xsk" && signingKeyPrefix != "addr_sk" && signingKeyPrefix != "addr_shared_sk")
            {
                validationErrors.Add(
                "Invalid option --signing-key is not a valid payment signing key");
            }
        }
        return (!validationErrors.Any(), networkType, validationErrors);
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
                new AggregateValue(
                    ulong.Parse(utxo.Value),
                    utxo.AssetList.Select(
                        a => new NativeAssetValue(
                            a.PolicyId,
                            a.AssetName,
                            ulong.Parse(a.Quantity)))
                    .ToArray())))
            .ToArray();
    }

    private static AggregateValue BuildConsolidatedTxInputValue(
        UnspentTransactionOutput[] sourceAddressUtxos,
        NativeAssetValue[]? nativeAssetsToMint = null)
    {
        if (nativeAssetsToMint != null && nativeAssetsToMint.Length > 0)
        {
            return sourceAddressUtxos
                .Select(utxo => utxo.Value)
                .Concat(new[] { new AggregateValue(0, nativeAssetsToMint) })
                .Sum();
        }
        return sourceAddressUtxos.Select(utxo => utxo.Value).Sum();
    }

    private static IDictionary<string, object> BuildMessageMetadata(string? message)
    {
        const int maxLength = 64;
        if (string.IsNullOrWhiteSpace(message))
        {
            return new Dictionary<string, object>()
            {
                {  "msg", "Built using CardanoSharp" }
            };
        }
        return new Dictionary<string, object>
        {
            {  "msg", message.Length > maxLength ? SplitString(message) : message }
        };

        static string[] SplitString(string? value)
        {
            if (value == null)
                return Array.Empty<string>();
            if (value.Length <= maxLength)
                return new[] { value };

            var offsetLength = maxLength - 1;
            var itemsLength = (value.Length + offsetLength) / maxLength;
            var items = new string[itemsLength];
            for (var i = 0; i < itemsLength; i++)
            {
                var substringStartIndex = i * maxLength;
                var substringLength = (substringStartIndex + maxLength) <= value.Length
                    ? maxLength
                    : value.Length % maxLength; 
                var segment = value.Substring(substringStartIndex, substringLength);
                items[i] = segment;
            }
            return items;
        }
    }
}
