using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Enums;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Query;

public class QueryTransactionInfoCommand : ICommand
{
    public string? TxId { get; init; }
    public string? Network { get; init; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, networkType, errors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors));
        }

        var transactionClient = BackendGateway.GetBackendClient<ITransactionClient>(networkType);
        try
        {
#pragma warning disable CS8604 // Possible null reference warning suppressed because of validation above
            var txInfo = await transactionClient.GetTransactionInformation(
                new GetTransactionRequest { TxHashes = new List<string>{ TxId } }).ConfigureAwait(false);
#pragma warning restore CS8604
            var json = JsonSerializer.Serialize(txInfo, SerialiserOptions);
            return CommandResult.Success(json);
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
        if (string.IsNullOrWhiteSpace(TxId))
        {
            validationErrors.Add(
                $"Invalid option --tx-id is required");
        }
        return (!validationErrors.Any(), networkType, validationErrors);
    }
}
