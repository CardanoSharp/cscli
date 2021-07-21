using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests
{
    public class GenerateMnemonicCommandShould
    {
        [Fact]
        public async Task Execute_Successfully_With_Default_Options_When_No_Properties_Are_Initialised()
        {
            var command = new GenerateMnemonicCommand();

            var executionResult = await command.ExecuteAsync(CancellationToken.None);

            command.Size.Should().Be(GenerateMnemonicCommand.DefaultSize);
            command.Language.Should().Be(GenerateMnemonicCommand.DefaultLanguage);
            executionResult.Outcome.Should().Be(CommandOutcome.Success);
            executionResult.Result.Split(' ').Length.Should().Be(GenerateMnemonicCommand.DefaultSize);
        }

        [Theory]
        [InlineData(9, "English")]
        [InlineData(12, "English")]
        [InlineData(15, "English")]
        [InlineData(18, "English")]
        [InlineData(21, "English")]
        [InlineData(24, "English")]
        public async Task Execute_Successfully_With_Mnemonics_When_Properties_Are_Valid(int size, string language)
        {
            var command = new GenerateMnemonicCommand()
            {
                Size = size,
                Language = language
            };

            var executionResult = await command.ExecuteAsync(CancellationToken.None);

            executionResult.Outcome.Should().Be(CommandOutcome.Success);
            executionResult.Result.Split(' ').Length.Should().Be(size);
        }

        [Theory]
        [InlineData(0, "English")]
        [InlineData(1, "English")]
        [InlineData(8, "English")]
        [InlineData(14, "English")]
        [InlineData(27, "English")]
        [InlineData(9, "x")]
        [InlineData(12, "en-US")]
        [InlineData(15, "Australian")]
        [InlineData(18, "British")]
        [InlineData(21, "!")]
        [InlineData(24, "uwotm8")]
        public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Properties_Are_Invalid(int size, string language)
        {
            var command = new GenerateMnemonicCommand()
            {
                Size = size,
                Language = language
            };

            var executionResult = await command.ExecuteAsync(CancellationToken.None);

            executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
            executionResult.Result.Should().StartWith("Invalid option");
        }
    }
}
