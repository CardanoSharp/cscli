using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Addresses;
using Cscli.ConsoleTool.Koios;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Query;

public class QueryAccountInfoCommand : ICommand
{
    public string StakeAddress { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string? Network { get; init; }

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, networkType, stakeAddress, errors) = Validate();
        if (!isValid || stakeAddress is null)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors));
        }

        var accountClient = BackendGateway.GetBackendClient<IAccountClient>(networkType);
        try
        {
            var accountInfo = await accountClient.GetStakeInformation(stakeAddress.ToString()).ConfigureAwait(false);
            if (!accountInfo.IsSuccessStatusCode || accountInfo.Content is null)
                return CommandResult.FailureBackend($"Koios backend response was unsuccessful");

            var json = JsonSerializer.Serialize(accountInfo.Content, SerialiserOptions);
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
        Address? derivedStakeAddress,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            validationErrors.Add(
                $"Invalid option --network must be either testnet or mainnet");
        }
        var stakeAddressArgumentExists = !string.IsNullOrWhiteSpace(StakeAddress);
        var paymentAddressArgumentExists = !string.IsNullOrWhiteSpace(Address);
        if (!stakeAddressArgumentExists && !paymentAddressArgumentExists
            || stakeAddressArgumentExists && paymentAddressArgumentExists)
        {
            validationErrors.Add(
                "Invalid option, one of either --stake-address or --address is required");
            return (!validationErrors.Any(), networkType, null, validationErrors);
        }
        if (stakeAddressArgumentExists)
        {
            if (!Bech32.IsValid(StakeAddress))
            {
                validationErrors.Add(
                    $"Invalid option --stake-address {StakeAddress} is invalid");
            }
            else
            {
                var stakeAddress = new Address(StakeAddress);
                if (stakeAddress.AddressType != AddressType.Reward || stakeAddress.NetworkType != networkType)
                {
                    validationErrors.Add(
                        $"Invalid option --stake-address {StakeAddress} is invalid for {Network}");
                }
                else
                {
                    return (!validationErrors.Any(), networkType, stakeAddress, validationErrors);
                }
            }
        }
        if (paymentAddressArgumentExists)
        {
            if (!Bech32.IsValid(Address))
            {
                validationErrors.Add(
                    $"Invalid option --address {Address} is not a base address with attached staking credentials");
            }
            else
            {
                var addressService = new AddressService();
                var address = new Address(Address);
                if (address.AddressType != AddressType.Base || address.NetworkType != networkType)
                {
                    validationErrors.Add(
                        $"Invalid option --address {Address} is not a base address with attached staking credentials for {Network}");
                }
                else
                {
                    var stakeAddress = addressService.ExtractRewardAddress(address);
                    return (!validationErrors.Any(), networkType, stakeAddress, validationErrors);
                }
            }
        }
        return (!validationErrors.Any(), networkType, null, validationErrors);
    }
}
