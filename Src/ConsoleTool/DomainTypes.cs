namespace Cscli.ConsoleTool;

public record struct NativeAssetValue(string PolicyId, string AssetName, ulong Quantity);

public record struct AggregateValue(ulong Lovelaces, NativeAssetValue[] NativeAssets);

public record struct PendingTransactionOutput(string Address, AggregateValue Value);

public record UnspentTransactionOutput(string TxHash, uint OutputIndex, AggregateValue Value)
{
    public override int GetHashCode() => ToString().GetHashCode();
    public override string ToString() => $"{TxHash}_{OutputIndex}";
    bool IEquatable<UnspentTransactionOutput>.Equals(UnspentTransactionOutput? other)
        => other != null && TxHash == other.TxHash && OutputIndex == other.OutputIndex;
    public ulong Lovelaces => Value.Lovelaces;
}

public record TextEnvelope(string? Type, string? Description, string? CborHex);

public record Tx(
    bool IsValid,
    TxBody TransactionBody,
    TxWitnessSet TransactionWitnessSet,
    TxAuxData AuxiliaryData);

public record TxBody(
    IEnumerable<TxIn> TransactionInputs,
    IEnumerable<TxOut> TransactionOutputs,
    IEnumerable<NativeAssetValue> NativeAssets,
    ulong Fee,
    uint? Ttl,
    string MetadataHash,
    uint? TransactionStartInterval);

public record TxIn(
    string TransactionId,
    uint TransactionIndex);

public record TxOut(
    string Address,
    AggregateValue Value);

public record TxWitnessSet(
    IEnumerable<TxVKeyWitness> VKeyWitnesses,
    IEnumerable<TxNativeScript> NativeScripts);

public record TxVKeyWitness(string Verificationkey, string Signature);

public record TxNativeScript(string Type);

public record TxAuxData(Dictionary<int, object> Metadata);

