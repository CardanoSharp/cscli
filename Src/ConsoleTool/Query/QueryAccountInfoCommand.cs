using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Query;

public class QueryAccountInfoCommand : ICommand
{
    public string StakeAddress { get; init; } = string.Empty;
    public string? Network { get; init; } = "testnet";

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, networkType, errors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors));
        }

        var accountClient = BackendGateway.GetBackendClient<IAccountClient>(networkType);
        try
        {
            var accountInfo = await accountClient.GetStakeInformation(StakeAddress).ConfigureAwait(false);
            var json = JsonSerializer.Serialize(accountInfo.First(), SerialiserOptions);
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
        if (string.IsNullOrWhiteSpace(StakeAddress))
        {
            validationErrors.Add(
                $"Invalid option --stake-address is required");
        }
        if (!Bech32.IsValid(StakeAddress))
        {
            validationErrors.Add(
                $"Invalid option --stake-address {StakeAddress} is invalid");
        }
        return (!validationErrors.Any(), networkType, validationErrors);
    }
}
