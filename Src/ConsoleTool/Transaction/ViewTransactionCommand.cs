using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Transaction;

public class ViewTransactionCommand : ICommand
{
    public string? CborHex { get; init; }
    public string? Network { get; init; }

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, network, txCborBytes, errors) = Validate();
        if (!isValid)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors)));
        }

        var deSerialisedTransaction = txCborBytes.DeserializeTransaction();
        var tx = new Tx(
            deSerialisedTransaction.IsValid,
            new TxBody(
                deSerialisedTransaction.TransactionBody.TransactionInputs.Select(
                    txI => new TxIn(txI.TransactionId.ToStringHex(), txI.TransactionIndex)).ToArray(),
                deSerialisedTransaction.TransactionBody.TransactionOutputs.Select(
                    txO => MapTxOut(network, txO)).ToArray(),
                MapNativeAssets(deSerialisedTransaction.TransactionBody.Mint),
                deSerialisedTransaction.TransactionBody.Fee,
                deSerialisedTransaction.TransactionBody.Ttl,
                deSerialisedTransaction.TransactionBody.MetadataHash,
                deSerialisedTransaction.TransactionBody.TransactionStartInterval),
            deSerialisedTransaction.TransactionWitnessSet is null 
                ? new TxWitnessSet(Array.Empty<TxVKeyWitness>(), Array.Empty<TxNativeScript>()) 
                : new TxWitnessSet(
                    deSerialisedTransaction.TransactionWitnessSet.VKeyWitnesses.Select(
                        vw => new TxVKeyWitness(vw.VKey.Key.ToStringHex(), vw.Signature.ToStringHex()))
                    .ToArray(),
                    deSerialisedTransaction.TransactionWitnessSet.NativeScripts.Select(MapNativeScript).ToArray()),
            new TxAuxData(deSerialisedTransaction.AuxiliaryData?.Metadata ?? new Dictionary<int, object>()));
        var json = JsonSerializer.Serialize(tx, SerialiserOptions);
        return ValueTask.FromResult(CommandResult.Success(json));
    }

    private (
        bool isValid,
        NetworkType networkType,
        byte[] txCborBytes,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        var txCborBytes = Array.Empty<byte>();
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            validationErrors.Add("Invalid option --network must be either testnet or mainnet");
        }
        if (string.IsNullOrWhiteSpace(CborHex))
        {
            validationErrors.Add(
                $"Invalid option --cbor-hex is required");
        }
        else
        {
            try
            {
                txCborBytes = Convert.FromHexString(CborHex);
            }
            catch (FormatException)
            {
                validationErrors.Add(
                    $"Invalid option --cbor-hex {CborHex} is not in hexadecimal format");
            }
        }
        return (!validationErrors.Any(), networkType, txCborBytes, validationErrors);
    }

    private static TxOut MapTxOut(NetworkType network, TransactionOutput txO)
    {
        var addrPrefix = network == NetworkType.Mainnet ? AddressMainnetBech32Prefix : AddressTestnetBech32Prefix;
        return new TxOut(
            new Address(addrPrefix, txO.Address).ToString(), 
            new Balance(txO.Value.Coin, MapNativeAssets(txO.Value.MultiAsset).ToArray()));
    }

    private static NativeAssetValue[] MapNativeAssets(IDictionary<byte[], NativeAsset> multiAsset)
    {
        if (multiAsset == null)
            return Array.Empty<NativeAssetValue>();

        return multiAsset.Keys.SelectMany(
            maKey => multiAsset[maKey].Token.Select(
                mat => new NativeAssetValue(maKey.ToStringHex(), mat.Key.ToStringHex(), mat.Value)))
            .ToArray();
    }

    private static TxNativeScript MapNativeScript(NativeScript nativeScript)
    {
        if (nativeScript == null)
            return new TxNativeScript("");

        return new TxNativeScript(BuildNativeScriptString(nativeScript));

        static string BuildNativeScriptString(NativeScript ns)
        {
            if (ns.ScriptPubKey != null)
                return $"PubKey({ns.ScriptPubKey.KeyHash.ToStringHex()})";
            if (ns.InvalidAfter != null)
                return $"InvalidAfter({ns.InvalidAfter.After})";
            if (ns.InvalidBefore != null)
                return $"InvalidBefore({ns.InvalidBefore.Before})";
            if (ns.ScriptNofK != null)
                return $"NofK({ns.ScriptNofK.N}of{ns.ScriptNofK.NativeScripts.Count}[{string.Join(',',ns.ScriptNofK.NativeScripts.Select(BuildNativeScriptString))}])";
            if (ns.ScriptAny != null)
                return $"Any([{string.Join(',', ns.ScriptAny.NativeScripts.Select(BuildNativeScriptString))}])";
            if (ns.ScriptAll != null)
                return $"All([{string.Join(',', ns.ScriptAll.NativeScripts.Select(BuildNativeScriptString))}])";
            return "";
        }
    }
}
