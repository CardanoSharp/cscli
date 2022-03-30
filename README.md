# cscli

## Goals
A cross-platform CLI for building and interacting with [Cardano's](https://developers.cardano.org/) primitives (i.e. keys, addresses and transactions) 
using .NET native types built on top of [CardanoSharp](https://github.com/CardanoSharp/cardanosharp-wallet).

### Advantages
 - Easy recovery-phrase (aka mnemonic) based key and address derivation for [Hierarchical Deterministic (HD) wallets](https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki), perfect for cold key/address/tx management and storage
 - [Passphrase](https://vault12.com/securemycrypto/crypto-security-basics/what-is-a-passphrase/passphrases-increase-your-protection-and-your-risk) support for additional root key security
 - International multi-language support for [recovery-phrases](https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki)
 - Generates compatible outputs for `cardano-cli` and `cardano-addresses` without any additional dependencies

## Usage

### Overview and Help
```bash
cscli --help

> cscli v0.0.3
> A cross-platform tool to generate and manage Cardano recovery-phrases, keys, addresses and transactions.
> 
> USAGE: cscli (OPTION | COMMAND)
> 
> Available options:
>     -v, --version   Show the cscli version
>     -h, --help      Show this help text
> 
> Available commands:
>     wallet recovery-phrase generate --size <size> [--language <language>]
>     wallet key root derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"]
>     wallet key payment derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
>     wallet key stake derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
>     wallet address payment derive --recovery-phrase "<string>"  --network-type <network-type> --payment-address-type <payment-address-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--stake-account-index <derivation-index>] [--stake-address-index <derivation-index>]
>     wallet address stake derive --recovery-phrase "<string>" --network-type <network-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>]
> 
> Arguments:
>     <size> ::= 9 | 12 | 15 | 18 | 21 | 24(Default)
>     <language> ::= English(Default)|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese
>     <derivation-index> ::= 0(Default) | 1 | .. | 2147483647
>     <network-type> ::= Testnet | Mainnet
>     <payment-address-type> ::= Enterprise | Base
```

📝❗Note: Recovery-phrases and keys should only be generated and stored in airgapped machines if used for real world transactions

### Generate Mnemonics
```bash
cscli wallet recovery-phrase generate | tee phrase.prv

>more enjoy seminar food bench online render dry essence indoor crazy page eight fragile mango zoo burger exhibit crouch drop rocket property alter uphold
```

### Derive Root Key
```bash
cscli wallet key root derive --recovery-phrase "$(cat phrase.prv)" | tee root.xsk

>root_xsk12qpr53a6r7dpjpu2mr6zh96vp4whx2td4zccmplq3am6ph6z4dga6td8nph4qpcnlkdcjkd96p83t23mplvh2w42n6yc3urav8qgph3d9az6lc0px7xq7sau4r4dsfp9h0syfkhge8e6muhd69vz9j6fggdhgd4e
```

### Derive Payment Key
```bash
cscli wallet key payment derive --recovery-phrase "$(cat phrase.prv)" | tee pay00.xsk

>addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw
```

### Derive Stake Key
```bash
cscli wallet key stake derive --recovery-phrase "$(cat phrase.prv)" | tee stake00.xsk

>stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g
```

### Derive Stake/Reward Address
```bash
cscli wallet address stake derive --recovery-phrase "$(cat phrase.prv)" --network-tag Mainnet 

>stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj
```

### Derive Payment Enterprise Address
```bash
cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type Enterprise --network-tag Mainnet | tee pay00.addr

>addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w
```

### Derive Payment Base Address
```bash
cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type Base --network-tag Mainnet | tee pay0000.addr

>addr1qy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqzupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdspma4ht
```

## Installation

### Pre-requisites
The [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) is required to install the global tool 
or to build from the source directly.

### .NET Global Tool

```bash
dotnet install --global cscli
cscli --help
```

### From Source
Building, testing and running compiled binary
```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet publish Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release --nologo
.\release\CsCli.ConsoleTool.exe
```

Directly building and running with `dotnet run`
```bash
cd Src/ConsoleTool
dotnet run --version
```

Build, test and install the global tool based on local source
```
dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet pack --no-build Src/ConsoleTool/CsCli.ConsoleTool.csproj -o nupkg -c Release
dotnet tool install --global --add-source ./nupkg cscli --version 0.0.4
```

## Contributing
Please see [CONTRIBUTING.md](./CONTRIBUTING.md)
