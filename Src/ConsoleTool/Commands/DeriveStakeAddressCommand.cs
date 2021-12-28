using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class DeriveStakeAddressCommand : ICommand
{
    public string? Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public string Passphrase { get; init; } = string.Empty;
    public int AccountIndex { get; init; } = 0;
    public int AddressIndex { get; init; } = 0;
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
            string stakePath = $"m/1852'/1815'/{AccountIndex}'/2/{AddressIndex}";
            var stakeSkey = rootPrvKey.Derive(stakePath);
            var stakeVkey = stakeSkey.GetPublicKey(false);
            var stakeAddr = addressService.GetAddress(
                paymentVkey,
                stakeVkey,
                NetworkTag == "Mainnet" ? NetworkType.Mainnet : NetworkType.Testnet,
                AddressType.Reward);

            var result = CommandResult.Success(stakeAddr.ToString());
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
}
