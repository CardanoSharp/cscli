using System.Reflection;

namespace Cscli.ConsoleTool.Commands;

public class ShowBaseHelpCommand : ICommand
{
    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var versionString = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException())
            .GetCustomAttribute<AssemblyFileVersionAttribute>()?
            .Version;
        var helpText = $@"cscli v{versionString}
A cross-platform tool for building and interacting with Cardano's wallet primitives (i.e. recovery-phrases, keys, addresses and transactions).

USAGE: cscli (OPTION | COMMAND)

Available options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Available commands:
    wallet recovery-phrase generate --size <size> [--language <language>]
    wallet key root derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""]
    wallet key stake derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet key payment derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet address stake derive --recovery-phrase ""<string>"" --network-type <network-type> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>]
    wallet address payment derive --recovery-phrase ""<string>""  --network-type <network-type> --payment-address-type <payment-address-type> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--stake-account-index <derivation-index>] [--stake-address-index <derivation-index>]

Arguments:
    <size> ::= 9 | 12 | 15 | 18 | 21 | 24(default)
    <language> ::= english(default)|chinesesimplified|chinesetraditional|french|italian|japanese|korean|spanish|czech|portuguese
    <derivation-index> ::= 0(default) | 1 | .. | 2147483647
    <network-type> ::= testnet | mainnet
    <payment-address-type> ::= enterprise | base
";
        return ValueTask.FromResult(CommandResult.Success(helpText));
    }
}
