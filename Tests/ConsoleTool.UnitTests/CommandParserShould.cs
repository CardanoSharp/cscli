using Cscli.ConsoleTool.Commands;
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
        "wallet address stake derive --recovery-phrase {MNEMONIC} --network-tag Testnet",
        "rabbit fence domain dirt burden bone entry genre twelve obey dwarf icon fabric tattoo chalk monster deputy tomato gun toy garment portion gun ribbon",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0, "Testnet")]
    [InlineData(
        "wallet address stake derive --recovery-phrase {MNEMONIC} --passphrase helloworld --language Spanish --network-tag Mainnet --account-index 101 --address-index 980",
        "vena pomo bolero papel colina paleta regalo alma dibujo examen lindo programa venir bozal elogio tacto romper observar bono separar refrán ecuador clase reducir",
        "Spanish",
        "helloworld", 101, 980, "Mainnet")]
    [InlineData(
        "wallet address stake derive --recovery-phrase {MNEMONIC} --passphrase 0ZYM4ND14Z --language Japanese --network-tag Mainnet --account-index 2147483647 --address-index 2147483647",
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
        stakeAddressCommand.NetworkTag.Should().Be(expectedNetworkTag);
    }

    [Theory]
    [InlineData(
        "wallet address payment derive --recovery-phrase {MNEMONIC} --network-tag Testnet --payment-address-type Enterprise",
        "slight aspect potato wealth two lazy ill try kick visit chunk cloth snap follow now sun curve quality cousin sister decrease help stadium enact",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0, 0, 0, "Testnet", "Enterprise")]
    [InlineData(
        "wallet address payment derive --language Portugese --recovery-phrase {MNEMONIC} --network-tag Mainnet --payment-address-type Base --account-index 256 --address-index 512 --stake-account-index 88 --stake-address-index 29 --passphrase 0ZYM4ND14Z",
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
        paymentAddressCommand.NetworkTag.Should().Be(expectedNetworkTag);
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
