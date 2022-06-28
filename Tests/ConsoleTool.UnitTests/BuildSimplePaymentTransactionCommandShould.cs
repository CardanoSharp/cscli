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
        string network)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
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
        string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --from address is required");
    }

    [Theory]
    [InlineData("testnet", "addr_test1vq5z", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4u", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("mainnet", "addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6x", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9i", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_From_Address_Is_Not_Valid_Bech32(
        string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --from address is not valid");
    }

    [Theory]
    [InlineData("testnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("testnet", "stake_test1uztkvps54v3yrwvxhvfz9uph8g6e2zd8jcg2cyss45g7xqclj4scq", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("testnet", "addr_xsk1lplvzxsmads66xa9n3869v8jjuyl7zs0pprtrjvuh277mu6z4dgcess2snu9tr26xqsf5cnvafm2xaah43tzrfqpayk085az37dtx0jl6h6v4p75zahdvxj32gy39s0y230u4pw7gc63ayj3aweaqvuj4cdhqk8g", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("testnet", "policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("mainnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("mainnet", "addr_xsk1lplvzxsmads66xa9n3869v8jjuyl7zs0pprtrjvuh277mu6z4dgcess2snu9tr26xqsf5cnvafm2xaah43tzrfqpayk085az37dtx0jl6h6v4p75zahdvxj32gy39s0y230u4pw7gc63ayj3aweaqvuj4cdhqk8g", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("mainnet", "policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_From_Address_Is_Not_A_Valid_Address(
        string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --from address is not valid for the network {network}");
    }

    [Theory]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", null)]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "")]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "  ")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", null)]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", " ")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_To_Address_Is_Null_Or_Empty(
        string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --to address is required");
    }

    [Theory]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "addr_test1vq5z")]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4u")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6x")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9i")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_To_Address_Is_Not_Valid_Bech32(
        string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --to address is not valid");
    }

    [Theory]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "stake_test1uztkvps54v3yrwvxhvfz9uph8g6e2zd8jcg2cyss45g7xqclj4scq")]
    [InlineData("testnet", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t", "addr_xsk1lplvzxsmads66xa9n3869v8jjuyl7zs0pprtrjvuh277mu6z4dgcess2snu9tr26xqsf5cnvafm2xaah43tzrfqpayk085az37dtx0jl6h6v4p75zahdvxj32gy39s0y230u4pw7gc63ayj3aweaqvuj4cdhqk8g")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj")]
    [InlineData("mainnet", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f", "policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_To_Address_Is_Not_A_Valid_Address(
        string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = 80,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --to address is not valid for the network {network}");
    }

    [Theory]
    [InlineData("testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_No_Payment_Option_Is_Provided(
        string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid options either (--lovelaces | --ada | --send-all) must be supplied");
    }

    [Theory]
    [InlineData(0, 2500000, true)]
    [InlineData(1, 1000000, false)]
    [InlineData(60, 0, true)]
    [InlineData(1, 1000000, true)]
    [InlineData(25, 3456789, false)]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Multiple_Payment_Options_Are_Provided(
        uint ada, ulong lovelaces, bool sendAll)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = "mainnet",
            From = "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h",
            To = "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f",
            Ada = ada,
            Lovelaces = lovelaces,
            SendAll = sendAll
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid options only one of (--lovelaces | --ada | --send-all) must be supplied");
    }

    [Theory]
    [InlineData(0.1, "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData(0.245678, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData(0.999999, "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData(0.000009, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData(-0.5, "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData(-0.1, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData(-12.865421, "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData(-99.9, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Making_Ada_Payment_Is_Less_Than_One(
        decimal ada, string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = ada,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --ada value must be at least 1");
    }

    [Theory]
    [InlineData(100000, "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData(245678, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData(999999, "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData(9, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Making_Lovelace_Payment_Is_Less_Than_One_Million(
        ulong lovelaces, string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Lovelaces = lovelaces,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --lovelaces value must be at least 1000000");
    }

    [Theory]
    [InlineData("a", "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("addr_xsk1abcdefg", "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dq", "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("100", "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Signing_Key_Is_Provided_But_Not_Valid_Bech32(
        string signingKey, string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            SigningKey = signingKey,
            Ada = 65,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --signing-key is not a valid signing key");
    }

    [Theory]
    [InlineData("policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02", "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g", "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData("addr_xvk1m62sxsn8t8apscjx2l6mejfj7wpzpmy7e6ex9yru4uk3nzmwp74zljqgxqf752ln56x7pzjex3hp98tmmpvt9y85prt9ew4f0syarncveq5jl", "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData("root_xsk12qpr53a6r7dpjpu2mr6zh96vp4whx2td4zccmplq3am6ph6z4dga6td8nph4qpcnlkdcjkd96p83t23mplvh2w42n6yc3urav8qgph3d9az6lc0px7xq7sau4r4dsfp9h0syfkhge8e6muhd69vz9j6fggdhgd4e", "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Signing_Key_Is_Provided_But_Not_A_Valid_Payment_Signing_Key(
        string signingKey, string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            SigningKey = signingKey,
            Ada = 65,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --signing-key is not a valid payment signing key");
    }

    [Theory]
    [InlineData(1, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    [InlineData(58318, "testnet", "addr_test1qq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqz8fquadv00d7t7a88rlf6z2knwfesls5f2cndan7runlcsad62ju", "addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t")]
    [InlineData(4492799, "mainnet", "addr1qy3y89nnzdqs4fmqv49fmpqw24hjheen3ce7tch082hh6x7nwwgg06dngunf9ea4rd7mu9084sd3km6z56rqd7e04ylslhzn9h", "addr1qxjkvymhhj0854kkaawxm9etyz8aq8rhuhsyhhemhh2tv0jupvkzyt42349mnkhgu8ghqzgtsqvzmvu2w675560fvvdsxa386f")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Ttl_Is_Before_Shelley_Launch(
        uint ttl, string network, string fromAddress, string toAddress)
    {
        var command = new BuildSimplePaymentTransactionCommand()
        {
            Network = network,
            From = fromAddress,
            To = toAddress,
            Ada = 65,
            Ttl = ttl,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --ttl slot cannot occur before the Shelley HFC event");
    }
}
