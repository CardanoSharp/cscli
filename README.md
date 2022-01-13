# cscli

## Goals
A cross-platform CLI for building and interacting with
[Cardano](https://developers.cardano.org/) data structures using 
.NET native cryptographic and serialisation primitives from 
[CardanoSharp](https://github.com/CardanoSharp/cardanosharp-wallet) 
(i.e. not just a wrapper over cardano-cli).

### Advantages
 - Multi-language support for [mnemonic phrases](https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki)
 - Easy [Hierarchical Deterministic (HD) wallet keys and addresses](https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki) with optional passphrases
 - No dependencies on `cardano-cli` or `cardano-node` but supporting compatible outputs
 - User-friendly commands with cardano-centric operations

## Usage

```bash
cscli --help

> cscli v0.0.4
> A cross-platform tool / console app for generating Cardano keys, addresses and transactions.
> 
> USAGE: cscli (OPTION | COMMAND)
> 
> Available options:
>     -v, --version   Show the cscli version
>     -h, --help      Show this help text
> 
> Available commands:
>     wallethd mnemonic gen --size <size> [--language <language>]
>     wallethd key root derive --mnemonic "<string>" [--language <language>] [--passphrase "<string>"]
>     wallethd key payment derive --mnemonic "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
>     wallethd key stake derive --mnemonic "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
>     wallethd address payment derive --mnemonic "<string>"  --network-type <network-type> --payment-address-type <payment-address-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--stake-account-index <derivation-index>] [--stake-address-index <derivation-index>]
>     wallethd address stake derive --mnemonic "<string>" --network-type <network-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>]
> 
> Arguments:
>     <size> ::= 9 | 12 | 15 | 18 | 21 | 24(Default)
>     <language> ::= English(Default)|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese
>     <derivation-index> ::= 0(Default) | 1 | .. | 2147483647
>     <network-type> ::= Testnet | Mainnet
>     <payment-address-type> ::= Enterprise | Base
```

### Generate Mnemonics
```bash
cscli wallethd mnemonic gen | tee phrase.prv

>more enjoy seminar food bench online render dry essence indoor crazy page eight fragile mango zoo burger exhibit crouch drop rocket property alter uphold
```

### Derive Root Key
```bash
cscli wallethd key root derive --mnemonic "$(cat .\phrase.prv)" | tee root.xsk

>root_xsk12qpr53a6r7dpjpu2mr6zh96vp4whx2td4zccmplq3am6ph6z4dga6td8nph4qpcnlkdcjkd96p83t23mplvh2w42n6yc3urav8qgph3d9az6lc0px7xq7sau4r4dsfp9h0syfkhge8e6muhd69vz9j6fggdhgd4e
```

### Derive Payment Key
```bash
cscli wallethd key payment derive --mnemonic "$(cat .\phrase.prv)" | tee pay00.xsk

>addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw
```

### Derive Payment Address
```bash
cscli wallethd address payment derive --mnemonic "$(cat .\phrase.prv)" --payment-address-type Enterprise --network-tag Mainnet | tee pay00.addr

>addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w
```

### Derive Stake Key
```bash
cscli wallethd key stake derive --mnemonic "$(cat .\phrase.prv)" | tee stake00.xsk

>stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g
```

### Derive Stake/Reward Address
```bash
cscli wallethd address stake derive --mnemonic "$(cat .\phrase.prv)" --network-tag Mainnet 

>stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj
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
Building and running compiled binary
```bash
cd Src/ConsoleTool
dotnet build -c Release
cd Bin/Release/net6.0
./Cscli.ConsoleTool.exe --help
```
Directly running with `dotnet run`
```bash
cd Src/ConsoleTool
dotnet run --version
```

## Contributing
Please see [CONTRIBUTING.md](./CONTRIBUTING.md)