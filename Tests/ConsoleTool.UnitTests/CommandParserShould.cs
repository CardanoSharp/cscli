using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests
{
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
        [InlineData("wallethd", "mnemonic", "gen")]
        [InlineData("wallethd", "mnemonic", "gen", "--size", "10", "--language", "English")]
        public void ParseArgs_Correctly_To_GenerateMnemonicCommand_When_Options_Are_Valid(params string[] args)
        {
            var command = CommandParser.ParseArgsToCommand(args);
            command.Should().BeOfType<GenerateMnemonicCommand>();
        }
    }
}
