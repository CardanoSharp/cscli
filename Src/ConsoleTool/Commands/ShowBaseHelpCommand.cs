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
A cross-platform tool to generate and manage Cardano recovery-phrases, keys, addresses and transactions.

USAGE: cscli (OPTION | COMMAND)

Available options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Available commands:
    wallet mnemonic generate --size <size> [--language <language>]
    wallet key root derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""]
    wallet key payment derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet key stake derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet address payment derive --recovery-phrase ""<string>""  --network-type <network-type> --payment-address-type <payment-address-type> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>]
    wallet address stake derive --recovery-phrase ""<string>"" --network-type <network-type> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>]

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
