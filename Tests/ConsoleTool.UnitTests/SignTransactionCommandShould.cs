using Cscli.ConsoleTool.Transaction;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class SignTransactionCommandShould
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Signing_Keys_Is_Not_Supplied(
        string invalidSigningKeys)
    {
        var command = new SignTransactionCommand()
        {
            SigningKeys = invalidSigningKeys,
            CborHex = "84a400818258205853cc86af075cc547a5a20658af54841f37a5832532a704c583ed4f010184a501018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a43e83ed4021a00028a9d031a03b00ee0a0f5f6",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option --signing-keys is required");
    }

    [Theory]
    [InlineData("", "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData(null, "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("", "addr_xsk15pt6dccwyy2jjgmv9gxszjyzc6kchhas2dr9q6ky0xpwz4679e2t5qu0yehpmg7xr3sc3wkcyagujlk2euwl0007w68wcfdm7ajl2tzkqve7vmqds5sf38syns3u2wey05yeh70p9m5n05kftku30uqjlqy0gjh2,addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData(" ", "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("  ", "addr_xsk15pt6dccwyy2jjgmv9gxszjyzc6kchhas2dr9q6ky0xpwz4679e2t5qu0yehpmg7xr3sc3wkcyagujlk2euwl0007w68wcfdm7ajl2tzkqve7vmqds5sf38syns3u2wey05yeh70p9m5n05kftku30uqjlqy0gjh2")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_CborHex_Is_Null_Or_Whitespace(
        string invalidCborHex, string signingKeys)
    {
        var command = new SignTransactionCommand()
        {
            SigningKeys = signingKeys,
            CborHex = invalidCborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --cbor-hex is required");
    }

    [Theory]
    [InlineData("0", "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("0", "addr_xsk15pt6dccwyy2jjgmv9gxszjyzc6kchhas2dr9q6ky0xpwz4679e2t5qu0yehpmg7xr3sc3wkcyagujlk2euwl0007w68wcfdm7ajl2tzkqve7vmqds5sf38syns3u2wey05yeh70p9m5n05kftku30uqjlqy0gjh2,addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("fff", "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("fff", "addr_xsk15pt6dccwyy2jjgmv9gxszjyzc6kchhas2dr9q6ky0xpwz4679e2t5qu0yehpmg7xr3sc3wkcyagujlk2euwl0007w68wcfdm7ajl2tzkqve7vmqds5sf38syns3u2wey05yeh70p9m5n05kftku30uqjlqy0gjh2,addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "addr_xsk15pt6dccwyy2jjgmv9gxszjyzc6kchhas2dr9q6ky0xpwz4679e2t5qu0yehpmg7xr3sc3wkcyagujlk2euwl0007w68wcfdm7ajl2tzkqve7vmqds5sf38syns3u2wey05yeh70p9m5n05kftku30uqjlqy0gjh2,addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw", "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    [InlineData("stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw", "addr_xsk15pt6dccwyy2jjgmv9gxszjyzc6kchhas2dr9q6ky0xpwz4679e2t5qu0yehpmg7xr3sc3wkcyagujlk2euwl0007w68wcfdm7ajl2tzkqve7vmqds5sf38syns3u2wey05yeh70p9m5n05kftku30uqjlqy0gjh2,addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_CborHex_Is_Not_Valid_Hexadecimal(
        string invalidCborHex, string signingKeys)
    {
        var command = new SignTransactionCommand()
        {
            SigningKeys = signingKeys,
            CborHex = invalidCborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --cbor-hex {invalidCborHex} is not in hexadecimal format");
    }

    [Theory]
    [InlineData("84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da0f582a11902a2a1636d73676d74687820666f72206c756e636880",
        "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw", 
        "84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da10081825820de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa5840d856197bc4f4cb62439ea9c2781e9764855685c3809364ef759b1926047d7bb326fecf2ee1144c5d49cf2f53feb432fa1af30e00d8d69c4145e6494fd1979a0cf582a11902a2a1636d73676d74687820666f72206c756e636880")]
    public async Task Execute_Successfully_With_Correct_Signed_Cbor_When_Options_Are_Valid(string cborHex, string signingKeys, string expectedCborHex)
    {
        var command = new SignTransactionCommand()
        {
            SigningKeys = signingKeys,
            CborHex = cborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedCborHex);
    }
}
