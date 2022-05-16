using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Query;

public class QueryAddressInfoCommand : ICommand
{
    public string? Address { get; init; }
    public string? Network { get; init; } = "testnet";

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, networkType, errors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors));
        }

        var addressClient = BackendGateway.GetBackendClient<IAddressClient>(networkType);
        try
        {
            var addressInfo = await addressClient.GetAddressInformation(Address).ConfigureAwait(false);
            var json = JsonSerializer.Serialize(addressInfo.First(), SerialiserOptions);
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
        if (string.IsNullOrWhiteSpace(Address))
        {
            validationErrors.Add(
                $"Invalid option --address is required");
        }
        if (!Bech32.IsValid(Address))
        {
            validationErrors.Add(
                $"Invalid option --address {Address} is invalid");
        }
        return (!validationErrors.Any(), networkType, validationErrors);
    }
}
