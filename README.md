# cscli

## Goals
A cross-platform CLI for building and interacting with [Cardano](https://developers.cardano.org/) wallet primitives (i.e. recovery-phrases, keys, addresses and transactions) 
using .NET native types built on top of [CardanoSharp](https://github.com/CardanoSharp/cardanosharp-wallet).

### Advantages
 - Easy recovery-phrase (aka mnemonic) based key and address derivation for [Hierarchical Deterministic (HD) wallets](https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki), perfect for cold key/address/tx management and storage
 - [Passphrase](https://vault12.com/securemycrypto/crypto-security-basics/what-is-a-passphrase/passphrases-increase-your-protection-and-your-risk) support for additional root key security
 - International multi-language support for [recovery-phrases](https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki)
 - Generates compatible outputs for `cardano-cli` and `cardano-addresses` without any additional dependencies

## Usage

📝❗Note: Recovery-phrases and keys should only be generated and stored in airgapped machines when used in real world transactions.

### Overview and Help
```bash
cscli --help

> cscli v0.0.3
> A cross-platform tool for building and interacting with Cardano wallet primitives (i.e. recovery-phrases, keys, addresses and transactions).
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
>     wallet key stake derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
>     wallet key payment derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
>     wallet address stake derive --recovery-phrase "<string>" --network-type <network-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>]
>     wallet address payment derive --recovery-phrase "<string>"  --network-type <network-type> --payment-address-type <payment-address-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--stake-account-index <derivation-index>] [--stake-address-index <derivation-index>]
> 
> Arguments:
>     <size> ::= 9 | 12 | 15 | 18 | 21 | 24(default)
>     <language> ::= english(default)|chinesesimplified|chinesetraditional|french|italian|japanese|korean|spanish|czech|portuguese
>     <derivation-index> ::= 0(default) | 1 | .. | 2147483647
>     <network-type> ::= testnet | mainnet
>     <payment-address-type> ::= enterprise | base
```

### Generate Recovery Phrase
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
cscli wallet key payment derive --recovery-phrase "$(cat phrase.prv)" | tee pay_0_0.xsk

>addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw
```

#### Payment Key with Custom Indexes
```bash
cscli wallet key payment derive --recovery-phrase "$(cat phrase.prv)" --account-index 569 --address-index 6949 | tee pay_569_6949.xsk

>addr_xsk1kzjky39hv28q30qecg46f3cag3nwsjnnvn5uf0jtkrsxau2z4dgssyrv8jfwdh6frfkd0hskhszcf98xskje0c6ttcnz7k2cwdmc62uv7k6w7nwdcngkwn0semehjsdaajlv2nr5c0rg077dnsyjwxm05vhkuqet
```

### Derive Stake Key
```bash
cscli wallet key stake derive --recovery-phrase "$(cat phrase.prv)" | tee stake_0_0.xsk4

>stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g
```

#### Stake Key with Custom Indexes
```bash
cscli wallet key stake derive --recovery-phrase "$(cat phrase.prv)" --account-index 968 --address-index 83106 | tee stake_968_83106.xsk

>stake_xsk14p0lhj3txvfcj8j08dk3ur954hmcfz6u6t00q0a3vnrsd7zz4dgcy9dwcxgf67v4rdp4mk9tkeqw70y4m7va73thnel7jwyx0achc5tyyx8r2au5x3pw37zhznj03v2cajc96paltxlh8hpefssucyecus24q26n
```

### Derive Stake/Reward Address
```bash
cscli wallet address stake derive --recovery-phrase "$(cat phrase.prv)" --network-tag mainnet | tee stake_0_0.addr

>stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj
```

#### Stake Address with Custom Indexes
```bash
cscli wallet address stake derive --recovery-phrase "$(cat phrase.prv)" --network-tag mainnet --account-index 1 --address-index 7 | tee stake_1_7.addr

>stake1u87phtdn9shvp39c44elyfdduuqg7wz072vs0vjvc20hvaqym7xan
```

### Derive Payment Enterprise Address
```bash
cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type enterprise --network-tag mainnet | tee pay_0_0.addr

>addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w
```

#### Payment Enterprise Address with Custom Indexes
```bash
cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type enterprise --network-tag mainnet --account-index 1387 --address-index 12 | tee pay_1387_12.addr

>addr1vy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6xcc8pzd9
```

### Derive Payment Base Address
```bash
cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type base --network-tag mainnet | tee pay_0_0_0_0.addr

>addr1qy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqzupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdspma4ht
```

#### Payment Base Address with Custom Indexes
```bash
cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type base --network-tag mainnet --account-index 1387 --address-index 12 --stake-account-index 968 --stake-address-index 83106 | tee pay_1387_12_968_83106.addr

>addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h
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
dotnet build --no-restore -c Release
dotnet test --no-build -c Release
dotnet publish --no-build Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release --nologo 
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
dotnet tool install --global --add-source ./nupkg cscli --version 0.0.4-local-branch.1
```

## Contributing
Please see [CONTRIBUTING.md](./CONTRIBUTING.md)
