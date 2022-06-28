using Cscli.ConsoleTool.Query;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class QueryAddressInfoCommandShould
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
        var command = new QueryAddressInfoCommand()
        {
            Network = network,
            Address = "stake1uxwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qg8t55dn",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option --network must be either testnet or mainnet");
    }

    [Theory]
    [InlineData("a", "testnet")]
    [InlineData("a", "mainnet")]
    [InlineData("test", "testnet")]
    [InlineData("test", "mainnet")]
    [InlineData("addr1abc1234", "testnet")]
    [InlineData("addr1abc1234", "mainnet")]
    [InlineData("addr_test1abc1234", "testnet")]
    [InlineData("addr_test1abc1234", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Address_Is_Not_Valid_Bech32(
        string invalidAddress, string network)
    {
        var command = new QueryAddressInfoCommand()
        {
            Network = network,
            Address = invalidAddress,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --address {invalidAddress} is invalid for {network}");
    }

    [Theory]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "testnet")]
    [InlineData("addr_test1qqg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jsnk5zw2", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Address_Is_Valid_But_For_Different_Network(
        string address, string network)
    {
        var command = new QueryAddressInfoCommand()
        {
            Network = network,
            Address = address,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --address {address} is invalid for {network}");
    }
}
