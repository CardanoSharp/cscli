using Cscli.ConsoleTool.Query;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class QueryAccountAssetCommandShould
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
        var command = new QueryAccountAssetCommand()
        {
            Network = network,
            StakeAddress = "stake1uxwlj9umavxw38uyqf0q9ts3cx9nqql8dyq5nj4xvta06qg8t55dn",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option --network must be either testnet or mainnet");
    }

    [Theory]
    [InlineData("testnet")]
    [InlineData("mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Stake_Address_And_Payment_Address_Are_Both_Not_Supplied(
        string network)
    {
        var command = new QueryAccountAssetCommand()
        {
            Network = network
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option, one of either --stake-address or --address is required");
    }

    [Theory]
    [InlineData("testnet")]
    [InlineData("mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Valid_Stake_Address_And_Payment_Address_Are_Both_Supplied(
        string network)
    {
        var command = new QueryAccountAssetCommand()
        {
            Network = network,
            StakeAddress = "stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj",
            Address = "addr1qy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqzupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdspma4ht"
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option, one of either --stake-address or --address is required");
    }

    [Theory]
    [InlineData("a", "testnet")]
    [InlineData("a", "mainnet")]
    [InlineData("test", "testnet")]
    [InlineData("test", "mainnet")]
    [InlineData("stake1abc1234", "testnet")]
    [InlineData("stake1abc1234", "mainnet")]
    [InlineData("stake_test1abc1234", "testnet")]
    [InlineData("stake_test1abc1234", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Stake_Address_Is_Not_Valid_Bech32(
        string invalidStakeAddress, string network)
    {
        var command = new QueryAccountAssetCommand()
        {
            Network = network,
            StakeAddress = invalidStakeAddress,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --stake-address {invalidStakeAddress} is invalid");
    }

    [Theory]
    [InlineData("stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw", "testnet")]
    [InlineData("stake_test1uqneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egft0emn", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Stake_Address_Is_Valid_But_For_Different_Network(
        string stakeAddress, string network)
    {
        var command = new QueryAccountAssetCommand()
        {
            Network = network,
            StakeAddress = stakeAddress,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --stake-address {stakeAddress} is invalid for {network}");
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
        var command = new QueryAccountAssetCommand()
        {
            Network = network,
            Address = invalidAddress,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --address {invalidAddress} is not a base address with attached staking credentials");
    }

    [Theory]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "testnet")]
    [InlineData("addr_test1qqg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jsnk5zw2", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Address_Is_Valid_But_For_Different_Network(
        string address, string network)
    {
        var command = new QueryAccountAssetCommand()
        {
            Network = network,
            Address = address,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --address {address} is not a base address with attached staking credentials for {network}");
    }
}
