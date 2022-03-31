using System.Reflection;

namespace Cscli.ConsoleTool.Commands;

public class ShowVersionCommand : ICommand
{
    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var cardanoSharpVersion = Assembly.GetAssembly(typeof(CardanoSharp.Wallet.MnemonicService))?.GetName().Version?.ToString();
        var cscliVersionString = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException())
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        var versionText = $"cscli {cscliVersionString} | CardanoSharp.Wallet {cardanoSharpVersion}";
        return ValueTask.FromResult(CommandResult.Success(versionText));
    }
}
