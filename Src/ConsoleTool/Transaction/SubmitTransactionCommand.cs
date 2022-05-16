using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Enums;
using Cscli.ConsoleTool.Koios;

namespace Cscli.ConsoleTool.Transaction;

public class SubmitTransactionCommand : ICommand
{
    public string? CborHex { get; init; }
    public string? Network { get; init; } = "testnet";

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, networkType, errors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors));
        }

        var txClient = BackendGateway.GetBackendClient<ITransactionClient>(networkType);
        try
        {
            var txRawBytes = Convert.FromHexString(CborHex ?? throw new ArgumentException(nameof(CborHex)));
            using var stream = new MemoryStream(txRawBytes);
            var txHash = await txClient.Submit(stream).ConfigureAwait(false); 
            return CommandResult.Success(txHash);
        }
        catch (Exception ex)
        {
            return CommandResult.FailureUnhandledException("Unexpected error", ex);
        }
    }

    private (
        bool isValid,
        NetworkType derivedNetworkType,
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
                $"Invalid option --tx-hex is required");
        }

        return (!validationErrors.Any(), networkType, validationErrors);
    }
}
