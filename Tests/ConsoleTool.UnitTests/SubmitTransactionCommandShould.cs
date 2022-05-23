using Cscli.ConsoleTool.Transaction;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class SubmitTransactionCommandShould
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
        string networkTag)
    {
        var command = new SubmitTransactionCommand()
        {
            Network = networkTag,
            CborHex = "83a500818258205f61dde2c9c05c104f81194f2cfc738b4463f098e46fa1ca62aedb1345561624000182825839005a99cb175eb944462d6bfd29d06e0a69defc091d8e5ecab740afac6f1922fcdeeb6df8592d78b20c6e22fdb73fa9446aad05626d78000b7f181982583900ae34b08e5a409e1e66683d0b04ff33bb7c5c1455046ad66ebe2b66d157664134527cd7cb875caa34251f7e063b337cd705649637c35df6c71bfffffffffffd1e5e021a0002e1ed031a037c8bb70758205fbac14ec74ffff7a1aa1b1acbf4bfed1ce774bf21a620092f647a04553b65d5a1008182582008c77407d35dab5e3f53ef4ad5066284ca269c8107e88de04fcdfb875de9a7055840d7250f2b39c8d3014aaa8fe6ba60f0b52a27240a6b8ae7f72d64079a4852acb4ecffb3559a5fb829d9868f0cd9806e567bc710f73513ba6eb9a699f71bb1800182a1183da1646e616d65783754657374202d204669786564206e6577206172726179207673206e6577206d617020666f7220656d707479207769746e6573732073657480",
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
    [InlineData("  ", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_CborHex_Is_Null_Or_Whitespace(
        string invalidCborHex, string network)
    {
        var command = new SubmitTransactionCommand()
        {
            Network = network,
            CborHex = invalidCborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --cbor-hex is required");
    }

    [Theory]
    [InlineData("fff", "testnet")]
    [InlineData("fff", "mainnet")]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "testnet")]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "mainnet")]
    [InlineData("stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw", "testnet")]
    [InlineData("stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_CborHex_Is_Not_Valid_Hexadecimal(
        string invalidCborHex, string network)
    {
        var command = new SubmitTransactionCommand()
        {
            Network = network,
            CborHex = invalidCborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --cbor-hex {invalidCborHex} is not in hexadecimal format");
    }
}
