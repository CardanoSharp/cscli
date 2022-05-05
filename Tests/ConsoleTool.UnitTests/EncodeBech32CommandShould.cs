using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class EncodeBech32CommandShould
{
    [Theory]
    [InlineData(
        "019493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251",
        "addr",
        "addr1qx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3n0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgse35a3x")]
    [InlineData(
        "11c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251",
        "addr",
        "addr1z8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gten0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgs9yc0hh")]
    [InlineData(
        "219493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8ec37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "addr",
        "addr1yx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerkr0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shs2z78ve")]
    [InlineData(
        "31c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542fc37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "addr",
        "addr1x8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gt7r0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shskhj42g")]
    [InlineData(
        "419493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e8198bd431b03",
        "addr",
        "addr1gx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer5pnz75xxcrzqf96k")]
    [InlineData(
        "51c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f8198bd431b03",
        "addr",
        "addr128phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtupnz75xxcrtw79hu")]
    [InlineData(
        "619493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e",
        "addr",
        "addr1vx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzers66hrl8")]
    [InlineData(
        "71c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "addr",
        "addr1w8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcyjy7wx")]
    [InlineData(
        "e1337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251",
        "stake",
        "stake1uyehkck0lajq8gr28t9uxnuvgcqrc6070x3k9r8048z8y5gh6ffgw")]
    [InlineData(
        "f1c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "stake",
        "stake178phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcccycj5")]
    [InlineData(
        "009493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251",
        "addr_test",
        "addr_test1qz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3n0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgs68faae")]
    [InlineData(
        "10c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251",
        "addr_test",
        "addr_test1zrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gten0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgsxj90mg")]
    [InlineData(
        "209493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8ec37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "addr_test",
        "addr_test1yz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerkr0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shsf5r8qx")]
    [InlineData(
        "30c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542fc37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "addr_test",
        "addr_test1xrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gt7r0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shs4p04xh")]
    [InlineData(
        "409493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e8198bd431b03",
        "addr_test",
        "addr_test1gz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer5pnz75xxcrdw5vky")]
    [InlineData(
        "50c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f8198bd431b03",
        "addr_test",
        "addr_test12rphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtupnz75xxcryqrvmw")]
    [InlineData(
        "609493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e",
        "addr_test",
        "addr_test1vz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerspjrlsz")]
    [InlineData(
        "70c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "addr_test",
        "addr_test1wrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcl6szpr")]
    [InlineData(
        "e0337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251",
        "stake_test",
        "stake_test1uqehkck0lajq8gr28t9uxnuvgcqrc6070x3k9r8048z8y5gssrtvn")]
    [InlineData(
        "f0c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f",
        "stake_test",
        "stake_test17rphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcljw6kf")]
    public async Task Execute_Successfully_With_Bech_Decoding_When_Properties_Are_Valid(string value, string prefix, string expectedBech32)
    {
        var command = new EncodeBech32Command()
        {
            Value = value,
            Prefix = prefix,
        };
        
        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("this is not a valid hex value")]
    [InlineData(" another invalid value")]
    public async Task Fail_With_Bech_Encoding_When_Properities_Are_Invalid(string invalidValue)
    {
        var command = new EncodeBech32Command()
        {
            Value = invalidValue
        };
        
        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
    }
}