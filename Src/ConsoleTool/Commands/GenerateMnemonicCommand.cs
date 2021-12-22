using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class GenerateMnemonicCommand : ICommand
{
    public static int[] ValidSizes = { 9, 12, 15, 18, 21, 24 };

    public int Size { get; init; } = DefaultMnemonicCount;
    public string Language { get; init; } = DefaultMnemonicLanguage;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (!ValidSizes.Contains(Size))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --size {Size} is not supported"));
        }
        if (!Enum.TryParse<WordLists>(Language, out var wordlist))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported"));
        }

        try
        {
            var mnemonicService = new MnemonicService();
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
