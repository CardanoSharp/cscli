using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DecodeBech32CommandShould
{
    [Theory]
    [InlineData(
        "addr1qx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3n0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgse35a3x",
        "019493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251")]
    [InlineData(
        "addr1z8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gten0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgs9yc0hh",
        "11c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251")]
    [InlineData(
        "addr1yx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerkr0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shs2z78ve",
        "219493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8ec37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    [InlineData(
        "addr1x8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gt7r0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shskhj42g",
        "31c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542fc37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    [InlineData("addr1gx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer5pnz75xxcrzqf96k",
        "419493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e8198bd431b03")]
    [InlineData("addr128phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtupnz75xxcrtw79hu",
        "51c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f8198bd431b03")]
    [InlineData("addr1vx2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzers66hrl8",
        "619493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e")]
    [InlineData("addr1w8phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcyjy7wx",
        "71c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    [InlineData("stake1uyehkck0lajq8gr28t9uxnuvgcqrc6070x3k9r8048z8y5gh6ffgw",
        "e1337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251")]
    [InlineData("stake178phkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcccycj5",
        "f1c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    [InlineData(
        "addr_test1qz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer3n0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgs68faae",
        "009493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251")]
    [InlineData(
        "addr_test1zrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gten0d3vllmyqwsx5wktcd8cc3sq835lu7drv2xwl2wywfgsxj90mg",
        "10c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251")]
    [InlineData(
        "addr_test1yz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerkr0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shsf5r8qx",
        "209493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8ec37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    [InlineData(
        "addr_test1xrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gt7r0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shs4p04xh",
        "30c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542fc37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    [InlineData("addr_test1gz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzer5pnz75xxcrdw5vky",
        "409493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e8198bd431b03")]
    [InlineData("addr_test12rphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtupnz75xxcryqrvmw",
        "50c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f8198bd431b03")]
    [InlineData("addr_test1vz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerspjrlsz",
        "609493315cd92eb5d8c4304e67b7e16ae36d61d34502694657811a2c8e")]
    [InlineData("addr_test1wrphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcl6szpr",
        "70c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    [InlineData("stake_test1uqehkck0lajq8gr28t9uxnuvgcqrc6070x3k9r8048z8y5gssrtvn",
        "e0337b62cfff6403a06a3acbc34f8c46003c69fe79a3628cefa9c47251")]
    [InlineData("stake_test17rphkx6acpnf78fuvxn0mkew3l0fd058hzquvz7w36x4gtcljw6kf",
        "f0c37b1b5dc0669f1d3c61a6fddb2e8fde96be87b881c60bce8e8d542f")]
    public async Task Execute_Successfully_With_Bech_Decoding_When_Properties_Are_Valid(string value, string expectedHex)
    {
        var command = new DecodeBech32Command()
        {
            Value = value
        };
        
        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedHex);
    }

    [Theory]
    [InlineData(
        "root_xsk12qpr53a6r7dpjpu2mr6zh96vp4whx2td4zccmplq3am6ph6z4dga6td8nph4qpcnlkdcjkd96p83t23mplvh2w42n6yc3urav8qgph3d9az6lc0px7xq7sau4r4dsfp9h0syfkhge8e6muhd69vz9j6fggdhgd4e",
        "50023a47ba1f9a19078ad8f42b974c0d5d73296da8b18d87e08f77a0df42ab51dd2da7986f500713fd9b8959a5d04f15aa3b0fd9753aaa9e8988f07d61c080de2d2f45afe1e1378c0f43bca8ead82425bbe044dae8c9f3adf2edd15822cb4942")]
    [InlineData(
        "addr_xsk1fzw9r482t0ekua7rcqewg3k8ju5d9run4juuehm2p24jtuzz4dg4wpeulnqhualvtx9lyy7u0h9pdjvmyhxdhzsyy49szs6y8c9zwfp0eqyrqyl290e6dr0q3fvngmsjn4aask9jjr6q34juh25hczw3euust0dw",
        "489c51d4ea5bf36e77c3c032e446c79728d28f93acb9ccdf6a0aab25f042ab5157073cfcc17e77ec598bf213dc7dca16c99b25ccdb8a04254b0143443e0a27242fc8083013ea2bf3a68de08a59346e129d7bd858b290f408d65cbaa97c09d1cf")]
    [InlineData(
        "stake_xsk1xr5c8423vymrfvrqz58wqqtpekg8cl2s7zvuedeass77emzz4dgs32nfp944ljxw86h7wkxcrut8gr8qmql8gvc9slc8nj9x47a6jtaqqxf9ywd4wfhrzv4c54vcjp827fytdzrxs3gdh5f0a0s7hcf8a5e4ay8g",
        "30e983d551613634b060150ee00161cd907c7d50f099ccb73d843decec42ab5108aa69096b5fc8ce3eafe758d81f16740ce0d83e74330587f079c8a6afbba92fa001925239b5726e3132b8a5598904eaf248b688668450dbd12febe1ebe127ed")]
    public async Task Execute_Successfully_With_Bech32_Decoding_When_Properties_Are_Valid(string value, string expectedHex)
    {
        var command = new DecodeBech32Command()
        {
            Value = value
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedHex);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("this is not a valid address")]
    [InlineData("addr_fake1yz2fxv2umyhttkxyxp8x0dlpdt3k6cwng5pxj3jhsydzerkr0vd4msrxnuwnccdxlhdjar77j6lg0wypcc9uar5d2shsf5r8qx")]
    public async Task Fail_With_Bech_Decoding_When_Properities_Are_Invalid(string value)
    {
        var command = new DecodeBech32Command()
        {
            Value = value
        };
        
        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
    }
}