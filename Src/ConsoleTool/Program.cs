namespace Cscli.ConsoleTool;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var cts = SetupUserInputCancellationTokenSource();
        var command = CommandParser.ParseArgsToCommand(args);
        var commandResult = await command.ExecuteAsync(cts.Token).ConfigureAwait(false);
        if (commandResult.Outcome == CommandOutcome.Success)
        {
            Console.Out.WriteLine(commandResult.Result);
            return 0;
        }
        Console.Error.WriteLine(commandResult.Result);
        return (int)commandResult.Outcome;
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
