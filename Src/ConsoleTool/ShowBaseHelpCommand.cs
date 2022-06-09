using System.Reflection;

namespace Cscli.ConsoleTool;

public class ShowBaseHelpCommand : ICommand
{
    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var versionString = (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException())
            .GetCustomAttribute<AssemblyFileVersionAttribute>()?
            .Version;
        var helpText = $@"cscli v{versionString}
A lightweight cross-platform tool for generating/serialising Cardano wallet primitives (i.e. recovery-phrases, keys, addresses and transactions), querying the chain and submitting transactions to the testnet or mainnet networks. Please refer to https://github.com/CardanoSharp/cscli for further documentation.

USAGE: cscli (OPTION | COMMAND)

Options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Wallet commands:
    wallet recovery-phrase generate --size <size> [--language <language>]
    wallet key root derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""]
    wallet key account derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <index>]
    wallet key stake derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet key payment derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet key policy derive --recovery-phrase ""<string>"" [--language <language>] [--passphrase ""<string>""] [--policy-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet key verification convert --signing-key ""<bech32_skey>"" [--verification-key-file <string>]
    wallet address stake derive --recovery-phrase ""<string>"" --network <network> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>]
    wallet address payment derive --recovery-phrase ""<string>""  --network <network> --payment-address-type <payment-address-type> [--language <language>] [--passphrase ""<string>""] [--account-index <derivation-index>] [--address-index <derivation-index>] [--stake-account-index <derivation-index>] [--stake-address-index <derivation-index>]

Query commands:
    query tip --network <network>
    query protocol-parameters --network <network>
    query info account --network <network> [--stake-address <stake_address> | --address <payment_base_address>]
    query asset account --network <network> --stake-address <stake_address>
    query info address --network <network> --address <payment_address>
    query info transaction --network <network> --tx-id <transaction_id>

Transaction Commands:
    transaction submit --network <network> --cbor-hex <hex_string>

Encoding/Crypto Commands:
    bech32 encode --value <hex_string> --prefix <string>
    bech32 decode --value <bech32_string>
    blake2b hash --value <hex_string> --length <digest_length> 

Arguments:
    <size> ::= 9 | 12 | 15 | 18 | 21 | 24(default)
    <language> ::= english(default)|chinesesimplified|chinesetraditional|french|italian|japanese|korean|spanish|czech|portuguese
    <derivation-index> ::= 0(default) | 1 | .. | 2147483647
    <network> ::= testnet | mainnet
    <payment-address-type> ::= enterprise | base
    <digest_length> ::= 160 | 224 | 256 | 512
";
        return ValueTask.FromResult(CommandResult.Success(helpText));
    }
}
