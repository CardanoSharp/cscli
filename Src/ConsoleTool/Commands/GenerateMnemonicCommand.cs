using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class GenerateMnemonicCommand : ICommand
{
    public int Size { get; init; } = DefaultMnemonicCount;
    public string Language { get; init; } = DefaultMnemonicLanguage;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (!ValidMnemonicSizes.Contains(Size))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --size {Size} is not supported"));
        }
        if (!Enum.TryParse<WordLists>(Language, ignoreCase: true, out var wordlist))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported"));
        }

        var mnemonicService = new MnemonicService();
        try
        {
            var mnemonic = mnemonicService.Generate(Size, wordlist);
            var wordsResult = CommandResult.Success(mnemonic.Words);
            return ValueTask.FromResult(wordsResult);
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }
}
