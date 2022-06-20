using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;

namespace Cscli.ConsoleTool.Transaction;

public class SignTransactionCommand : ICommand
{
    public string? CborHex { get; init; }
    public string? SigningKeys { get; init; }

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, txCborBytes, signingKeys, errors) = Validate();
        if (!isValid)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors)));
        }

        var tx = txCborBytes.DeserializeTransaction();
        foreach (var signingKey in signingKeys)
        {
            var paymentSkey = TxUtils.GetPrivateKeyFromBech32SigningKey(signingKey);
            var paymentVkey = paymentSkey.GetPublicKey();
            tx.TransactionWitnessSet.VKeyWitnesses.Add(new CardanoSharp.Wallet.Models.Transactions.VKeyWitness
            {
                SKey = paymentSkey,
                VKey = paymentVkey
            });
        }
        txCborBytes = tx.Serialize();
        return ValueTask.FromResult(CommandResult.Success(txCborBytes.ToStringHex()));
    }

    private (
        bool isValid,
        byte[] txCborBytes,
        string[] signingKeys,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var txCborBytes = Array.Empty<byte>();
        var signingKeys = Array.Empty<string>();
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
                txCborBytes = Convert.FromHexString(CborHex);
            }
            catch (FormatException)
            {
                validationErrors.Add(
                    $"Invalid option --cbor-hex {CborHex} is not in hexadecimal format");
            }
        }
        if (string.IsNullOrWhiteSpace(SigningKeys))
        {
            validationErrors.Add("Invalid option --signing-keys is required");
        }
        else
        {
            signingKeys = SigningKeys.Split(',');
            for (int i = 0; i < signingKeys.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(signingKeys[i]))
                {
                    validationErrors.Add($"Invalid option --signing-keys[{i}] is empty or whitespace");
                }
            }
        }
        return (!validationErrors.Any(), txCborBytes, signingKeys, validationErrors);
    }
}
