using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DeriveStakeAddressCommandShould
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("", "invalid")]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("english", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData("English", "")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Properties_Are_Invalid(
        string language, string mnemonic)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option");
    }

    [Theory]
    [InlineData("Spanish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData("Spanish", "shoe")]
    [InlineData("English", "brusco kilo trabajo rodar parque muela zorro boti´n minero")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Wrong_Word_List_Language_Is_Used(
        string language, string mnemonic)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
    }

    [Theory]
    [InlineData(
        "English", "", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        "addr_xsk1sqltfrmwph2x9qarf9h3dtnpeuyavq34d2y2hqd5qfq2hdk5sa8jyjj360azt08e30zx5sh5025syp7l5ah9jsjxk808p9lgn2ag99ttp849evwpy8sdctu4sx2vc8sftdt4g8kfrgdxvrm2f6ul0l3l4uycfw8y")]
    [InlineData(
        "English", "", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame",
        "addr_xsk1uz8nprlx2vk5et594g4k9lu2c9lc7rl2hqml5dkxrmwrrjgzrdrc7s6q4t35ynwzjylcnl2wjr4hcg0te3mpu8jfpks7he6ly4fw3tl7crlckxtjphnuyl4tf9l6dlh84jzhejxm9vdh3drapdxapsh4cu4rjwrd")]
    [InlineData(
        "English", "", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird",
        "addr_xsk17qemn6k0cq0hm28f9r6xpjw459eae7ey07pn28p3akpnz6rv0ex2qavdnnqvmdyk9palrhjf5u5xk53k52zmsn9qe3au0my3f69ynk8gca680qzhq8zl5tmnaa0ysfuucejzmynj5xvcw7pmqmszfnfw6gv88mpw")]
    [InlineData(
        "English", "", "gap motor liberty title also carry aerobic confirm beef become dawn angry boring coconut program daughter flag ripple fashion peace balance cool purchase rocket",
        "addr_xsk1nrpt2gjva3syj7c0wacc00zqhgzw44g24w903xr278krgpqksfr2vd2ngydhvf0e3nxpyg9ed9a7j55e94s2n5tqm2rnmvktwjgzrvvymu0mj2nr05drwn5tw90k7wd65p8yghj76dhren2as92fed5j2q9rgz3c")]
    public async Task Execute_Successfully_With_Correct_Bech32_Extended_Root_Key_When_Mandatory_Properties_Are_Valid(
        string language, string passphrase, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passphrase
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }
}