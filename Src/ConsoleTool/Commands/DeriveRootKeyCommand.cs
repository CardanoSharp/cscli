using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class DeriveRootKeyCommand : ICommand
{
    public string Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;

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

        try
        {
            var mnemonicService = new MnemonicService();
            var mnemonic = mnemonicService.Restore(Mnemonic, wordlist);
            var rootPrvKey = mnemonic.GetRootKey();
            //var rootPubKey = rootPrvKey.GetPublicKey();
            var bech32FormattedRootPrvKey = Bech32.Encode(rootPrvKey.Key, RootKeyBech32Prefix);
            var result = CommandResult.Success(bech32FormattedRootPrvKey);
            return ValueTask.FromResult(result);
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }
}
