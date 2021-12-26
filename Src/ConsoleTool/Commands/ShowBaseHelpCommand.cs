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
A cross-platform Tool / Console App for generating Cardano keys, addresses and transactions.

USAGE: cscli (OPTION | COMMAND)

Available options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Available commands:
    wallethd mnemonic gen --size (9|12|15|18|21|24) --language (English|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese)
    wallethd key root derive --mnemonic ""$MNEMONIC"" [--passphrase $PASSPHRASE]
    wallethd key payment derive --mnemonic ""$MNEMONIC"" [--passphrase $PASSPHRASE] [--account-index $ACCT_IX] [--address-index $ADDR_IX] [--verification-key-file $VKEYPATH] [--signing-key-file $SKEYPATH]
    wallethd key stake derive --mnemonic ""$MNEMONIC"" [--passphrase $PASSPHRASE] [--account-index $ACCT_IX] [--address-index $ADDR_IX] [--verification-key-file $VKEYPATH] [--signing-key-file $SKEYPATH]
    (WIP) wallethd address payment derive --mnemonic ""$MNEMONIC"" [--account-index ACCT_IX] --address-index ADDR_IX --output-format (HEX|CBOR|BECH32|JSON) (--mainnet | --testnet-magic MAGIC)
    (WIP) wallethd address stake derive --mnemonic ""$MNEMONIC"" [--account-index ACCT_IX] --address-index ADDR_IX --output-format (HEX|CBOR|BECH32|JSON) (--mainnet | --testnet-magic MAGIC)";
        return ValueTask.FromResult(CommandResult.Success(helpText));
    }
}
