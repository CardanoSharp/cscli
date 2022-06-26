using Cscli.ConsoleTool.Transaction;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class BuildSimplePaymentTransactionCommandShould
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
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = networkTag,
            From = "addr1q9uuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aa2upvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdstd8nlr",
            To = "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f",
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option --network must be either testnet or mainnet");
    }

    [Theory]
    [InlineData("testnet", null, "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("testnet", "", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("testnet", "  ", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("mainnet", null, "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("mainnet", "", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("mainnet", " ", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_From_Address_Is_Null_Or_Empty(
        string networkTag, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = networkTag,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --from address is required");
    }

    [Theory]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", null)]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "")]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "  ")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", null)]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", " ")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_To_Address_Is_Null_Or_Empty(
        string networkTag, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = networkTag,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --to address is required");
    }
}
