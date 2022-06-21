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

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, txCborBytes, errors) = Validate();
        if (!isValid)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors)));
        }

        var tx = txCborBytes.DeserializeTransaction();

        var transaction = new Tx(
            tx.IsValid,
            new TxBody(
                tx.TransactionBody.TransactionInputs.Select(txI => new TxIn(txI.TransactionId.ToStringHex(), txI.TransactionIndex)).ToArray(),
                tx.TransactionBody.TransactionOutputs.Select(
                    txO => MapTxOut(txO)).ToArray(),
                MapNativeAssets(tx.TransactionBody.Mint),
                tx.TransactionBody.Fee,
                tx.TransactionBody.Ttl,
                tx.TransactionBody.MetadataHash,
                tx.TransactionBody.TransactionStartInterval),
            new TxWitnessSet(
                tx.TransactionWitnessSet.VKeyWitnesses.Select(
                    vw => new TxVKeyWitness(vw.VKey.Key.ToStringHex(), vw.Signature.ToStringHex()))
                .ToArray(),
                tx.TransactionWitnessSet.NativeScripts.Select(MapNativeScript).ToArray()),
            new TxAuxData(tx.AuxiliaryData.Metadata));

        var json = JsonSerializer.Serialize(transaction, SerialiserOptions);

        return ValueTask.FromResult(CommandResult.Success(json));
    }

    private (
        bool isValid,
        byte[] txCborBytes,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(CborHex))
        {
            validationErrors.Add(
                $"Invalid option --cbor-hex is required");
        }
        else
        {
            try
            {
                var txCborBytes = Convert.FromHexString(CborHex);
                return (!validationErrors.Any(), txCborBytes, validationErrors);
            }
            catch (FormatException)
            {
                validationErrors.Add(
                    $"Invalid option --cbor-hex {CborHex} is not in hexadecimal format");
            }
        }
        return (!validationErrors.Any(), Array.Empty<byte>(), validationErrors);
    }

    private static TxOut MapTxOut(TransactionOutput txO)
    {
        return new TxOut(
            new Address("addr", txO.Address).ToString(), // TODO: address deserialization based on network
            new AggregateValue(txO.Value.Coin, MapNativeAssets(txO.Value.MultiAsset).ToArray()));
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
