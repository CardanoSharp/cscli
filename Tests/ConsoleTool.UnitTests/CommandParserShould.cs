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
        "wallethd key root derive --mnemonic ARGS",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage, 
        "")]
    [InlineData(
        "wallethd key root derive --mnemonic ARGS --passphrase helloworld --language Spanish",
        "main ivory bring aim auction wheat credit horn picnic road horse never myth photo devote pen slice small wrestle play group two usage egg",
        "Spanish",
        "helloworld")]
    public void ParseArgs_Correctly_To_DeriveRootKeyCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase)
    {
        var args = flatArgs.Split(' ');
        // Replace ARGS with full mnemonic to mimic built-in argument binding to the args array
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "ARGS")
                args[i] = expectedMnemonic;
        }
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<DeriveRootKeyCommand>();
        ((DeriveRootKeyCommand)command).Mnemonic.Should().Be(expectedMnemonic);
        ((DeriveRootKeyCommand)command).Language.Should().Be(expectedLanguage);
        ((DeriveRootKeyCommand)command).Passphrase.Should().Be(expectedPassPhrase);
    }

    [Theory]
    [InlineData(
        "wallethd key payment derive --mnemonic ARGS",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0)]
    [InlineData(
        "wallethd key payment derive --mnemonic ARGS --passphrase helloworld --language Spanish --account-index 2 --address-index 256",
        "main ivory bring aim auction wheat credit horn picnic road horse never myth photo devote pen slice small wrestle play group two usage egg",
        "Spanish",
        "helloworld", 2, 256)]
    public void ParseArgs_Correctly_To_DerivePaymentKeyCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, int expectedAccountIndex, int expectedAddressIndex)
    {
        var args = flatArgs.Split(' ');
        // Replace ARGS with full mnemonic to mimic built-in argument binding to the args array
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "ARGS")
                args[i] = expectedMnemonic;
        }
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<DerivePaymentKeyCommand>();
        ((DerivePaymentKeyCommand)command).Mnemonic.Should().Be(expectedMnemonic);
        ((DerivePaymentKeyCommand)command).Language.Should().Be(expectedLanguage);
        ((DerivePaymentKeyCommand)command).Passphrase.Should().Be(expectedPassPhrase);
    }

    [Theory]
    [InlineData(
        "wallethd key stake derive --mnemonic ARGS",
        "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        Constants.DefaultMnemonicLanguage,
        "", 0, 0)]
    [InlineData(
        "wallethd key stake derive --mnemonic ARGS --passphrase helloworld --language Spanish --account-index 2 --address-index 256",
        "main ivory bring aim auction wheat credit horn picnic road horse never myth photo devote pen slice small wrestle play group two usage egg",
        "Spanish",
        "helloworld", 2, 256)]
    public void ParseArgs_Correctly_To_DeriveStakeKeyCommand_When_Options_Are_Valid(
        string flatArgs, string expectedMnemonic, string expectedLanguage, string expectedPassPhrase, int expectedAccountIndex, int expectedAddressIndex)
    {
        var args = flatArgs.Split(' ');
        // Replace ARGS with full mnemonic to mimic built-in argument binding to the args array
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "ARGS")
                args[i] = expectedMnemonic;
        }
        var command = CommandParser.ParseArgsToCommand(args);
        command.Should().BeOfType<DeriveStakeKeyCommand>();
        ((DeriveStakeKeyCommand)command).Mnemonic.Should().Be(expectedMnemonic);
        ((DeriveStakeKeyCommand)command).Language.Should().Be(expectedLanguage);
        ((DeriveStakeKeyCommand)command).Passphrase.Should().Be(expectedPassPhrase);
    }


}
