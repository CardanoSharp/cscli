using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Addresses;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Query;

public class QueryAddressInfoCommand : ICommand
{
    public string? Address { get; init; }
    public string? Network { get; init; }

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
#pragma warning disable CS8604 // Possible null reference warning suppressed because of validation above
            var addressInfo = await addressClient.GetAddressInformation(Address).ConfigureAwait(false);
#pragma warning restore CS8604 
            var json = JsonSerializer.Serialize(addressInfo, SerialiserOptions);
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
        var address = new Address(Address);
        if (!Bech32.IsValid(Address) || address.NetworkType != networkType)
        {
            validationErrors.Add(
                $"Invalid option --address {Address} is invalid for {Network}");
        }
        return (!validationErrors.Any(), networkType, validationErrors);
    }
}
