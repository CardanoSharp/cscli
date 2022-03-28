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
A cross-platform tool for generating Cardano keys, addresses and transactions.

USAGE: cscli (OPTION | COMMAND)

Available options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Available commands:
    wallethd mnemonic generate --size <size> [--language <language>]
    wallethd key root derive --mnemonic ""<string>"" [--language <language>] [--passphrase ""<string>""]
    wallethd key payment derive --mnemonic ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallethd key stake derive --mnemonic ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallethd address payment derive --mnemonic ""<string>""  --network-type <network-type> --payment-address-type <payment-address-type> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>]
    wallethd address stake derive --mnemonic ""<string>"" --network-type <network-type> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>]

Arguments:
    <size> ::= 9 | 12 | 15 | 18 | 21 | 24(Default)
    <language> ::= English(Default)|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese
    <derivation-index> ::= 0(Default) | 1 | .. | 2147483647
    <network-type> ::= Testnet | Mainnet
    <payment-address-type> ::= Enterprise | Base
";
        return ValueTask.FromResult(CommandResult.Success(helpText));
    }
}
