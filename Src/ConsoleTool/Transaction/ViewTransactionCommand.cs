using CardanoSharp.Wallet.Extensions.Models.Transactions;
using System.Text.Json;

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
        var json = JsonSerializer.Serialize(tx);

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
}
