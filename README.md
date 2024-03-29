# cscli

## Goals
A lightweight cross-platform CLI for [Cardano](https://developers.cardano.org/) using .NET native types and cryptographic libraries built on top of [CardanoSharp](https://github.com/CardanoSharp/cardanosharp-wallet) and [Koios](https://api.koios.rest/). It supports the following features out of the box:
 - Building and serialising wallet primitives (i.e. recovery-phrases, keys, addresses and transactions) 
 - Live querying of accounts, addresses, transactions and native assets across both Testnet and Mainnet networks
 - Submitting transactions to the Testnet or Mainnet network
 - Cryptographic and encoding transformations (blake2b, bech32, etc.)

Why would you use `cscli` in addition to `cardano-cli`, `cardano-address`, `cardano-wallet` and a host of other tools?
### Advantages
 - Simple installation and powerful commands with **no dependencies** on a local full node or other tools/sdks
 - Easy recovery-phrase (aka mnemonic) based key and address derivation for [Hierarchical Deterministic (HD) wallets](https://github.com/bitcoin/bips/blob/master/bip-0044.mediawiki), perfect for offline management
 - [Passphrase](https://vault12.com/securemycrypto/crypto-security-basics/what-is-a-passphrase/passphrases-increase-your-protection-and-your-risk) support for additional root key security
 - [International multi-language](https://github.com/CardanoSharp/cardanosharp-wallet/tree/main/CardanoSharp.Wallet/Words) support for [recovery-phrases](https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki)
 - Generates compatible outputs for `cardano-cli`, `cardano-address` and `cardano-wallet`

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
.\release\CsCli.ConsoleTool.exe wallet recovery-phrase generate
```

Or directly building and running with `dotnet run`
```console
cd Src/ConsoleTool
dotnet run wallet recovery-phrase generate
```

Or build, test and install the global tool based on local source
```
dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet pack --no-build Src/ConsoleTool/CsCli.ConsoleTool.csproj -o nupkg -c Release
dotnet tool install --global --add-source ./nupkg cscli --version 0.1.0-local-branch.1
```
</details>

## Usage

📝❗Note: Recovery-phrases and keys should only be generated and stored in airgapped machines when used in real world transactions.

### Overview and Help
```console
$ cscli --help
cscli v0.4.0
A lightweight cross-platform tool for building and serialising Cardano wallet entities (i.e. recovery-phrases, keys, addresses and transactions), querying the chain and submitting transactions to the testnet or mainnet networks. Please refer to https://github.com/CardanoSharp/cscli for further documentation.

USAGE: cscli (OPTION | COMMAND)

Options:
    -v, --version   Show the cscli version
    -h, --help      Show this help text

Wallet Commands:
    wallet recovery-phrase generate --size <size> [--language <language>]
    wallet key root derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"]
    wallet key account derive --recovery-phrase "<string>" [--language <language>] [--passphrase "<string>"] [--account-index <index>]
    wallet key stake derive --recovery-phrase "<string>" [--account-index <index>] [--address-index <index>]
    wallet key payment derive --recovery-phrase "<string>" [--account-index <index>] [--address-index <index>]
    wallet key policy derive --recovery-phrase "<string>" [--policy-index <index>] 
    wallet key verification convert --signing-key "<bech32_skey>" 
    wallet address stake derive --recovery-phrase "<string>" --network <network> [--account-index <index>] [--address-index <index>]
    wallet address payment derive --recovery-phrase "<string>" --network <network> --payment-address-type <payment-address-type> [--account-index <index>] [--address-index <index>] [--stake-account-index <index>] [--stake-address-index <index>]

Query Commands:
    query tip --network <network>
    query protocol-parameters --network <network>
    query info account --network <network> (--stake-address <stake_address> | --address <payment_base_address>)
    query asset account --network <network> --stake-address <stake_address>
    query info address --network <network> --address <payment_address>
    query info transaction --network <network> --tx-id <transaction_id>

Transaction Commands:
    BETA: transaction simple-payment build --network <network> --from <address> --to <address> (--ada <ada_amount> | --lovelaces <lovelace_amount> | --send-all true) [--ttl <slot_no>] [--signing-key <from_addr_payment_key>] [--submit true] [--message "<string>"] [--out-file <output_path>] 
    transaction view --network <network> --cbor-hex <hex_string>
    transaction sign --cbor-hex <hex_string> --signing-keys <comma_separated_bech32_skeys> [--out-file <output_path>]
    transaction submit --network <network> --cbor-hex <hex_string>

Encoding/Cryptography Commands:
    bech32 encode --value <hex_string> --prefix <string>
    bech32 decode --value <bech32_string>
    blake2b hash --value <hex_string> [--length <hash_digest_length>]

Arguments:
    <size> ::= 9 | 12 | 15 | 18 | 21 | 24(default)
    <language> ::= english(default)|chinesesimplified|chinesetraditional|french|italian|japanese|korean|spanish|czech|portuguese
    <derivation-index> ::= 0(default) | 1 | .. | 2147483647
    <network> ::= testnet | mainnet
    <payment-address-type> ::= enterprise | base
    <hash_digest_length> ::= 160 | 224(default) | 256 | 512
```

### Generate Recovery Phrase
```console
$ cscli wallet recovery-phrase generate | tee phrase.en.prv
more enjoy seminar food bench online render dry essence indoor crazy page eight fragile mango zoo burger exhibit crouch drop rocket property alter uphold
```

<details>
  <summary>Generating a recovery phrase in Spanish</summary>

```console
$ cscli wallet recovery-phrase generate --language spanish | tee phrase.es.prv
solución aborto víspera puma molino ático ética feroz hacer orador salero baba carbón lonja texto sanción sobre pasar iris masa vacuna diseño pez playa
```
</details>

### Derive Root Key
```console
$ cscli wallet key root derive --recovery-phrase "$(cat phrase.en.prv)" | tee root.en.xsk
root_xsk12qpr53a6r7dpjpu2mr6zh96vp4whx2td4zccmplq3am6ph6z4dga6td8nph4qpcnlkdcjkd96p83t23mplvh2w42n6yc3urav8qgph3d9az6lc0px7xq7sau4r4dsfp9h0syfkhge8e6muhd69vz9j6fggdhgd4e
```

### Derive Account Key
```console
$ cscli wallet key account derive --recovery-phrase "$(cat phrase.en.prv)" | tee acct_0.en.xsk
acct_xsk13pfkzdyzuagmsquy0xjvszdxdjt84x49yrmvt2f3z8ndp6zz4dgka03j3ctm4gne9s5gullvjd7kynxxkny4qwyuuup2mcjfztctswdu3zp4s3ps5dskaq929vrp6cw8z3u77x7mymgntjw46f4l9kh3mcvg78y9
```

<details>
  <summary>Account Key with Specific Index</summary>

```console
$ cscli wallet key account derive --recovery-phrase "$(cat phrase.en.prv)" --account-index 96884067 | tee acct_96884067.en.xsk
acct_xsk1vzcpqwahy0asxuua4gswzjagmt5awjepy9clhmvtr8tgpejz4dglkfl7zhunx0dcrvljtmgcx59yzv728wlxllp646qudhgkuaj3xycu4pkysaaau0lm4z2s8t2yum7nyfn99e3xxrwgaz5yt367r8638uetazlu
```
</details>

### Derive Payment Key
```console
$ cscli wallet key payment derive --recovery-phrase "$(cat phrase.en.prv)" | tee pay_0_0.en.xsk
addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw
```
<details>
  <summary>Payment Key with Custom Indexes</summary>

```console
$ cscli wallet key payment derive --recovery-phrase "$(cat phrase.en.prv)" --account-index 569 --address-index 6949 | tee pay_569_6949.en.xsk
addr_xsk1kzjky39hv28q30qecg46f3cag3nwsjnnvn5uf0jtkrsxau2z4dgssyrv8jfwdh6frfkd0hskhszcf98xskje0c6ttcnz7k2cwdmc62uv7k6w7nwdcngkwn0semehjsdaajlv2nr5c0rg077dnsyjwxm05vhkuqet
```
</details>
<details>
  <summary>Payment Key with cardano-cli Output Key Files</summary>

```console
$ cscli wallet key payment derive --recovery-phrase "$(cat phrase.en.prv)" --signing-key-file pay_0_0.en.skey --verification-key-file pay_0_0.en.vkey | tee pay_0_0.en.xsk
addr_xsk1kzjky39hv28q30qecg46f3cag3nwsjnnvn5uf0jtkrsxau2z4dgssyrv8jfwdh6frfkd0hskhszcf98xskje0c6ttcnz7k2cwdmc62uv7k6w7nwdcngkwn0semehjsdaajlv2nr5c0rg077dnsyjwxm05vhkuqet
$ cat pay_0_0.en.skey
{
  "type": "PaymentExtendedSigningKeyShelley_ed25519_bip32",
  "description": "Payment Signing Key",
  "cborHex": "5880489c51d4ea5bf36e77c3c032e446c79728d28f93acb9ccdf6a0aab25f042ab5157073cfcc17e77ec598bf213dc7dca16c99b25ccdb8a04254b0143443e0a2724de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa2fc8083013ea2bf3a68de08a59346e129d7bd858b290f408d65cbaa97c09d1cf"
}
$ cat pay_0_0.en.vkey
{
  "type": "PaymentExtendedVerificationKeyShelley_ed25519_bip32",
  "description": "Payment Verification Key",
  "cborHex": "5840de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa2fc8083013ea2bf3a68de08a59346e129d7bd858b290f408d65cbaa97c09d1cf"
}
```
</details>
<details>
  <summary>Payment Key from Spanish recovery-phrase with custom passphrase</summary>

```console
$ cscli wallet key payment derive --language spanish --recovery-phrase "$(cat phrase.es.prv)" --passphrase "/\\/\\`/ |\\|4/\\/\\3 !5 02`//\\/\\4|\\||)!45, |<!|\\|9 0|= |<!|\\|95" --signing-key-file pay_0_0.es.skey --verification-key-file pay_0_0.es.vkey | tee pay_0_0.es.xsk
addr_xsk15pt6dccwyy2jjgmv9gxszjyzc6kchhas2dr9q6ky0xpwz4679e2t5qu0yehpmg7xr3sc3wkcyagujlk2euwl0007w68wcfdm7ajl2tzkqve7vmqds5sf38syns3u2wey05yeh70p9m5n05kftku30uqjlqy0gjh2
$ cat pay_0_0.es.skey
{
  "type": "PaymentExtendedSigningKeyShelley_ed25519_bip32",
  "description": "Payment Signing Key",
  "cborHex": "5880a057a6e30e211529236c2a0d014882c6ad8bdfb05346506ac47982e1575e2e54ba038f266e1da3c61c6188bad82751c97ecacf1df7bdfe768eec25bbf765f52ca592e0a3f8f0be44c9d65c0ac5347206b32b73ed45ba4e704014c269e2560db9560333e66c0d8520989e049c23c53b247d099bf9e12ee937d2c95db917f012f8"
}
$ cat pay_0_0.es.vkey
{
  "type": "PaymentExtendedVerificationKeyShelley_ed25519_bip32",
  "description": "Payment Verification Key",
  "cborHex": "5840a592e0a3f8f0be44c9d65c0ac5347206b32b73ed45ba4e704014c269e2560db9560333e66c0d8520989e049c23c53b247d099bf9e12ee937d2c95db917f012f8"
}
```
</details>

### Derive Stake Key
```console
$ cscli wallet key stake derive --recovery-phrase "$(cat phrase.en.prv)" | tee stake_0_0.en.xsk
stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g
```

<details>
  <summary>Stake Key with Custom Indexes</summary>

```console
$ cscli wallet key stake derive --recovery-phrase "$(cat phrase.en.prv)" --account-index 968 --address-index 83106 | tee stake_968_83106.en.xsk
stake_xsk14p0lhj3txvfcj8j08dk3ur954hmcfz6u6t00q0a3vnrsd7zz4dgcy9dwcxgf67v4rdp4mk9tkeqw70y4m7va73thnel7jwyx0achc5tyyx8r2au5x3pw37zhznj03v2cajc96paltxlh8hpefssucyecus24q26n
```
</details>
<details>
  <summary>Stake Key with cardano-cli Output Key Files</summary>

```console
$ cscli wallet key stake derive --recovery-phrase "$(cat phrase.en.prv)" --signing-key-file stake_0_0.en.skey --verification-key-file stake_0_0.en.vkey | tee stake_0_0.en.xsk
stake_xsk14p0lhj3txvfcj8j08dk3ur954hmcfz6u6t00q0a3vnrsd7zz4dgcy9dwcxgf67v4rdp4mk9tkeqw70y4m7va73thnel7jwyx0achc5tyyx8r2au5x3pw37zhznj03v2cajc96paltxlh8hpefssucyecus24q26n
$ cat stake_0_0.en.skey
{
  "type": "StakeExtendedSigningKeyShelley_ed25519_bip32",
  "description": "Stake Signing Key",
  "cborHex": "588030e983d551613634b060150ee00161cd907c7d50f099ccb73d843decec42ab5108aa69096b5fc8ce3eafe758d81f16740ce0d83e74330587f079c8a6afbba92f1bd85ec71d2d8ce0180138310983aafffa4585486db1576bc385b0ae350562e6a001925239b5726e3132b8a5598904eaf248b688668450dbd12febe1ebe127ed"
}
$ cat stake_0_0.en.vkey
{
  "type": "StakeVerificationKeyShelley_ed25519",
  "description": "Stake Verification Key",
  "cborHex": "58201bd85ec71d2d8ce0180138310983aafffa4585486db1576bc385b0ae350562e6"
}
```
</details>

### Derive Stake/Reward Address
```console
$ cscli wallet address stake derive --recovery-phrase "$(cat phrase.en.prv)" --network mainnet | tee stake_0_0.en.addr
stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj
```

<details>
  <summary>Stake Address with Custom Indexes</summary>

```console
$ cscli wallet address stake derive --recovery-phrase "$(cat phrase.en.prv)" --network mainnet --account-index 1 --address-index 7 | tee stake_1_7.en.addr
stake1u87phtdn9shvp39c44elyfdduuqg7wz072vs0vjvc20hvaqym7xan
```
</details>
<details>
  <summary>Stake Address from Spanish recovery-phrase with custom passphrase</summary>

```console
$ cscli wallet address stake derive --language spanish --recovery-phrase "$(cat phrase.es.prv)" --passphrase "/\\/\\`/ |\\|4/\\/\\3 !5 02`//\\/\\4|\\||)!45, |<!|\\|9 0|= |<!|\\|95" --network testnet | tee stake_0_0.es.addr
stake_test1uztkvps54v3yrwvxhvfz9uph8g6e2zd8jcg2cyss45g7xqclj4scq
```
</details>

### Derive Payment Enterprise Address
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.en.prv)" --payment-address-type enterprise --network mainnet | tee pay_0_0.en.addr
addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w
```

<details>
  <summary>Payment Enterprise Address with Custom Indexes</summary>
  
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.en.prv)" --payment-address-type enterprise --network mainnet --account-index 1387 --address-index 12 | tee pay_1387_12.en.addr
addr1vy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6xcc8pzd9
```

</details>

### Derive Payment Base Address
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.en.prv)" --payment-address-type base --network mainnet | tee pay_0_0_0_0.en.addr
addr1qy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqzupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdspma4ht
```

<details>
  <summary>Payment Base Address with Custom Indexes</summary>
```console
$ cscli wallet address payment derive --recovery-phrase "$(cat phrase.en.prv)" --payment-address-type base --network mainnet --account-index 1387 --address-index 12 --stake-account-index 968 --stake-address-index 83106 | tee pay_1387_12_968_83106.en.addr
addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h
```
</details>
<details>
  <summary>Payment Base Address from Spanish recovery-phrase with custom passphrase</summary>

```console
$ cscli wallet address payment derive --language spanish --recovery-phrase "$(cat phrase.es.prv)" --passphrase "/\\/\\`/ |\\|4/\\/\\3 !5 02`//\\/\\4|\\||)!45, |<!|\\|9 0|= |<!|\\|95" --network testnet --payment-address-type base | tee pay_0_0_0_0.es.addr
addr_test1qpvttg5263dnutj749k5dcr35yk5mr94fxx0q2zs2xeuxq5hvcrpf2ezgxucdwcjytcrww34j5y609ss4sfpptg3uvpsxmcdtf
```
</details>

### Derive Policy Key
```console
$ cscli wallet key policy derive --recovery-phrase "$(cat phrase.en.prv)" | tee policy_0.en.sk
policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02
```
<details>
  <summary>Policy Key with Custom Indexes</summary>

```console
$ cscli wallet key policy derive --recovery-phrase "$(cat phrase.en.prv)" --policy-index 88 | tee policy_88.en.xsk
policy_sk1tz5k03lravcx7ecjveg6j0ndyydma2a89ny4zkmvzvpz4u6z4dgkxctdpcvhjvjl3j4peywe4l25zu4672eg5qsluz36z5mgm4n2ftg3nhmyd
```
</details>
<details>
  <summary>Policy Key with cardano-cli Output Key Files</summary>

```console
$ cscli wallet key policy derive --recovery-phrase "$(cat phrase.en.prv)" --signing-key-file policy_0.skey --verification-key-file policy_0.vkey | tee policy_0.xsk
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

### Convert Verification Key 
```console
$ cscli wallet key verification convert --signing-key $(cat pay_0_0.en.xsk) | tee pay_0_0.en.xvk
addr_xvk1m62sxsn8t8apscjx2l6mejfj7wpzpmy7e6ex9yru4uk3nzmwp74zljqgxqf752ln56x7pzjex3hp98tmmpvt9y85prt9ew4f0syarncveq5jl
```

<details>
  <summary>Verification Key with cardano-cli Output Key Files</summary>

```console
$ cscli wallet key verification convert --signing-key $(cat stake_0_0.en.xsk) --verification-key-file stake_0_0.en.vkey | tee stake_0_0.en.xvk
stake_xvk1r0v9a3ca9kxwqxqp8qcsnqa2llaytp2gdkc4w67rskc2udg9vtn2qqvj2gum2unwxyet3f2e3yzw4ujgk6yxdpzsm0gjl6lpa0sj0mg4tq9sj
$ cat stake_0_0.en.vkey
{
  "type": "StakeVerificationKeyShelley_ed25519",
  "description": "Stake Verification Key",
  "cborHex": "58201bd85ec71d2d8ce0180138310983aafffa4585486db1576bc385b0ae350562e6"
}
```
</details>

### Build Simple Payment Transaction
```console
$ cscli transaction simple-payment build --network testnet --from addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t --to addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3 --ada 420 --message "thx for lunch"
84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da0f582a11902a2a1636d73676d74687820666f72206c756e636880
```

<details>
  <summary>Build and Submit Signed Simple Payment</summary>

```console
$ cscli transaction simple-payment build --submit true --signing-key addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw --network testnet --from addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t --to addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3 --ada 420 --message "thx for lunch"
5853cc86af075cc547a5a20658af54841f37a5832532a704c583ed4f010184a5
```
</details>

<details>
  <summary>Build Signed Simple Payment Tx with cardano-cli Output File</summary>

```console
$ cscli transaction simple-payment build --out-file tx.txsigned --signing-key addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw --network testnet --from addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t --to addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3 --ada 420 --message "thx for lunch"
84a500818258205853cc86af075cc547a5a20658af54841f37a5832532a704c583ed4f010184a501018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a43e80724021a0002c24d031a03afe00c0758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da10081825820de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa584025f49ad0cb27c0a297ebd2237134be2803b19d11c6e52416d3d7beba130bbc2bd95eb9b7e9e7410d7efcd5c2abd338dd62100d86b26308636335b533873bb508f582a11902a2a1636d73676d74687820666f72206c756e636880
$ cat tx.txsigned
{
  "type": "Tx AlonzoEra",
  "description": "",
  "cborHex": "84a500818258205853cc86af075cc547a5a20658af54841f37a5832532a704c583ed4f010184a501018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a43e80724021a0002c24d031a03afe00c0758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da10081825820de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa584025f49ad0cb27c0a297ebd2237134be2803b19d11c6e52416d3d7beba130bbc2bd95eb9b7e9e7410d7efcd5c2abd338dd62100d86b26308636335b533873bb508f582a11902a2a1636d73676d74687820666f72206c756e636880"
}
```
</details>

### View Transaction
```console
$ cscli transaction view --network testnet --cbor-hex 84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da0f582a11902a2a1636d73676d74687820666f72206c756e636880
{
  "id": "be9b3070e17f9f0c3e5477b315a35b9c5be0ec355f6c6bfb4beee42270413a25",
  "isValid": true,
  "transactionBody": {
    "inputs": [
      {
        "transactionId": "dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f92",
        "transactionIndex": 1
      }
    ],
    "outputs": [
      {
        "address": "addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3",
        "value": {
          "lovelaces": 420000000,
          "nativeAssets": []
        }
      },
      {
        "address": "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t",
        "value": {
          "lovelaces": 1559469529,
          "nativeAssets": []
        }
      }
    ],
    "mint": [],
    "fee": 171749,
    "ttl": 61856856,
    "auxiliaryDataHash": "0088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3d",
    "transactionStartInterval": null
  },
  "transactionWitnessSet": null,
  "auxiliaryData": {
    "metadata": {
      "674": {
        "msg": "thx for lunch"
      }
    }
  }
}
```

### Sign Transaction

```console
$ cscli transaction sign --signing-keys addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw --network testnet --cbor-hex 84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da0f582a11902a2a1636d73676d74687820666f72206c756e636880
84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da10081825820de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa5840d856197bc4f4cb62439ea9c2781e9764855685c3809364ef759b1926047d7bb326fecf2ee1144c5d49cf2f53feb432fa1af30e00d8d69c4145e6494fd1979a0cf582a11902a2a1636d73676d74687820666f72206c756e636880
```


### Submit Transaction
```console
$ cscli transaction submit --network testnet --cbor-hex 84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da10081825820de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa5840d856197bc4f4cb62439ea9c2781e9764855685c3809364ef759b1926047d7bb326fecf2ee1144c5d49cf2f53feb432fa1af30e00d8d69c4145e6494fd1979a0cf582a11902a2a1636d73676d74687820666f72206c756e636880
5c9f1456a2f7cdf30c12d569ede3f298b377115a63dc0cef791e692dbe4be26b
```

### Query Tip
```console
$ cscli query tip --network mainnet
{
  "hash": "cff749d596281c59f7f6c50eb7a8ede766397dd7d943701210ddd32c677e19ef",
  "epoch_no": 347,
  "abs_slot": 64835901,
  "epoch_slot": 295101,
  "block_no": 7431004,
  "block_time": "2022-06-28T07:43:12"
}
```

### Query Protocol Parameters
```console
$ cscli query protocol-parameters --network mainnet
{
  "epoch_no": 347,
  "min_fee_a": 44,
  "min_fee_b": 155381,
  ...
  "coins_per_utxo_word": 34482
}    
```

### Query Account Info
```console
$ cscli query info account --network mainnet --stake-address stake1uyrx65wjqjgeeksd8hptmcgl5jfyrqkfq0xe8xlp367kphsckq250
[
  {
    "status": "registered",
    "delegated_pool": "pool14wk2m2af7y4gk5uzlsmsunn7d9ppldvcxxa5an9r5ywek8330fg",
    "total_balance": "1126364036992",
    "utxo": "1120067931255",
    "rewards": "90668729339",
    "withdrawals": "84372623602",
    "rewards_available": "6296105737",
    "reserves": "0",
    "treasury": "0"
  }
]
```
<details>
  <summary>Query Account Info of Payment Address (requires base address)</summary>

```console
$ cscli query info account --network mainnet --address addr1q9r4307pqxq93fh554yvfssha46atz7h8waha568d8ddvnktwkkz3tg57qd9knlsfyhlgjuxpyxhl09u2w8f4l20hk2q7dt678
[
  {
    "status": "registered",
    "delegated_pool": "pool1ddg6t2h9kj6lqlec4ncjs945lzj43m3ggrgdhf5sgzhtygpkznz",
    "total_balance": "7031456885",
    "utxo": "7019386794",
    "rewards": "56497309",
    "withdrawals": "44427218",
    "rewards_available": "12070091",
    "reserves": "0",
    "treasury": "0"
  }
]
```
</details>

### Query Account Asset 
```console
$ cscli query asset account --network testnet --stake-address $(cat stake_0_0.es.addr)
[
  {
    "asset_policy": "540f107c7a3df20d2111a41c3bc407cce3e63c10c8dd673d51a02c22",
    "asset_name": "COND1",
    "quantity": "1"
  }
]
```
<details>
  <summary>Query Account Asset of Payment Address (requires base address)</summary>

```console
$ cscli query asset account --network testnet --address $(cat pay_0_0_0_0.es.addr)
[
  {
    "asset_policy": "540f107c7a3df20d2111a41c3bc407cce3e63c10c8dd673d51a02c22",
    "asset_name": "COND1",
    "quantity": "1"
  }
]
```
</details>

### Query Address Info 
```console
$ cscli query info address --network testnet --address $(cat pay_0_0_0_0.es.addr)
{
  "balance": "1001344798",
  "stake_address": "stake_test1uztkvps54v3yrwvxhvfz9uph8g6e2zd8jcg2cyss45g7xqclj4scq",
  "utxo_set": [ ... ]
}
```

### Query Transaction Info 
```console
$ cscli query info transaction --network testnet --txid 4fe73db7e345f6853ade214b0779d5db51f9a4b5e296198d3cb84b7b707e7d34
[
  {
    "tx_hash": "4fe73db7e345f6853ade214b0779d5db51f9a4b5e296198d3cb84b7b707e7d34",
    "block_hash": "e96c400f303d2f30f7b49761b1c541b5a29b43ddb28268a1f179b2877f828aad",
    ...
    "inputs": [ ... ],
    "outputs": [ ... ],
    ...
  }
]
```

### Bech32 Decode
```console
$ cscli bech32 decode --value "$(cat pay_0_0.en.addr)"
61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600
```

### Bech32 Encode
```console
$ cscli bech32 encode --value 61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600 --prefix addr
addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w
```

### Blake2b Hash
```console
$ cscli blake2b hash --length 224 --value de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa  
282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600
```

## Contributing
Please see [CONTRIBUTING.md](./CONTRIBUTING.md) 