using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Enums;
using Cscli.ConsoleTool.Koios;

namespace Cscli.ConsoleTool.Transaction;

public class SubmitTransactionCommand : ICommand
{
    public string? CborHex { get; init; }
    public string? Network { get; init; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, networkType, txCborBytes, errors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors));
        }

        var txClient = BackendGateway.GetBackendClient<ITransactionClient>(networkType);
        try
        {
            using var stream = new MemoryStream(txCborBytes);
            var txSubmissionResponse = await txClient.Submit(stream).ConfigureAwait(false);
            if (!txSubmissionResponse.IsSuccessStatusCode)
            {
                if (txSubmissionResponse.Error is null || string.IsNullOrWhiteSpace(txSubmissionResponse.Error.Content))
                    return CommandResult.FailureBackend($"Koios backend response was unsuccessful");
                return CommandResult.FailureBackend(txSubmissionResponse.Error.Content);
            }
            if (txSubmissionResponse.Content is null)
            {
                return CommandResult.FailureBackend("Koios transaction submission response did not return a valid transaction ID");
            }
            return CommandResult.Success(txSubmissionResponse.Content.TrimStart('"').TrimEnd('"'));
        }
        catch (Exception ex)
        {
            return CommandResult.FailureUnhandledException("Unexpected error", ex);
        }
    }

    private (
        bool isValid,
        NetworkType derivedNetworkType,
        byte[] txCborBytes,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            validationErrors.Add(
                $"Invalid option --network must be either testnet or mainnet");
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
                var txCborBytes = Convert.FromHexString(CborHex);
                return (!validationErrors.Any(), networkType, txCborBytes, validationErrors);
            }
            catch (FormatException)
            {
                validationErrors.Add(
                    $"Invalid option --cbor-hex {CborHex} is not in hexadecimal format");
            }
        }
        return (!validationErrors.Any(), networkType, Array.Empty<byte>(), validationErrors);
    }
}
