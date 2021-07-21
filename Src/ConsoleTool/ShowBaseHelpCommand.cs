using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cscli.ConsoleTool
{
    public class ShowBaseHelpCommand : ICommand
    {
        public ValueTask<CommandResult> ExecuteAsync(
            CancellationToken ct)
        {
            var versionString = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException())
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            var helpText = $@"cscli v{versionString}
A .NET Cross Platform Tool for integrating with Cardano wallet data structures

USAGE: cscli (OPTION | COMMAND)

Available options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Available commands:
    wallethd mnemonic gen --size (9|12|15|18|21|24|27) --language (English)
    wallethd key payment derive --mnemonic MNEMONIC --account-index ACCT_IX --verification-key-file VKEY --signing-key-file SKEY
    wallethd key stake derive --mnemonic MNEMONIC --account-index ACCT_IX --verification-key-file VKEY --signing-key-file SKEY
    wallethd address payment derive --mnemonic MNEMONIC --account-index ACCT_IX
    wallethd address stake derive --mnemonic MNEMONIC --account-index ACCT_IX";

            return ValueTask.FromResult(CommandResult.Success(helpText));
        }
    }
}
