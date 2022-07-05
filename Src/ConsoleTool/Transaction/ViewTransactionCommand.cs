using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Utilities;
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

        var deSerialisedTx = txCborBytes.DeserializeTransaction();
        var txId = deSerialisedTx.TransactionWitnessSet is null
            ? "n/a"
            : HashUtility.Blake2b256(deSerialisedTx.TransactionBody.Serialize(deSerialisedTx.AuxiliaryData)).ToStringHex();
        var tx = new Tx(
            Id: txId,
            IsValid: deSerialisedTx.IsValid,
            TransactionBody: new TxBody(
                deSerialisedTx.TransactionBody.TransactionInputs.Select(
                    txI => new TxIn(txI.TransactionId.ToStringHex(), txI.TransactionIndex)).ToArray(),
                deSerialisedTx.TransactionBody.TransactionOutputs.Select(
                    txO => MapTxOut(network, txO)).ToArray(),
                MapNativeAssets(deSerialisedTx.TransactionBody.Mint),
                deSerialisedTx.TransactionBody.Fee,
                deSerialisedTx.TransactionBody.Ttl,
                deSerialisedTx.AuxiliaryData is null ? null : HashUtility.Blake2b256(deSerialisedTx.AuxiliaryData.GetCBOR().EncodeToBytes()).ToStringHex(),
                deSerialisedTx.TransactionBody.TransactionStartInterval),
            TransactionWitnessSet: deSerialisedTx.TransactionWitnessSet is null 
                ? null
                : new TxWitnessSet(
                    deSerialisedTx.TransactionWitnessSet.VKeyWitnesses.Select(
                        vw => new TxVKeyWitness(vw.VKey.Key.ToStringHex(), vw.Signature.ToStringHex()))
                    .ToArray(),
                    deSerialisedTx.TransactionWitnessSet.NativeScripts.Select(MapNativeScript).ToArray()),
            AuxiliaryData: new TxAuxData(deSerialisedTx.AuxiliaryData?.Metadata ?? new Dictionary<int, object>()));
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
        if (multiAsset is null)
            return Array.Empty<NativeAssetValue>();

        return multiAsset.Keys.SelectMany(
            maKey => multiAsset[maKey].Token.Select(
                mat => new NativeAssetValue(maKey.ToStringHex(), mat.Key.ToStringHex(), mat.Value)))
            .ToArray();
    }

    private static TxNativeScript MapNativeScript(NativeScript nativeScript)
    {
        if (nativeScript is null)
            return new TxNativeScript("");

        return new TxNativeScript(BuildNativeScriptString(nativeScript));

        static string BuildNativeScriptString(NativeScript ns)
        {
            if (ns.ScriptPubKey is not null)
                return $"PubKey({ns.ScriptPubKey.KeyHash.ToStringHex()})";
            if (ns.InvalidAfter is not null)
                return $"InvalidAfter({ns.InvalidAfter.After})";
            if (ns.InvalidBefore is not null)
                return $"InvalidBefore({ns.InvalidBefore.Before})";
            if (ns.ScriptNofK is not null)
                return $"NofK({ns.ScriptNofK.N}of{ns.ScriptNofK.NativeScripts.Count}[{string.Join(',',ns.ScriptNofK.NativeScripts.Select(BuildNativeScriptString))}])";
            if (ns.ScriptAny is not null)
                return $"Any([{string.Join(',', ns.ScriptAny.NativeScripts.Select(BuildNativeScriptString))}])";
            if (ns.ScriptAll is not null)
                return $"All([{string.Join(',', ns.ScriptAll.NativeScripts.Select(BuildNativeScriptString))}])";
            return "";
        }
    }
}
