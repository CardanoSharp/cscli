using Cscli.ConsoleTool.Wallet;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class ConvertVerificationKeyCommandShould
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Supplied(
        string invalidSigningKey)
    {
        var command = new ConvertVerificationKeyCommand()
        {
            SigningKey = invalidSigningKey
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --sigining-key is required");
    }

    [Theory]
    [InlineData("KxFC1jmwwCoACiCAWZ3eXa96mBM6tb3TYzGmf6YwgdGWZgawvrtJ")]
    [InlineData("1E99423A4ED27608A15A2616A2B0E9E52CED330AC530EDCC32C8FFC6A526AEDD")]
    [InlineData("0x983110309620D911731Ac0932219af06091b6744")]
    [InlineData("some_sk1aaa3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02")]
    [InlineData("addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust000")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_SigningKey_Is_Not_Valid_Bech32(
        string invalidSigningKey)
    {
        var command = new ConvertVerificationKeyCommand()
        {
            SigningKey = invalidSigningKey
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --sigining-key is not in bech32 format - please see https://cips.cardano.org/cips/cip5/");
    }

    [Theory]
    [InlineData("addr_test1qpvttg5263dnutj749k5dcr35yk5mr94fxx0q2zs2xeuxq5hvcrpf2ezgxucdwcjytcrww34j5y609ss4sfpptg3uvpsxmcdtf", "addr_test")]
    [InlineData("addr1vy5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqrukl6w", "addr")]
    [InlineData("stake_test1uztkvps54v3yrwvxhvfz9uph8g6e2zd8jcg2cyss45g7xqclj4scq", "stake_test")]
    [InlineData("stake1u9wqktpz964g6jaemt5wr5tspy9cqxpdkw98d022d85kxxc2n2yxj", "stake")]
    [InlineData("addr_xvk1m62sxsn8t8apscjx2l6mejfj7wpzpmy7e6ex9yru4uk3nzmwp74zljqgxqf752ln56x7pzjex3hp98tmmpvt9y85prt9ew4f0syarncveq5jl", "addr_xvk")]
    [InlineData("policy_vk17s29wgt93lj3m830qhlpx8rx5shwmtljhdswdjyjetprhu5yaahqwzt9lc", "policy_vk")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_SigningKey_Is_Not_Supported(
        string invalidSigningKey, string derivedPrefix)
    {
        var command = new ConvertVerificationKeyCommand()
        {
            SigningKey = invalidSigningKey
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --sigining-key with prefix '{derivedPrefix}' is not supported");
    }

    [Theory]
    [InlineData("addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw", "addr_xvk1m62sxsn8t8apscjx2l6mejfj7wpzpmy7e6ex9yru4uk3nzmwp74zljqgxqf752ln56x7pzjex3hp98tmmpvt9y85prt9ew4f0syarncveq5jl")]
    [InlineData("stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g", "stake_xvk1r0v9a3ca9kxwqxqp8qcsnqa2llaytp2gdkc4w67rskc2udg9vtn2qqvj2gum2unwxyet3f2e3yzw4ujgk6yxdpzsm0gjl6lpa0sj0mg4tq9sj")]
    [InlineData("policy_sk1trt3shjrd4gy70q4m2ejgjgsdzwej4whc4r2trrcwedlpm6z4dglxl4nycrd8fptxrkye3tl3q29euxlqj7zndk9cfg4tskqlnp90uqwjqz02", "policy_vk17s29wgt93lj3m830qhlpx8rx5shwmtljhdswdjyjetprhu5yaahqwzt9lc")]
    public async Task Execute_Successfully_With_Correct_Public_Verification_Key_When_Signing_Key_Is_Supported(
        string signingKey, string expectedVerificationKey)
    {
        var command = new ConvertVerificationKeyCommand()
        {
            SigningKey = signingKey
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedVerificationKey);
    }
}
