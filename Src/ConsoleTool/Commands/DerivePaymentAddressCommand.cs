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
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic is required"));
        }
        if (!Enum.TryParse<WordLists>(Language, out var wordlist))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported"));
        }
        var wordCount = Mnemonic.Split(' ', StringSplitOptions.TrimEntries).Length;
        if (!ValidMnemonicSizes.Contains(wordCount))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic must have the following word count ({string.Join(", ", ValidMnemonicSizes)})"));
        }
        if (PaymentAddressType != "Base" && PaymentAddressType != "Enterprise")
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --payment-address-type {PaymentAddressType} is not supported"));
        }
        if (AccountIndex < 0 || AccountIndex > MaxDerivationPathIndex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --account-index must be between 0 and {MaxDerivationPathIndex}"));
        }
        if (AddressIndex < 0 || AddressIndex > MaxDerivationPathIndex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --address-index must be between 0 and {MaxDerivationPathIndex}"));
        }
        if (StakeAccountIndex < 0 || StakeAccountIndex > MaxDerivationPathIndex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --stake-account-index must be between 0 and {MaxDerivationPathIndex}"));
        }
        if (StakeAddressIndex < 0 || StakeAddressIndex > MaxDerivationPathIndex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --stake-address-index must be between 0 and {MaxDerivationPathIndex}"));
        }
        if (NetworkTag != "Testnet" && NetworkTag != "Mainnet")
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --network-tag must be either Testnet or Mainnet"));
        }

        try
        {
            var mnemonicService = new MnemonicService();
            var addressService = new AddressService();

            var rootPrvKey = mnemonicService.Restore(Mnemonic, wordlist).GetRootKey(Passphrase);
            var paymentPath = $"m/1852'/1815'/{AccountIndex}'/0/{AddressIndex}";
            var paymentSkey = rootPrvKey.Derive(paymentPath);
            var paymentVkey = paymentSkey.GetPublicKey(false);

            var networkType = NetworkTag == "Mainnet" ? NetworkType.Mainnet : NetworkType.Testnet;
            _ = Enum.TryParse<AddressType>(PaymentAddressType, out var addressType);
            var stakeVkey = TryDeriveStakeVKey(addressType, rootPrvKey);
            var address = addressService.GetAddress(
                paymentVkey,
                stakeVkey,
                networkType,
                addressType);

            var result = CommandResult.Success(address.ToString());
            return ValueTask.FromResult(result);
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

    private PublicKey? TryDeriveStakeVKey(AddressType addressType, PrivateKey rootPrvKey) =>
        addressType switch
        {
            // Derive stake public key based on stake derivation path
            AddressType.Base => rootPrvKey
                .Derive($"m/1852'/1815'/{StakeAccountIndex}'/2/{StakeAddressIndex}").GetPublicKey(false),
            _ => null // Enterprise (and everything else)
        };
}
