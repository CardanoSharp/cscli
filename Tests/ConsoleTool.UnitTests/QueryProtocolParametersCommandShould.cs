using Cscli.ConsoleTool.Query;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class QueryProtocolParametersCommandShould
{
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("test")]
    [InlineData("tastnet")]
    [InlineData("Mainet")]
    [InlineData("mainet")]
    [InlineData("mainnetwork")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Network_Is_Not_Valid(
        string network)
    {
        var command = new QueryProtocolParametersCommand()
        {
            Network = network,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option --network must be either testnet or mainnet");
    }
}
