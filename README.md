# cscli

## Goals
A cross-platform CLI for building and interacting with [Cardano](https://developers.cardano.org/) wallet primitives (i.e. recovery-phrases, keys, addresses and transactions) 
using .NET native types built on top of [CardanoSharp](https://github.com/CardanoSharp/cardanosharp-wallet).

### Advantages
 - Easy recovery-phrase (aka mnemonic) based key and address derivation for [Hierarchical Deterministic (HD) wallets](https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki), perfect for cold key/address/tx management and storage
 - [Passphrase](https://vault12.com/securemycrypto/crypto-security-basics/what-is-a-passphrase/passphrases-increase-your-protection-and-your-risk) support for additional root key security
 - International multi-language support for [recovery-phrases](https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki)
 - Generates compatible outputs for `cardano-cli` and `cardano-addresses` without any additional dependencies

## Installation

### Download Cross-Platform Binaries from Latest Release
The easiest option without any dependencies is to download the platform-specific binaries from the [latest release](https://github.com/CardanoSharp/cscli/releases) and run.

### As a .NET global tool or Built from Source
The [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) with the `dotnet` CLI is required to install the global tool 
or to build from the source directly.

<details>
  <summary>.NET Global Tool</summary>

```console
dotnet tool install --global cscli
cscli --help
```
</details>

<details>
  <summary>Build From Source</summary>

Building, testing and running compiled binary
```console
dotnet restore
dotnet build --no-restore -c Release
dotnet test --no-build -c Release
dotnet publish --no-build Src/ConsoleTool/CsCli.ConsoleTool.csproj -c Release -o release --nologo 
.\release\CsCli.ConsoleTool.exe
```

Or directly building and running with `dotnet run`
```console
cd Src/ConsoleTool
dotnet run --version
```

Or build, test and install the global tool based on local source
```
dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet pack --no-build Src/ConsoleTool/CsCli.ConsoleTool.csproj -o nupkg -c Release
dotnet tool install --global --add-source ./nupkg cscli --version 0.0.5-local-branch.1
```
</details>

## Usage

📝❗Note: Recovery-phrases and keys should only be generated and stored in airgapped machines when used in real world transactions.

### Overview and Help
```console
$ cscli --help
cscli v0.0.5
A cross-platform tool for building and interacting with Cardano wallet primitives (i.e. recovery-phrases, keys, addresses and transactions).

USAGE: cscli (OPTION | COMMAND)

Available options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Available commands:
    wallet recovery-phrase generate --size <size> [--language <language>]
    wallet key root derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"]
    wallet key stake derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet key payment derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet key policy derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--policy-index <derivation-index>] [--verification-key-file <string>] [--signing-key-file <string>]
    wallet address stake derive --recovery-phrase "<string>" --network-type <network-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>]
    wallet address payment derive --recovery-phrase "<string>"  --network-type <network-type> --payment-address-type <payment-address-type> [--language <language>] [--passphrase "<string>"] [--account-index <derivation-index>] [--address-index <derivation-index>] [--stake-account-index <derivation-index>] [--stake-address-index <derivation-index>]

Arguments:
    <size> ::= 9 | 12 | 15 | 18 | 21 | 24(default)
    <language> ::= english(default)|chinesesimplified|chinesetraditional|french|italian|japanese|korean|spanish|czech|portuguese
    <derivation-index> ::= 0(default) | 1 | .. | 2147483647
    <network-type> ::= testnet | mainnet
    <payment-address-type> ::= enterprise | base
```

### Generate Recovery Phrase
```console
$ cscli wallet recovery-phrase generate | tee phrase.prv
more enjoy seminar food bench online render dry essence indoor crazy page eight fragile mango zoo burger exhibit crouch drop rocket property alter uphold
```

### Derive Root Key
```console
$ cscli wallet key root derive --recovery-phrase "$(cat phrase.prv)" | tee root.xsk
root_xsk12qpr53a6r7dpjpu2mr6zh96vp4whx2td4zccmplq3am6ph6z4dga6td8nph4qpcnlkdcjkd96p83t23mplvh2w42n6yc3urav8qgph3d9az6lc0px7xq7sau4r4dsfp9h0syfkhge8e6muhd69vz9j6fggdhgd4e
```

### Derive Payment Key
```console
$ cscli wallet key payment derive --recovery-phrase "$(cat phrase.prv)" | tee pay_0_0.xsk
addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw
```
<details>
  <summary>Payment Key with Custom Indexes</summary>

```console
$ cscli wallet key payment derive --recovery-phrase "$(cat phrase.prv)" --account-index 569 --address-index 6949 | tee pay_569_6949.xsk
addr_xsk1kzjky39hv28q30qecg46f3cag3nwsjnnvn5uf0jtkrsxau2z4dgssyrv8jfwdh6frfkd0hskhszcf98xskje0c6ttcnz7k2cwdmc62uv7k6w7nwdcngkwn0semehjsdaajlv2nr5c0rg077dnsyjwxm05vhkuqet
```
</details>
<details>
  <summary>Payment Key with cardano-cli Output Key Files</summary>

```console
$ cscli wallet key payment derive --recovery-phrase "$(cat phrase.prv)" --signing-key-file pay_0_0.skey --verification-key-file pay_0_0.vkey | tee pay_0_0.xsk
addr_xsk1kzjky39hv28q30qecg46f3cag3nwsjnnvn5uf0jtkrsxau2z4dgssyrv8jfwdh6frfkd0hskhszcf98xskje0c6ttcnz7k2cwdmc62uv7k6w7nwdcngkwn0semehjsdaajlv2nr5c0rg077dnsyjwxm05vhkuqet
$ cat pay_0_0.skey
{
  "type": "PaymentExtendedSigningKeyShelley_ed25519_bip32",
  "description": "Payment Signing Key",
  "cborHex": "5880489c51d4ea5bf36e77c3c032e446c79728d28f93acb9ccdf6a0aab25f042ab5157073cfcc17e77ec598bf213dc7dca16c99b25ccdb8a04254b0143443e0a2724de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa2fc8083013ea2bf3a68de08a59346e129d7bd858b290f408d65cbaa97c09d1cf"
}
$ cat pay_0_0.vkey
{
  "type": "PaymentExtendedVerificationKeyShelley_ed25519_bip32",
  "description": "Payment Verification Key",
  "cborHex": "5840de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa2fc8083013ea2bf3a68de08a59346e129d7bd858b290f408d65cbaa97c09d1cf"
}
```
</details>

### Derive Stake Key
```console
$ cscli wallet key stake derive --recovery-phrase "$(cat phrase.prv)" | tee stake_0_0.xsk
stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g
```

<details>
  <summary>Stake Key with Custom Indexes</summary>

```console
$ cscli wallet key stake derive --recovery-phrase "$(cat phrase.prv)" --account-index 968 --address-index 83106 | tee stake_968_83106.xsk
stake_xsk14p0lhj3txvfcj8j08dk3ur954hmcfz6u6t00q0a3vnrsd7zz4dgcy9dwcxgf67v4rdp4mk9tkeqw70y4m7va73thnel7jwyx0achc5tyyx8r2au5x3pw37zhznj03v2cajc96paltxlh8hpefssucyecus24q26n
```
</details>
<details>
  <summary>Stake Key with cardano-cli Output Key Files</summary>

```console
$ cscli wallet key stake derive --recovery-phrase "$(cat phrase.prv)" --signing-key-file stake_0_0.skey --verification-key-file stake_0_0.vkey | tee stake_0_0.xsk
stake_xsk14p0lhj3txvfcj8j08dk3ur954hmcfz6u6t00q0a3vnrsd7zz4dgcy9dwcxgf67v4rdp4mk9tkeqw70y4m7va73thnel7jwyx0achc5tyyx8r2au5x3pw37zhznj03v2cajc96paltxlh8hpefssucyecus24q26n
$ cat stake_0_0.skey
{
  "type": "StakeExtendedSigningKeyShelley_ed25519_bip32",
  "description": "Stake Signing Key",
  "cborHex": "588030e983d551613634b060150ee00161cd907c7d50f099ccb73d843decec42ab5108aa69096b5fc8ce3eafe758d81f16740ce0d83e74330587f079c8a6afbba92f1bd85ec71d2d8ce0180138310983aafffa4585486db1576bc385b0ae350562e6a001925239b5726e3132b8a5598904eaf248b688668450dbd12febe1ebe127ed"
}
$ cat stake_0_0.vkey
{
  "type": "StakeVerificationKeyShelley_ed25519",
  "description": "Stake Verification Key",
  "cborHex": "58201bd85ec71d2d8ce0180138310983aafffa4585486db1576bc385b0ae350562e6"
}
```
</details>

### Derive Stake/Reward Address
```console
$ cscli wallet address stake derive --recovery-phrase "$(cat phrase.prv)" --network-tag mainnet | tee stake_0_0.addr
stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj
```

<details>
  <summary>Stake Address with Custom Indexes</summary>

```console
$ cscli wallet address stake derive --recovery-phrase "$(cat phrase.prv)" --network-tag mainnet --account-index 1 --address-index 7 | tee stake_1_7.addr
stake1u87phtdn9shvp39c44elyfdduuqg7wz072vs0vjvc20hvaqym7xan
```
</details>

### Derive Payment Enterprise Address
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type enterprise --network-tag mainnet | tee pay_0_0.addr
addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w
```

<details>
  <summary>Payment Enterprise Address with Custom Indexes</summary>
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type enterprise --network-tag mainnet --account-index 1387 --address-index 12 | tee pay_1387_12.addr
addr1vy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6xcc8pzd9
```
</details>

### Derive Payment Base Address
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type base --network-tag mainnet | tee pay_0_0_0_0.addr
addr1qy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqzupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdspma4ht
```

<details>
  <summary>Payment Base Address with Custom Indexes</summary>
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.prv)" --payment-address-type base --network-tag mainnet --account-index 1387 --address-index 12 --stake-account-index 968 --stake-address-index 83106 | tee pay_1387_12_968_83106.addr
addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h
```
</details>

### Derive Policy Key
```console
$ cscli wallet key policy derive --recovery-phrase "$(cat phrase.prv)" | tee policy_0.sk
policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02
```
<details>
  <summary>Policy Key with Custom Indexes</summary>

```console
$ cscli wallet key policy derive --recovery-phrase "$(cat phrase.prv)" --policy-index 88 | tee policy_88.xsk
policy_sk1tz5k03lravcx7ecjveg6j0ndyydma2a89ny4zkmvzvpz4u6z4dgkxctdpcvhjvjl3j4peywe4l25zu4672eg5qsluz36z5mgm4n2ftg3nhmyd
```
</details>
<details>
  <summary>Policy Key with cardano-cli Output Key Files</summary>

```console
$ cscli wallet key policy derive --recovery-phrase "$(cat phrase.prv)" --signing-key-file policy_0.skey --verification-key-file policy_0.vkey | tee policy_0.xsk
policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02
$ cat policy_0.skey
{
  "type": "PaymentExtendedSigningKeyShelley_ed25519_bip32",
  "description": "Payment Signing Key",
  "cborHex": "588058d7185e436d504f3c15dab3244910689d9955d7c546a58c78765bf0ef42ab51f37eb32606d3a42b30ec4cc57f88145cf0df04bc29b6c5c25155c2c0fcc257f0f4145721658fe51d9e2f05fe131c66a42eedaff2bb60e6c892cac23bf284ef6ed1e8fc6b2fbf0ff79876723feea8bfa2e683318657f34480e1e16686bb442029"
}
$ cat policy_0.vkey
{
  "type": "PaymentExtendedVerificationKeyShelley_ed25519_bip32",
  "description": "Payment Verification Key",
  "cborHex": "5840f4145721658fe51d9e2f05fe131c66a42eedaff2bb60e6c892cac23bf284ef6ed1e8fc6b2fbf0ff79876723feea8bfa2e683318657f34480e1e16686bb442029"
}
```
</details>

## Contributing
Please see [CONTRIBUTING.md](./CONTRIBUTING.md)
