using CardanoSharp.Wallet;
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
        private static int[] ValidSizes = { 9, 12, 15, 18, 21, 24 };
        private static string[] ValidLanguages = { "English" };

        //public GenerateMnemonicCommand(
        //    int size = DefaultSize, string language = DefaultLanguage)
        //{
        //    Size = size;
        //    Language = language;
        //}

        public int Size { get; init; } = DefaultSize;

        public string Language { get; init; } = DefaultLanguage;

        public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
        {
            if (!ValidSizes.Contains(Size))
            {
                var invalidOptionsResult = CommandResult.FailureInvalidOptions($"Invalid option --size {Size} is not supported");
                return ValueTask.FromResult(invalidOptionsResult);
            }
            if (!ValidLanguages.Contains(Language))
            {
                var invalidOptionsResult = CommandResult.FailureInvalidOptions($"Invalid option --language {Language} is not supported");
                return ValueTask.FromResult(invalidOptionsResult);
            }

            var keyService = new KeyService();
            var result = CommandResult.Success(keyService.Generate(Size));
            return ValueTask.FromResult(result);
        }
    }
}
