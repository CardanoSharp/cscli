﻿using CardanoSharp.Koios.Client;
using CardanoSharp.Wallet.Enums;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Query;

public class QueryTipCommand : ICommand
{
    public string? Network { get; init; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --network must be either testnet or mainnet");
        }

        var networkClient = BackendGateway.GetBackendClient<INetworkClient>(networkType);
        try
        {
            var chainTip = await networkClient.GetChainTip().ConfigureAwait(false);
            if (!chainTip.IsSuccessStatusCode || chainTip.Content is null)
                return CommandResult.FailureBackend($"Koios backend response was unsuccessful");

            var json = JsonSerializer.Serialize(chainTip.Content.First(), SerialiserOptions);
            return CommandResult.Success(json);
        }
        catch (Exception ex)
        {
            return CommandResult.FailureUnhandledException("Unexpected error", ex);
        }
    }
}
