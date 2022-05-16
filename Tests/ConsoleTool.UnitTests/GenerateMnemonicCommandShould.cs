using Cscli.ConsoleTool.Wallet;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class GenerateMnemonicCommandShould
{
    [Fact]
    public async Task Execute_Successfully_With_Default_Options_When_No_Properties_Are_Initialised()
    {
        var command = new GenerateMnemonicCommand();

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        command.Size.Should().Be(Constants.DefaultMnemonicCount);
        command.Language.Should().Be(Constants.DefaultMnemonicLanguage);
        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Split(' ').Length.Should().Be(Constants.DefaultMnemonicCount);
    }

    [Theory]
    [InlineData(9, "English")]
    [InlineData(12, "English")]
    [InlineData(15, "English")]
    [InlineData(18, "English")]
    [InlineData(21, "English")]
    [InlineData(24, "English")]
    #region Non-English 
    //[InlineData(9, "Spanish")]
    [InlineData(12, "Spanish")]
    [InlineData(15, "Spanish")]
    [InlineData(18, "Spanish")]
    [InlineData(21, "Spanish")]
    [InlineData(24, "Spanish")]
    //[InlineData(9, "Portuguese")]
    [InlineData(12, "Portuguese")]
    [InlineData(15, "Portuguese")]
    [InlineData(18, "Portuguese")]
    [InlineData(21, "Portuguese")]
    [InlineData(24, "Portuguese")]
    //[InlineData(9, "Japanese")]
    [InlineData(12, "Japanese")]
    [InlineData(15, "Japanese")]
    [InlineData(18, "Japanese")]
    [InlineData(21, "Japanese")]
    [InlineData(24, "Japanese")]
    //[InlineData(9, "ChineseTraditional")]
    [InlineData(12, "ChineseTraditional")]
    [InlineData(15, "ChineseTraditional")]
    [InlineData(18, "ChineseTraditional")]
    [InlineData(21, "ChineseTraditional")]
    [InlineData(24, "ChineseTraditional")]
    #endregion
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
    [InlineData(-1, "")]
    [InlineData(0, null)]
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
