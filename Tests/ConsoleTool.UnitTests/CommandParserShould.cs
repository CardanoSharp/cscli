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
    [InlineData("walethd", "mnemonic", "gen")]
    public void ParseArgs_To_ShowInvalidArgumentCommand_When_Arguments_Are_Invalid(params string[] args)
    {
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<ShowInvalidArgumentCommand>();
    }

    [Theory]
    [InlineData("walethd", "mnemonic", "gen")]
    [InlineData("wallethd", "mnuemonic", "gen")]
    [InlineData("wallethd", "mnemonic", "create")]
    public void ParseArgs_To_ShowInvalidArgumentCommand_When_Arguments_Have_Spelling_Mistakes(params string[] args)
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
    [InlineData("wallethd mnemonic gen", Constants.DefaultMnemonicCount, Constants.DefaultMnemonicLanguage)]
    [InlineData("wallethd mnemonic gen --language Spanish", Constants.DefaultMnemonicCount, "Spanish")]
    [InlineData("wallethd mnemonic gen --size 15", 15, Constants.DefaultMnemonicLanguage)]
    [InlineData("wallethd mnemonic gen --size 21 --language English", 21, "English")]
    [InlineData("wallethd mnemonic gen --size 21 --language Spanish", 21, "Spanish")]
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
        "wallethd key root derive --mnemonic {MNEMONIC}",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage, 
        "")]
    [InlineData(
        "wallethd key root derive --mnemonic {MNEMONIC} --passphrase helloworld --language Spanish",
        "main ivory bring aim auction wheat credit horn picnic road horse never myth photo devote pen slice small wrestle play group two usage egg",
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
        "wallethd key payment derive --mnemonic {MNEMONIC}",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0)]
    [InlineData(
        "wallethd key payment derive --mnemonic {MNEMONIC} --passphrase helloworld --language Spanish --account-index 2 --address-index 256",
        "main ivory bring aim auction wheat credit horn picnic road horse never myth photo devote pen slice small wrestle play group two usage egg",
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
        "wallethd key stake derive --mnemonic {MNEMONIC}",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0)]
    [InlineData(
        "wallethd key stake derive --mnemonic {MNEMONIC} --passphrase helloworld --language Spanish --account-index 2 --address-index 256",
        "main ivory bring aim auction wheat credit horn picnic road horse never myth photo devote pen slice small wrestle play group two usage egg",
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
        "wallethd address stake derive --mnemonic {MNEMONIC} --network-tag Testnet",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0, "Testnet")]
    [InlineData(
        "wallethd address stake derive --mnemonic {MNEMONIC} --passphrase helloworld --language Spanish --network-tag Mainnet --account-index 101 --address-index 980",
        "vena pomo bolero papel colina paleta regalo alma dibujo examen lindo programa venir bozal elogio tacto romper observar bono separar refrán ecuador clase reducir",
        "Spanish",
        "helloworld", 101, 980, "Mainnet")]
    [InlineData(
        "wallethd address stake derive --mnemonic {MNEMONIC} --passphrase 0ZYM4ND14Z --language Japanese --network-tag Mainnet --account-index 2147483647 --address-index 2147483647",
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
        "wallethd address payment derive --mnemonic {MNEMONIC} --network-tag Testnet --payment-address-type Enterprise",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0, 0, 0, "Testnet", "Enterprise")]
    [InlineData(
        "wallethd address payment derive --language Portugese --mnemonic {MNEMONIC} --network-tag Mainnet --payment-address-type Base --account-index 256 --address-index 512 --stake-account-index 88 --stake-address-index 29 --passphrase 0ZYM4ND14Z",
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
