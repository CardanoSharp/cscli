using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cscli.ConsoleTool
{
    public class ShowVersionCommand : ICommand
    {
        public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
        {
            var cardanoSharpVersion = Assembly.GetAssembly(typeof(CardanoSharp.Wallet.KeyService)).GetName().Version.ToString();
            var cscliVersionString = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException())
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            var versionText = $"cscli {cscliVersionString} | CardanoSharp.Wallet {cardanoSharpVersion}";
            return ValueTask.FromResult(CommandResult.Success(versionText));
        }
    }
}
