using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cscli.ConsoleTool
{
    public class GenerateMnemonicCommand : ICommand
    {
        public const int DefaultSize = 24;
        public const string DefaultLanguage = "English";
        public static int[] ValidSizes = { 9, 12, 15, 18, 21, 24 };

        public int Size { get; init; } = DefaultSize;

        public string Language { get; init; } = DefaultLanguage;

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

            var keyService = new KeyService();
            var result = CommandResult.Success(keyService.Generate(Size, wordlist));
            return ValueTask.FromResult(result);
        }
    }
}
