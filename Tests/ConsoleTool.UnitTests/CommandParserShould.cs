﻿using Cscli.ConsoleTool.Crypto;
using Cscli.ConsoleTool.Query;
using Cscli.ConsoleTool.Transaction;
using Cscli.ConsoleTool.Wallet;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class CommandParserShould
{
    [Theory]
    [InlineData("run")]
    [InlineData("-z")]
    [InlineData("--make")]
    [InlineData("!")]
    public void ParseArgs_To_ShowInvalidArgumentCommand_When_Arguments_Are_Invalid(params string[] args)
    {
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<ShowInvalidArgumentCommand>();
    }

    [Theory]
    [InlineData("help")]
    [InlineData("--help")]
    [InlineData("-h")]
    public void ParseArgs_Correctly_To_ShowBaseHelpCommand_When_Arguments_Are_Valid(params string[] args)
    {
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<ShowBaseHelpCommand>();
    }

    [Theory]
    [InlineData("version")]
    [InlineData("--version")]
    [InlineData("-v")]
    public void ParseArgs_Correctly_To_ShowVersionTextCommand_When_Arguments_Are_Valid(params string[] args)
    {
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<ShowVersionCommand>();
    }

    [Theory]
    [InlineData("walet", "recovery-phrase", "generate")]
    [InlineData("wallet", "recovery-phase", "generate")]
    [InlineData("wallet", "recovery-phrase", "gen")]
    [InlineData("wallethd", "recovery-phrase", "generate")]
    public void ParseArgs_To_ShowInvalidArgumentCommand_When_Arguments_Have_Spelling_Mistakes(params string[] args)
    {
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<ShowInvalidArgumentCommand>();
    }

    [Theory]
    [InlineData("wallet recovery-phrase generate", Constants.DefaultMnemonicCount, Constants.DefaultMnemonicLanguage)]
    [InlineData("wallet recovery-phrase generate --language Spanish", Constants.DefaultMnemonicCount, "Spanish")]
    [InlineData("wallet recovery-phrase generate --size 15", 15, Constants.DefaultMnemonicLanguage)]
    [InlineData("wallet recovery-phrase generate --size 21 --language English", 21, "English")]
    [InlineData("wallet recovery-phrase generate --size 21 --language Spanish", 21, "Spanish")]
    [InlineData("wallet recovery-phrase generate --size 24 --language Japanese", 24, "Japanese")]
    public void ParseArgs_Correctly_To_GenerateMnemonicCommand_When_Options_Are_Valid(
        string flatArgs, int expectedSize, string expectedLanguage)
    {
        var command = CommandParser.ParseArgsToCommand(flatArgs.Split(' '));
        command.Should().BeOfType<GenerateMnemonicCommand>();
        ((GenerateMnemonicCommand)command).Size.Should().Be(expectedSize);
        ((GenerateMnemonicCommand)command).Language.Should().Be(expectedLanguage);
    }

    [Theory]
    [InlineData(
        "wallet key root derive --recovery-phrase {MNEMONIC}",
        "noise ability outer loud cabbage borrow model daughter small visual connect awake attract topic float gift bench video trial tomorrow piece risk decrease daring",
        Constants.DefaultMnemonicLanguage, 
        "")]
    [InlineData(
        "wallet key root derive --recovery-phrase {MNEMONIC} --passphrase helloworld --language Spanish",
        "visor sepia feria señal brecha logro sirena flaco lámina arroz juicio cumbre centro aseo jarra ganso mes abogado rociar ámbito isla anuncio dosis dolor",
        "Spanish",
        "helloworld")]
    public void ParseArgs_Correctly_To_DeriveRootKeyCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase)
    {
        var args = GenerateArgs(flatArgs, expectedMnemonic);

        var command = CommandParser.ParseArgsToCommand(args);

        command.Should().BeOfType<DeriveRootKeyCommand>();
        ((DeriveRootKeyCommand)command).Mnemonic.Should().Be(expectedMnemonic);
        ((DeriveRootKeyCommand)command).Language.Should().Be(expectedLanguage);
        ((DeriveRootKeyCommand)command).Passphrase.Should().Be(expectedPassPhrase);
    }

    [Theory]
    [InlineData(
        "wallet key account derive --recovery-phrase {MNEMONIC}",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0)]
    [InlineData(
        "wallet key account derive --recovery-phrase {MNEMONIC} --passphrase screenfuryidentifyworldsailarenadevoteonlygas --language Italian --account-index 1234567",
        "svagare sprecare dirupo scandalo pedonale midollo indagine oste proroga fessura partire snervato pulsante eremita poesia esito esofago coltivato affisso cravatta lombare pupazzo colore spruzzo",
        "Italian",
        "screenfuryidentifyworldsailarenadevoteonlygas", 1234567)]
    public void ParseArgs_Correctly_To_DeriveAccountCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, int expectedAccountIndex)
    {
        var args = GenerateArgs(flatArgs, expectedMnemonic);

        var command = CommandParser.ParseArgsToCommand(args);

        var acctKeyCommand = (DeriveAccountKeyCommand)command;
        command.Should().BeOfType<DeriveAccountKeyCommand>();
        acctKeyCommand.Mnemonic.Should().Be(expectedMnemonic);
        acctKeyCommand.Language.Should().Be(expectedLanguage);
        acctKeyCommand.Passphrase.Should().Be(expectedPassPhrase);
        acctKeyCommand.AccountIndex.Should().Be(expectedAccountIndex);
    }

    [Theory]
    [InlineData(
        "wallet key payment derive --recovery-phrase {MNEMONIC}",
        "repeat jazz magnet finger sunny gaze shuffle deputy feel forget decline parent immune actor anchor funny avocado source replace setup grace best inflict capable",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0)]
    [InlineData(
        "wallet key payment derive --recovery-phrase {MNEMONIC} --passphrase helloworld --language Spanish --account-index 2 --address-index 256",
        "guiso calamar suplir uno ingenio furia papel otoño rebaño treinta hazaña fallo coser ánimo palco piloto edición túnica cara probar elixir toser grosor verbo",
        "Spanish",
        "helloworld", 2, 256)]
    public void ParseArgs_Correctly_To_DerivePaymentKeyCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, int expectedAccountIndex, int expectedAddressIndex)
    {
        var args = GenerateArgs(flatArgs, expectedMnemonic);

        var command = CommandParser.ParseArgsToCommand(args);

        var payKeyCommand = (DerivePaymentKeyCommand)command;
        command.Should().BeOfType<DerivePaymentKeyCommand>();
        payKeyCommand.Mnemonic.Should().Be(expectedMnemonic);
        payKeyCommand.Language.Should().Be(expectedLanguage);
        payKeyCommand.Passphrase.Should().Be(expectedPassPhrase);
        payKeyCommand.AccountIndex.Should().Be(expectedAccountIndex);
        payKeyCommand.AddressIndex.Should().Be(expectedAddressIndex);
    }

    [Theory]
    [InlineData(
        "wallet key policy derive --recovery-phrase {MNEMONIC}",
        "essay choose supply announce entire cart gap duty grow dog similar moral illegal screen jump fury identify world sail arena devote only gas video",
        Constants.DefaultMnemonicLanguage,
        "", 0)]
    [InlineData(
        "wallet key policy derive --recovery-phrase {MNEMONIC} --passphrase p455 --language Spanish --policy-index 8",
        "dardo demora osadía severo veinte peor humilde óxido secta bocina hallar flauta orador recreo villa fax tienda delito amante lector vicio buitre cosmos zona",
        "Spanish",
        "p455", 8)]
    public void ParseArgs_Correctly_To_DerivePolicyKeyCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, int expectedPolicyIndex)
    {
        var args = GenerateArgs(flatArgs, expectedMnemonic);

        var command = CommandParser.ParseArgsToCommand(args);

        var policyKeyCommand = (DerivePolicyKeyCommand)command;
        command.Should().BeOfType<DerivePolicyKeyCommand>();
        policyKeyCommand.Mnemonic.Should().Be(expectedMnemonic);
        policyKeyCommand.Language.Should().Be(expectedLanguage);
        policyKeyCommand.Passphrase.Should().Be(expectedPassPhrase);
        policyKeyCommand.PolicyIndex.Should().Be(expectedPolicyIndex);
    }

    [Theory]
    [InlineData(
        "wallet key stake derive --recovery-phrase {MNEMONIC}",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0)]
    [InlineData(
        "wallet key stake derive --recovery-phrase {MNEMONIC} --passphrase helloworld --language Spanish --account-index 2 --address-index 256",
        "acabar maestro llaga cruz senda veinte remar avance toro oído loción sótano resto tesis velero programa zafiro fresa alteza faltar metro género pelea pista",
        "Spanish",
        "helloworld", 2, 256)]
    public void ParseArgs_Correctly_To_DeriveStakeKeyCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, int expectedAccountIndex, int expectedAddressIndex)
    {
        var args = GenerateArgs(flatArgs, expectedMnemonic);

        var command = CommandParser.ParseArgsToCommand(args);

        var stakeKeyCommand = (DeriveStakeKeyCommand)command;
        command.Should().BeOfType<DeriveStakeKeyCommand>();
        stakeKeyCommand.Mnemonic.Should().Be(expectedMnemonic);
        stakeKeyCommand.Language.Should().Be(expectedLanguage);
        stakeKeyCommand.Passphrase.Should().Be(expectedPassPhrase);
        stakeKeyCommand.AccountIndex.Should().Be(expectedAccountIndex);
        stakeKeyCommand.AddressIndex.Should().Be(expectedAddressIndex);
    }

    [Theory]
    [InlineData(
        "wallet key verification convert --signing-key addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw",
        "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw", null)]
    [InlineData(
        "wallet key verification convert --signing-key policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02 --verification-key-file pay.vkey",
        "policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02", "pay.vkey")]
    public void ParseArgs_Correctly_To_ConvertVerificationKeyCommand_When_Options_Are_Valid(
        string args, string expectedSigningKey, string expectedVerificationKeyFile)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));
        command.Should().BeOfType<ConvertVerificationKeyCommand>();
        ((ConvertVerificationKeyCommand)command).SigningKey.Should().Be(expectedSigningKey);
        ((ConvertVerificationKeyCommand)command).VerificationKeyFile.Should().Be(expectedVerificationKeyFile);
    }

    [Theory]
    [InlineData(
        "wallet address stake derive --recovery-phrase {MNEMONIC} --network Testnet",
        "rabbit fence domain dirt burden bone entry genre twelve obey dwarf icon fabric tattoo chalk monster deputy tomato gun toy garment portion gun ribbon",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0, "Testnet")]
    [InlineData(
        "wallet address stake derive --recovery-phrase {MNEMONIC} --passphrase helloworld --language Spanish --network mainnet --account-index 101 --address-index 980",
        "vena pomo bolero papel colina paleta regalo alma dibujo examen lindo programa venir bozal elogio tacto romper observar bono separar refrán ecuador clase reducir",
        "Spanish",
        "helloworld", 101, 980, "mainnet")]
    [InlineData(
        "wallet address stake derive --recovery-phrase {MNEMONIC} --passphrase 0ZYM4ND14Z --language Japanese --network Mainnet --account-index 2147483647 --address-index 2147483647",
        "みすい よけい ちしりょう つみき たいせつ たりる せんぱい れんぞく めいぶつ あじわう はらう ちぬき やぶる ひろう にんい うらぐち おやゆび きんじょ きりん たいおう ねいる みつかる うよく かあつ",
        "Japanese",
        "0ZYM4ND14Z", 2147483647, 2147483647, "Mainnet")]
    public void ParseArgs_Correctly_To_DeriveStakeAddressCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, int expectedAccountIndex, int expectedAddressIndex, string expectedNetworkTag)
    {
        var args = GenerateArgs(flatArgs, expectedMnemonic);

        var command = CommandParser.ParseArgsToCommand(args);
        var stakeAddressCommand = (DeriveStakeAddressCommand)command;
        command.Should().BeOfType<DeriveStakeAddressCommand>();
        stakeAddressCommand.Mnemonic.Should().Be(expectedMnemonic);
        stakeAddressCommand.Language.Should().Be(expectedLanguage);
        stakeAddressCommand.Passphrase.Should().Be(expectedPassPhrase);
        stakeAddressCommand.AccountIndex.Should().Be(expectedAccountIndex);
        stakeAddressCommand.AddressIndex.Should().Be(expectedAddressIndex);
        stakeAddressCommand.Network.Should().Be(expectedNetworkTag);
    }

    [Theory]
    [InlineData(
        "wallet address payment derive --recovery-phrase {MNEMONIC} --network testnet --payment-address-type Enterprise",
        "slight aspect potato wealth two lazy ill try kick visit chunk cloth snap follow now sun curve quality cousin sister decrease help stadium enact",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0, 0, 0, "testnet", "Enterprise")]
    [InlineData(
        "wallet address payment derive --language Portugese --recovery-phrase {MNEMONIC} --network Mainnet --payment-address-type Base --account-index 256 --address-index 512 --stake-account-index 88 --stake-address-index 29 --passphrase 0ZYM4ND14Z",
        "sadio sombrio prato selvagem enrugar fugir depois braveza acolhida javali enviado alfinete emenda mexer legado goela vedar refogar pivete afrontar tracejar materno vespa chapada",
        "Portugese", "0ZYM4ND14Z", 256, 512, 88, 29, "Mainnet", "Base")]
    public void ParseArgs_Correctly_To_DerivePaymentAddressCommand_When_Options_Are_Valid(
        string flatArgs, 
        string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, 
        int expectedAccountIndex, int expectedAddressIndex,
        int expectedStakeAccountIndex, int expectedStakeAddressIndex, string expectedNetworkTag, string expectedPaymentAddressType)
    {
        var args = GenerateArgs(flatArgs, expectedMnemonic);

        var command = CommandParser.ParseArgsToCommand(args);
        var paymentAddressCommand = (DerivePaymentAddressCommand)command;
        command.Should().BeOfType<DerivePaymentAddressCommand>();
        paymentAddressCommand.Mnemonic.Should().Be(expectedMnemonic);
        paymentAddressCommand.Language.Should().Be(expectedLanguage);
        paymentAddressCommand.Passphrase.Should().Be(expectedPassPhrase);
        paymentAddressCommand.AccountIndex.Should().Be(expectedAccountIndex);
        paymentAddressCommand.AddressIndex.Should().Be(expectedAddressIndex);
        paymentAddressCommand.StakeAccountIndex.Should().Be(expectedStakeAccountIndex);
        paymentAddressCommand.StakeAddressIndex.Should().Be(expectedStakeAddressIndex);
        paymentAddressCommand.Network.Should().Be(expectedNetworkTag);
        paymentAddressCommand.PaymentAddressType.Should().Be(expectedPaymentAddressType);
    }

    [Theory]
    [InlineData("bech32 encode --value 009493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251 --prefix addr",
    "009493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251",
    "addr")]
    [InlineData("bech32 encode --prefix addr_test --value 609493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e",
    "609493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e",
    "addr_test")]
    public void ParseArgs_Correctly_To_EncodeBech32Command_When_Options_Are_Valid(string args, string expectedValue, string expectedPrefix)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var bech32DecodeCommand = (EncodeBech32Command)command;
        bech32DecodeCommand.Should().BeOfType<EncodeBech32Command>();
        bech32DecodeCommand.Value.Should().Be(expectedValue);
        bech32DecodeCommand.Prefix.Should().Be(expectedPrefix);
    }

    [Theory]
    [InlineData("bech32 decode --value addr_test1zrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gten0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgsxj90mg",
        "addr_test1zrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gten0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgsxj90mg")]
    [InlineData("bech32 decode --value addr1x8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gt7r0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shskhj42g",
        "addr1x8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gt7r0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shskhj42g")]
    public void ParseArgs_Correctly_To_DecodeBech32Command_When_Options_Are_Valid(string args, string expectedValue)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var bech32DecodeCommand = (DecodeBech32Command)command;
        bech32DecodeCommand.Should().BeOfType<DecodeBech32Command>();
        bech32DecodeCommand.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("blake2b hash --value 8512f8eb5d0a26038f703d58e2b45123b6e441e2208cb37fc22605f0d215a26a",
        "8512f8eb5d0a26038f703d58e2b45123b6e441e2208cb37fc22605f0d215a26a", 224)]
    [InlineData("blake2b hash --length 160 --value d92b380b5413b76202056eea98b6bf579d52a54a44688c1f7f97b8237469636B65745F6D61647269645F3033",
        "d92b380b5413b76202056eea98b6bf579d52a54a44688c1f7f97b8237469636B65745F6D61647269645F3033", 160)]
    public void ParseArgs_Correctly_To_HashBlake2bCommand_When_Options_Are_Valid(string args, string expectedValue, int expectedLength)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var hashBlake2bCommand = (HashBlake2bCommand)command;
        hashBlake2bCommand.Should().BeOfType<HashBlake2bCommand>();
        hashBlake2bCommand.Value.Should().Be(expectedValue);
        hashBlake2bCommand.Length.Should().Be(expectedLength);
    }

    [Theory]
    [InlineData("query tip --network testnet", "testnet")]
    [InlineData("query tip --network mainnet", "mainnet")]
    public void ParseArgs_Correctly_To_QueryTipCommand_When_Options_Are_Valid(string args, string expectedNetwork)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var queryTipCommand = (QueryTipCommand)command;
        queryTipCommand.Should().BeOfType<QueryTipCommand>();
        queryTipCommand.Network.Should().Be(expectedNetwork);
    }

    [Theory]
    [InlineData("query protocol-parameters --network testnet", "testnet")]
    [InlineData("query protocol-parameters --network mainnet", "mainnet")]
    public void ParseArgs_Correctly_To_QueryProtocolParametersCommand_When_Options_Are_Valid(string args, string expectedNetwork)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var queryProtocolParametersCommand = (QueryProtocolParametersCommand)command;
        queryProtocolParametersCommand.Should().BeOfType<QueryProtocolParametersCommand>();
        queryProtocolParametersCommand.Network.Should().Be(expectedNetwork);
    }

    [Theory]
    [InlineData("query asset account --network testnet --stake-address stake_test1uzdyuk9ts8eguyzn6s64hwy8phzkhqf76zfwznwfpaw94dgmj3zcx", "testnet", "stake_test1uzdyuk9ts8eguyzn6s64hwy8phzkhqf76zfwznwfpaw94dgmj3zcx")]
    [InlineData("query asset account --network mainnet --stake-address stake1uxwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qg8t55dn", "mainnet", "stake1uxwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qg8t55dn")]
    public void ParseArgs_Correctly_To_QueryAccountAssetCommand_When_Options_Are_Valid(string args, string expectedNetwork, string expectedStakeAddress)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var queryAccountAssetCommand = (QueryAccountAssetCommand)command;
        queryAccountAssetCommand.Should().BeOfType<QueryAccountAssetCommand>();
        queryAccountAssetCommand.Network.Should().Be(expectedNetwork);
        queryAccountAssetCommand.StakeAddress.Should().Be(expectedStakeAddress);
    }

    [Theory]
    [InlineData("query info account --network testnet --stake-address stake_test1uzdyuk9ts8eguyzn6s64hwy8phzkhqf76zfwznwfpaw94dgmj3zcx", "testnet", "stake_test1uzdyuk9ts8eguyzn6s64hwy8phzkhqf76zfwznwfpaw94dgmj3zcx")]
    [InlineData("query info account --network mainnet --stake-address stake1uxwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qg8t55dn", "mainnet", "stake1uxwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qg8t55dn")]
    public void ParseArgs_Correctly_To_QueryAccountInfoCommand_When_Options_Are_Valid(string args, string expectedNetwork, string expectedStakeAddress)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var queryAccountInfoCommand = (QueryAccountInfoCommand)command;
        queryAccountInfoCommand.Should().BeOfType<QueryAccountInfoCommand>();
        queryAccountInfoCommand.Network.Should().Be(expectedNetwork);
        queryAccountInfoCommand.StakeAddress.Should().Be(expectedStakeAddress);
    }

    [Theory]
    [InlineData("query info address --network testnet --address addr_test1qr3ls8ycdxgvlkqzsw2ysk9w2rpdstm208fnpnnsznst0lvalyteh6cvaz0cgqj7q2hprsvtxqp7w6gpf892vch6l5qs6ug90f", "testnet", "addr_test1qr3ls8ycdxgvlkqzsw2ysk9w2rpdstm208fnpnnsznst0lvalyteh6cvaz0cgqj7q2hprsvtxqp7w6gpf892vch6l5qs6ug90f")]
    [InlineData("query info address --network mainnet --address addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w", "mainnet", "addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w")]
    public void ParseArgs_Correctly_To_QueryAddressInfoCommand_When_Options_Are_Valid(string args, string expectedNetwork, string expectedAddress)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var queryAddressInfoCommand = (QueryAddressInfoCommand)command;
        queryAddressInfoCommand.Should().BeOfType<QueryAddressInfoCommand>();
        queryAddressInfoCommand.Network.Should().Be(expectedNetwork);
        queryAddressInfoCommand.Address.Should().Be(expectedAddress);
    }

    [Theory]
    [InlineData("query info transaction --network testnet --tx-id 18d10520a11acfa8ceb8e13b2560747cf4f357cfaac1dc83b35a26c5dc61a2e3", "testnet", "18d10520a11acfa8ceb8e13b2560747cf4f357cfaac1dc83b35a26c5dc61a2e3")]
    [InlineData("query info transaction --network mainnet --tx-id 421734e5c8e5be24d788da2defeb9005516c20eb7d3ddef2140e836d20282a2a", "mainnet", "421734e5c8e5be24d788da2defeb9005516c20eb7d3ddef2140e836d20282a2a")]
    public void ParseArgs_Correctly_To_QueryTransactionInfoCommand_When_Options_Are_Valid(string args, string expectedNetwork, string expectedTxId)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var queryTransactionInfoCommand = (QueryTransactionInfoCommand)command;
        queryTransactionInfoCommand.Should().BeOfType<QueryTransactionInfoCommand>();
        queryTransactionInfoCommand.Network.Should().Be(expectedNetwork);
        queryTransactionInfoCommand.TxId.Should().Be(expectedTxId);
    }

    [Theory]
    [InlineData("transaction submit --network testnet --cbor-hex 9df9179beb0ce89f84025e02ae11c18b3003e7690149caa662fafd01", "testnet", "9df9179beb0ce89f84025e02ae11c18b3003e7690149caa662fafd01")]
    [InlineData("transaction submit --network mainnet --cbor-hex 61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600", "mainnet", "61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600")]
    public void ParseArgs_Correctly_SubmitTransactionCommand_When_Options_Are_Valid(string args, string expectedNetwork, string expectedCborHex)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var submitTransactionCommand = (SubmitTransactionCommand)command;
        submitTransactionCommand.Should().BeOfType<SubmitTransactionCommand>();
        submitTransactionCommand.Network.Should().Be(expectedNetwork);
        submitTransactionCommand.CborHex.Should().Be(expectedCborHex);
    }

    [Theory]
    [InlineData("transaction view --network testnet --cbor-hex 9df9179beb0ce89f84025e02ae11c18b3003e7690149caa662fafd01", "testnet", "9df9179beb0ce89f84025e02ae11c18b3003e7690149caa662fafd01")]
    [InlineData("transaction view --network mainnet --cbor-hex 61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600", "mainnet", "61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600")]
    public void ParseArgs_Correctly_ViewTransactionCommand_When_Options_Are_Valid(string args, string expectedNetwork, string expectedCborHex)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var viewTransactionCommand = (ViewTransactionCommand)command;
        viewTransactionCommand.Should().BeOfType<ViewTransactionCommand>();
        viewTransactionCommand.Network.Should().Be(expectedNetwork);
        viewTransactionCommand.CborHex.Should().Be(expectedCborHex);
    }

    [Theory]
    [InlineData("transaction sign --cbor-hex 9df9179beb0ce89f84025e02ae11c18b3003e7690149caa662fafd01 --signing-keys addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw,policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02", "9df9179beb0ce89f84025e02ae11c18b3003e7690149caa662fafd01", "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw,policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02", null)]
    [InlineData("transaction sign --signing-keys addr_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqcgddqj --cbor-hex 61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600 --outfile tx.txsigned", "61282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f58600", "addr_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqcgddqj", "tx.txsigned")]
    public void ParseArgs_Correctly_SignTransactionCommand_When_Options_Are_Valid(string args, string expectedCborHex, string expectedSigningKeys, string expectedOutFile)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var signTransactionCommand = (SignTransactionCommand)command;
        signTransactionCommand.Should().BeOfType<SignTransactionCommand>();
        signTransactionCommand.CborHex.Should().Be(expectedCborHex);
        signTransactionCommand.SigningKeys.Should().Be(expectedSigningKeys);
        signTransactionCommand.OutFile.Should().Be(expectedOutFile);
    }

    [Theory]
    [InlineData("transaction simple-payment build --network testnet --from addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t --to addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3 --ada 2.987654", "testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", null, "addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3", 0, 2.987654, false, null, false, null)]
    [InlineData("transaction simple-payment build --lovelaces 1234567 --from addr_test1vzjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0s69l8h0  --to addr_test1vpcdmuc2rrxu6e8vewrgvdlwhz32h8dqf38yyw20hqacnpsdffx77 --submit true --signing-key addr_xsk1lplvzxsmads66xa9n3869v8jjuyl7zs0pprtrjvuh277mu6z4dgcess2snu9tr26xqsf5cnvafm2xaah43tzrfqpayk085az37dtx0jl6h6v4p75zahdvxj32gy39s0y230u4pw7gc63ayj3aweaqvuj4cdhqk8g --out-file tx.txsigned --network testnet", "testnet", "addr_test1vzjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0s69l8h0", "addr_xsk1lplvzxsmads66xa9n3869v8jjuyl7zs0pprtrjvuh277mu6z4dgcess2snu9tr26xqsf5cnvafm2xaah43tzrfqpayk085az37dtx0jl6h6v4p75zahdvxj32gy39s0y230u4pw7gc63ayj3aweaqvuj4cdhqk8g", "addr_test1vpcdmuc2rrxu6e8vewrgvdlwhz32h8dqf38yyw20hqacnpsdffx77", 1234567, 0, false, null, true, "tx.txsigned")]
    [InlineData("transaction simple-payment build  --network mainnet --from addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w --to addr1v9uuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagmgkpv5 --send-all true --message iou", "mainnet", "addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w", null, "addr1v9uuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagmgkpv5", 0, 0, true, "iou", false, null)]
    [InlineData("transaction simple-payment build --from addr1q9uuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aa2upvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdstd8nlr --to addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f --ada 60 --network mainnet --message maccaslunch --out-file tx.txunsigned", "mainnet", "addr1q9uuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aa2upvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdstd8nlr", null, "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", 0, 60, false, "maccaslunch", false, "tx.txunsigned")]
    [InlineData("transaction simple-payment build --to addr1q9cdmuc2rrxu6e8vewrgvdlwhz32h8dqf38yyw20hqacnpjupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvds7ejjnf --from addr1v9cdmuc2rrxu6e8vewrgvdlwhz32h8dqf38yyw20hqacnpskpa63m --lovelaces 2882856190 --message Birthdayprezzie --submit true --out-file bdayprez.txsigned --network mainnet --signing-key addr_xsk1gzza52g80rhqw4gy7z49wf82lcn5qrthrmgpnsr7kun3am2z4dgncv8yagkhfjgknna0074lff7jf6f0ptrwmlr5zj2hky8wsz4gu0eu26emwg2lczjgwkngwnmd39a3jshz76e8yy3lf0rs0kxy8ca39ctajv9q", "mainnet", "addr1v9cdmuc2rrxu6e8vewrgvdlwhz32h8dqf38yyw20hqacnpskpa63m", "addr_xsk1gzza52g80rhqw4gy7z49wf82lcn5qrthrmgpnsr7kun3am2z4dgncv8yagkhfjgknna0074lff7jf6f0ptrwmlr5zj2hky8wsz4gu0eu26emwg2lczjgwkngwnmd39a3jshz76e8yy3lf0rs0kxy8ca39ctajv9q", "addr1q9cdmuc2rrxu6e8vewrgvdlwhz32h8dqf38yyw20hqacnpjupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvds7ejjnf", 2882856190, 0, false, "Birthdayprezzie", true, "bdayprez.txsigned")]
    public void ParseArgs_Correctly_BuildSimplePaymentTransactionCommand_When_Options_Are_Valid(
        string args, 
        string expectedNetwork, string expectedFrom, string expectedSigningKey, string expectedTo, 
        ulong expectedLovelaces, decimal expectedAda, bool expectedSendAll, 
        string expectedMessage, bool expectedSubmit, string expectedOutFile)
    {
        var command = CommandParser.ParseArgsToCommand(args.Split(' '));

        var buildSimpleTxCommand = (BuildSimplePaymentTransactionCommand)command;
        buildSimpleTxCommand.Should().BeOfType<BuildSimplePaymentTransactionCommand>();
        buildSimpleTxCommand.Network.Should().Be(expectedNetwork);
        buildSimpleTxCommand.From.Should().Be(expectedFrom);
        buildSimpleTxCommand.SigningKey.Should().Be(expectedSigningKey);
        buildSimpleTxCommand.To.Should().Be(expectedTo);
        buildSimpleTxCommand.Lovelaces.Should().Be(expectedLovelaces);
        buildSimpleTxCommand.Ada.Should().Be(expectedAda);
        buildSimpleTxCommand.SendAll.Should().Be(expectedSendAll);
        buildSimpleTxCommand.Message.Should().Be(expectedMessage);
        buildSimpleTxCommand.Submit.Should().Be(expectedSubmit);
        buildSimpleTxCommand.OutFile.Should().Be(expectedOutFile);
    }

    private static string[] GenerateArgs(string flatArgs, string expectedMnemonic)
    {
        var args = flatArgs.Split(' ');
        // Replace {MNEMONIC} with full mnemonic to mimic built-in argument binding to the args array
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "{MNEMONIC}")
                args[i] = expectedMnemonic;
        }

        return args;
    }
}
