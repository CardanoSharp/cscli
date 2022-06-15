using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;

namespace Cscli.ConsoleTool.Transaction;

public class SignTransactionCommand : ICommand
{
    public string? CborHex { get; init; }
    public string? SigningKey { get; init; }

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, txCborBytes, errors) = Validate();
        if (!isValid)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors)));
        }

        var tx = txCborBytes.DeserializeTransaction();
        var paymentSkey = TxUtils.GetPrivateKeyFromBech32SigningKey(SigningKey);
        var paymentVkey = paymentSkey.GetPublicKey();
        tx.TransactionWitnessSet.VKeyWitnesses.Add(new CardanoSharp.Wallet.Models.Transactions.VKeyWitness
        {
            SKey = paymentSkey,
            VKey = paymentVkey
        });
        txCborBytes = tx.Serialize();
        return ValueTask.FromResult(CommandResult.Success(txCborBytes.ToStringHex()));
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
        if (string.IsNullOrWhiteSpace(SigningKey))
        {
            validationErrors.Add("Invalid option --signing-key is required");
        }
        return (!validationErrors.Any(), Array.Empty<byte>(), validationErrors);
    }
}
