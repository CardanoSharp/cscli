using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class DerivePaymentAddressCommand : ICommand
{
    public string? Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public string Passphrase { get; init; } = string.Empty;
    public string? PaymentAddressType { get; init; }
    public int AccountIndex { get; init; } = 0;
    public int AddressIndex { get; init; } = 0;
    public int StakeAccountIndex { get; init; } = 0;
    public int StakeAddressIndex { get; init; } = 0;
    public string NetworkTag { get; init; } = "testnet";

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, derivedWorldList, addressType, network, errors) = Validate();
        if (!isValid)
        {
            return ValueTask.FromResult(
                CommandResult.FailureInvalidOptions(string.Join(Environment.NewLine, errors)));
        }
        try
        {
            var mnemonicService = new MnemonicService();
            var addressService = new AddressService();
            var rootKey = mnemonicService.Restore(Mnemonic, derivedWorldList)
                .GetRootKey(Passphrase);
            var paymentVkey = rootKey.Derive($"m/1852'/1815'/{AccountIndex}'/0/{AddressIndex}")
                .GetPublicKey(false);
            var stakeVkey = rootKey.Derive($"m/1852'/1815'/{StakeAccountIndex}'/2/{StakeAddressIndex}")
                .GetPublicKey(false);
            var address = addressService.GetAddress(
                paymentVkey,
                stakeVkey,
                network,
                addressType);
            return ValueTask.FromResult(CommandResult.Success(address.ToString()));
        }
        catch (ArgumentException ex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(ex.Message));
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }

    private (
        bool isValid, 
        WordLists derivedWordList,
        AddressType addressType,
        NetworkType derivedNetworkType,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            validationErrors.Add(
                $"Invalid option --mnemonic is required");
        }
        var wordCount = Mnemonic?.Split(' ', StringSplitOptions.TrimEntries).Length;
        if (wordCount.HasValue && !ValidMnemonicSizes.Contains(wordCount.Value))
        {
            validationErrors.Add(
                $"Invalid option --mnemonic must have the following word count ({string.Join(", ", ValidMnemonicSizes)})");
        }
        if (!Enum.TryParse<WordLists>(Language, out var wordlist))
        {
            validationErrors.Add(
                $"Invalid option --language {Language} is not supported");
        }
        if (!Enum.TryParse<AddressType>(PaymentAddressType, out var paymentAddressType)
            || paymentAddressType != AddressType.Base && paymentAddressType != AddressType.Enterprise)
        {
            validationErrors.Add(
                $"Invalid option --payment-address-type {PaymentAddressType} is not supported");
        }
        if (AccountIndex < 0 || AccountIndex > MaxDerivationPathIndex)
        {
            validationErrors.Add(
                $"Invalid option --account-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (AddressIndex < 0 || AddressIndex > MaxDerivationPathIndex)
        {
            validationErrors.Add(
                $"Invalid option --address-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (StakeAccountIndex < 0 || StakeAccountIndex > MaxDerivationPathIndex)
        {
            validationErrors.Add(
                $"Invalid option --stake-account-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (StakeAddressIndex < 0 || StakeAddressIndex > MaxDerivationPathIndex)
        {
            validationErrors.Add(
                $"Invalid option --stake-address-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (!Enum.TryParse<NetworkType>(NetworkTag, out var networkType))
        {
            validationErrors.Add(
                $"Invalid option --network-tag must be either Testnet or Mainnet");
        }
        return (!validationErrors.Any(), wordlist, paymentAddressType, networkType, validationErrors);
    }
}
