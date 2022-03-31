namespace Cscli.ConsoleTool;

public interface ICommand
{
    ValueTask<CommandResult> ExecuteAsync(CancellationToken ct);
}
