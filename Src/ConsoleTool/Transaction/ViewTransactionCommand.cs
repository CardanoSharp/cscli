using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using System.Text.Json;

namespace Cscli.ConsoleTool.Transaction;

public record TxIn(
    string TransactionId,
    uint TransactionIndex);

public record TxOut(
    string Address,
    AggregateValue Value);

public record TransactionBody(
    IEnumerable<TxIn> TransactionInputs,
    IEnumerable<TxOut> TransactionOutputs,
    IEnumerable<NativeAssetValue> NativeAssets,
    ulong Fee,
    uint? Ttl,
    string MetadataHash,
    uint? TransactionStartInterval);

public record Transaction(
    TransactionBody TransactionBody);

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

        var transaction = new Transaction(
            new TransactionBody(
                tx.TransactionBody.TransactionInputs.Select(txI => new TxIn(txI.TransactionId.ToStringHex(), txI.TransactionIndex)).ToArray(),
                tx.TransactionBody.TransactionOutputs.Select(
                    txO => MapTxOut(txO)).ToArray(),
                MapNativeAssets(tx.TransactionBody.Mint),
                tx.TransactionBody.Fee,
                tx.TransactionBody.Ttl,
                tx.TransactionBody.MetadataHash,
                tx.TransactionBody.TransactionStartInterval));

        var json = JsonSerializer.Serialize(transaction);

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
        return multiAsset.Keys.SelectMany(
            maKey => multiAsset[maKey].Token.Select(
                mat => new NativeAssetValue(maKey.ToStringHex(), mat.Key.ToStringHex(), mat.Value)))
            .ToArray();
    }
}
