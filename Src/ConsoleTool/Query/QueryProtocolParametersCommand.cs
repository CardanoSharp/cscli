using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Enums;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Query;

public class QueryProtocolParametersCommand : ICommand
{
    public string? Network { get; init; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --network must be either testnet or mainnet");
        }

        var epochClient = BackendGateway.GetBackendClient<IEpochClient>(networkType);
        try
        {
            var protocolParams = await epochClient.GetProtocolParameters(null, limit:1).ConfigureAwait(false);
            if (!protocolParams.IsSuccessStatusCode || protocolParams.Content == null)
                return CommandResult.FailureBackend($"Koios backend response was unsuccessful");

            var json = JsonSerializer.Serialize(protocolParams.Content.First(), SerialiserOptions);
            return CommandResult.Success(json);
        }
        catch (Exception ex)
        {
            return CommandResult.FailureUnhandledException("Unexpected error", ex);
        }
    }
}
