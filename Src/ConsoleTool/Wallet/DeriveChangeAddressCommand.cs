using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Wallet;

public class DeriveChangeAddressCommand : ICommand
{
    public string? Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public string Passphrase { get; init; } = string.Empty;
    public string? PaymentAddressType { get; init; }
    public int AccountIndex { get; init; } = 0;
    public int AddressIndex { get; init; } = 0;
    public int StakeAccountIndex { get; init; } = 0;
    public int StakeAddressIndex { get; init; } = 0;
    public string? Network { get; init; }

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, wordList, addressType, network, errors) = Validate();
        if (!isValid)
        {
            return ValueTask.FromResult(
                CommandResult.FailureInvalidOptions(string.Join(Environment.NewLine, errors)));
        }

        var mnemonicService = new MnemonicService();
        try
        {
            var rootKey = mnemonicService.Restore(Mnemonic, wordList)
                .GetRootKey(Passphrase);
            var address = GetPaymentAddress(
                addressType, rootKey, new AddressService(), network);
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

    private Address GetPaymentAddress(
        AddressType addressType,
        PrivateKey rootKey,
        IAddressService addressService,
        NetworkType networkType) => addressType switch
        {
            AddressType.Enterprise => addressService.GetEnterpriseAddress(
                rootKey.Derive($"m/1852'/1815'/{AccountIndex}'/1/{AddressIndex}").GetPublicKey(false),
                networkType),
            AddressType.Base => addressService.GetBaseAddress(
                rootKey.Derive($"m/1852'/1815'/{AccountIndex}'/1/{AddressIndex}").GetPublicKey(false),
                rootKey.Derive($"m/1852'/1815'/{StakeAccountIndex}'/2/{StakeAddressIndex}").GetPublicKey(false),
                networkType),
            _ => throw new NotImplementedException($"--payment-address-type not valid for {nameof(DerivePaymentAddressCommand)}")
        };

    private (
        bool isValid, 
        WordLists derivedWordList,
        AddressType derivedAddressType,
        NetworkType derivedNetworkType,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            validationErrors.Add(
                $"Invalid option --recovery-phrase is required");
        }
        var wordCount = Mnemonic?.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length;
        if (wordCount.HasValue && wordCount > 0 && !ValidMnemonicSizes.Contains(wordCount.Value))
        {
            validationErrors.Add(
                $"Invalid option --recovery-phrase must have the following word count ({string.Join(", ", ValidMnemonicSizes)})");
        }
        if (!Enum.TryParse<WordLists>(Language, ignoreCase: true, out var wordlist))
        {
            validationErrors.Add(
                $"Invalid option --language {Language} is not supported");
        }
        if (!Enum.TryParse<AddressType>(PaymentAddressType, ignoreCase: true, out var paymentAddressType)
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
        if (!Enum.TryParse<NetworkType>(Network, ignoreCase: true, out var networkType))
        {
            validationErrors.Add(
                $"Invalid option --network must be either testnet or mainnet");
        }
        return (!validationErrors.Any(), wordlist, paymentAddressType, networkType, validationErrors);
    }
}
