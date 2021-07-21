using System.Threading;
using System.Threading.Tasks;

namespace Cscli.ConsoleTool
{
    public interface ICommand
    {
        ValueTask<CommandResult> ExecuteAsync(CancellationToken ct);
    }
}
