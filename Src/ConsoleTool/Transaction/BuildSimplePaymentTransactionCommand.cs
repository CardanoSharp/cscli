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
using Cscli.ConsoleTool.Koios;

namespace Cscli.ConsoleTool.Transaction;

public class BuildSimplePaymentTransactionCommand : ICommand
{
    public string? From{ get; init; } // Address Bech32
    public string? SigningKey { get; init; } // Payment Signing Key Bech32 for From Address
    public string? To{ get; init; } // Address Bech32
    public ulong Lovelaces { get; init; }
    public string? Message { get; init; }
    public string? OutFile { get; init; }
    public string? FromAddressSigningKey { get; init; }
    public string? Network { get; init; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            return CommandResult.FailureInvalidOptions($"Invalid option --network must be either testnet or mainnet");
        }
        // TODO: Other validation e.g. FromAddress, FromAddressSigningKey, ToAddress, Lovelaces amount

        (var epochClient, var networkClient, var addressClient) = GetKoiosClients(networkType);
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
        var paymentSkey = GetPrivateKeyFromBech32SigningKey(SigningKey);
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
        // var txId = HashUtility.Blake2b256(tx.TransactionBody.Serialize(auxDataBuilder.Build())).ToStringHex();
        return CommandResult.Success(tx.Serialize().ToStringHex());
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

    private static PrivateKey GetPrivateKeyFromBech32SigningKey(string bech32EncodedSigningKey)
    {
        var keyBytes = Bech32.Decode(bech32EncodedSigningKey, out _, out _);
        return new PrivateKey(keyBytes[..64], keyBytes[64..]);
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
