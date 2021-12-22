using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class DerivePaymentAddressCommand : ICommand
{
    public string Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
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
        if (!GenerateMnemonicCommand.ValidSizes.Contains(wordCount))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic must have the following word count ({string.Join(", ", GenerateMnemonicCommand.ValidSizes)})"));
        }
        if (AccountIndex < 0 || AccountIndex > MaxPathIndex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --account-index must be between 0 and {MaxPathIndex}"));
        }
        if (AddressIndex < 0 || AddressIndex > MaxPathIndex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --address-index must be between 0 and {MaxPathIndex}"));
        }
        if (NetworkTag != "testnet" && NetworkTag != "mainnet")
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --network-tag must be either testnet or mainnet"));
        }

        try
        {
            var mnemonicService = new MnemonicService();
            var addressService = new AddressService();

            var mnemonic = mnemonicService.Restore(Mnemonic, wordlist);
            var rootPrvKey = mnemonic.GetRootKey();
            var paymentPath = $"m/1852'/1815'/{AccountIndex}'/0/{AddressIndex}";
            var paymentSkey = rootPrvKey.Derive(paymentPath);
            var paymentVkey = paymentSkey.GetPublicKey(false);
            string stakePath = $"m/1852'/1815'/{AccountIndex}'/2/{AddressIndex}";
            var stakeSkey = rootPrvKey.Derive(stakePath);
            var stakeVkey = stakeSkey.GetPublicKey(false);
            var baseAddr = addressService.GetAddress(
                paymentVkey,
                stakeVkey,
                NetworkTag == "mainnet" ? NetworkType.Mainnet : NetworkType.Testnet,
                AddressType.Base);

            var result = CommandResult.Success(baseAddr.ToString());
            return ValueTask.FromResult(result);
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }
}
