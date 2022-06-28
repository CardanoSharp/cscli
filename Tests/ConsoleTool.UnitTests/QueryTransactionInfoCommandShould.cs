using Cscli.ConsoleTool.Query;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class QueryTransactionInfoCommandShould
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
        var command = new QueryTransactionInfoCommand()
        {
            Network = network,
            TxId = "421734e5c8e5be24d788da2defeb9005516c20eb7d3ddef2140e836d20282a2a",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option --network must be either testnet or mainnet");
    }

    [Theory]
    [InlineData(null, "testnet")]
    [InlineData(null, "mainnet")]
    [InlineData("", "testnet")]
    [InlineData("", "mainnet")]
    [InlineData(" ", "testnet")]
    [InlineData(" ", "mainnet")]
    [InlineData("     ", "testnet")]
    [InlineData("     ", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_TxId_Is_Null_Or_Whitespace(
        string nullOrWhitespaceTxId, string network)
    {
        var command = new QueryTransactionInfoCommand()
        {
            Network = network,
            TxId = nullOrWhitespaceTxId,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --tx-id is required");
    }
}
