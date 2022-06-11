using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Wallet;

public class DeriveAccountKeyCommand : ICommand
{
    public string? Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public string Passphrase { get; init; } = string.Empty;
    public int AccountIndex { get; init; } = 0;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --recovery-phrase is required"));
        }
        if (!Enum.TryParse<WordLists>(Language, ignoreCase: true, out var wordlist))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported"));
        }
        var wordCount = Mnemonic.Split(' ', StringSplitOptions.TrimEntries).Length;
        if (!ValidMnemonicSizes.Contains(wordCount))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --recovery-phrase must have the following word count ({string.Join(", ", ValidMnemonicSizes)})"));
        }
        if (AccountIndex < 0 || AccountIndex > MaxDerivationPathIndex)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --account-index must be between 0 and {MaxDerivationPathIndex}"));
        }

        var mnemonicService = new MnemonicService();
        try
        {
            var rootPrvKey = mnemonicService.Restore(Mnemonic, wordlist)
                .GetRootKey(Passphrase);
            var accountSkey = rootPrvKey.Derive($"m/1852'/1815'/{AccountIndex}'");
            var accountVkey = accountSkey.GetPublicKey(false);
            var accountSkeyExtendedBytes = accountSkey.BuildExtendedSkeyBytes();
            var bech32AccountkeyExtended = Bech32.Encode(accountSkeyExtendedBytes, AccountExtendedSigningKeyBech32Prefix);
            var result = CommandResult.Success(bech32AccountkeyExtended);
            return ValueTask.FromResult(result);
        }
        catch (ArgumentException ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureInvalidOptions(ex.Message));
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }
}
