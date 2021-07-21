using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cscli.ConsoleTool
{
    public class ShowVersionCommand : ICommand
    {
        public ValueTask<CommandResult> ExecuteAsync(
            CancellationToken ct)
        {
            var versionString = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException())
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            var versionText = $"cscli {versionString}";
            return ValueTask.FromResult(CommandResult.Success(versionText));
        }
    }
}
