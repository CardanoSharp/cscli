using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cscli.ConsoleTool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var cts = SetupUserInputCancellationTokenSource();

            var command = CommandParser.ParseArgsToCommand(args);
            var commandResult = await command.ExecuteAsync(cts.Token).ConfigureAwait(false);
            if (commandResult.Outcome == CommandOutcome.Success)
            {
                Console.Out.WriteLine(commandResult.Result);
            }
            else
            {
                Console.Error.WriteLine(commandResult.Result);
                return 1;
            }
            return 0;
        }

        private static CancellationTokenSource SetupUserInputCancellationTokenSource()
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };
            return cts;
        }
    }
}
