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
 - User-friendly commands for cardano-centric operations

## Usage

```bash
cscli --help

> cscli v0.0.3
> A cross-platform Tool for generating Cardano keys, addresses and transactions.
> 
> USAGE: cscli (OPTION | COMMAND)
> 
> Available options:
>     -v, --version   Show the cscli version
>     -h, --help      Show this help text
> 
> Available commands:
>     wallethd mnemonic gen [--size (9|12|15|18|21|24(Default)) --language (English(Default)|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese)]
>     wallethd key root derive --mnemonic "$MN" [--language (English(Default)|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese) --passphrase "$PASS"]
>     wallethd key payment derive --mnemonic "$MN" [--language (English(Default)|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese) --passphrase "$PASS" --account-index $ACCT_IX --address-index $ADDR_IX --verification-key-file $VKEY --signing-key-file $SKEY]
>     wallethd key stake derive --mnemonic "$MN" [--language (English(Default)|ChineseSimplified|ChineseTraditional|French|Italian|Japanese|Korean|Spanish|Czech|Portuguese) --passphrase "$PASS" --account-index $ACCT_IX --address-index $ADDR_IX --verification-key-file $VKEY --signing-key-file $SKEY]
>     wallethd address payment derive --mnemonic "$MN" [--account-index $ACCT_IX --address-index $ADDR_IX]
>     wallethd address stake derive --mnemonic "$MN" [--account-index $ACCT_IX --address-index $ADDR_IX]
```
### Generate Mnemonics
```bash
cscli wallethd mnemonic gen 
```

### Derive Root Key
```bash
cscli wallethd key root derive --mnemonic "$mnemonic"
```

### Derive Payment Key
```bash
cscli wallethd key payment derive --mnemonic "$mnemonic"
```

### Derive Payment Address
```bash
cscli wallethd address payment derive --mnemonic "$mnemonic"
```

### Derive Stake Key
```bash
cscli wallethd key stake derive --mnemonic "$mnemonic"
```

### Derive Stake/Reward Address
```bash
cscli wallethd address stake derive --mnemonic "$mnemonic"
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