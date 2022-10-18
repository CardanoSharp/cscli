using CardanoSharp.Koios.Client;
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
        var (isValid, networkType, txId, errors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors));
        }

        var transactionClient = BackendGateway.GetBackendClient<ITransactionClient>(networkType);
        try
        {
            var txInfo = await transactionClient.GetTransactionInformation(
                new GetTransactionRequest { TxHashes = new List<string>{ txId } }).ConfigureAwait(false);
            if (!txInfo.IsSuccessStatusCode || txInfo.Content is null)
                return CommandResult.FailureBackend($"Koios backend response was unsuccessful");

            var json = JsonSerializer.Serialize(txInfo.Content, SerialiserOptions);
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
        string txId,
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
        return (!validationErrors.Any(), networkType, TxId ?? "", validationErrors);
    }
}
