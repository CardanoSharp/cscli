using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DerivePaymentKeyCommandShould
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("", "invalid")]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("english", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData("English", "")]
    [InlineData("Spanish", "shoe")]
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
        "addr_sk13qf2w4jsex0a27xjpwvv68jlk6vhlfql6la88wv495x9mfx5sa8h2xx5wxzx6ghgg504nqkaw55cev37xer2mpwcast76zk98zt7c35eagfuj4wnzv3pgmzxlfhucqg03s4c8ha6jg8j6y2h6ghdplpelqjtss3r")]
    [InlineData(
        "English", "", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame",
        "addr_sk1crr05lc9w35hdmz4qrgzv2895jvksqsk5lk58zt8l4kvntczrdr5we87q8mg0pfsefey2wsejpjp8hh94c8y3l976uha5qlmcsezpc7r975hd5vg30jfuu0g3qhux6ynjw7t5mamg57tkelpwukzcyrxty7s0qs7")]
    [InlineData(
        "English", "", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird",
        "addr_sk1rrqsavyh9ktz599kgd8jwmvmnhh2wavp6swqs7nh5vw6zktv0exg6h8quvuemwwcd3tr9pr3jmyh6ap0rqsn8dmafrl5u4p35vw50pe2r43t0yujkcarqygam5j0c3yjag0m0k2hxpuwd2eage9nhaduwct9dgnx")]
    [InlineData(
        "English", "", "gap motor liberty title also carry aerobic confirm beef become dawn angry boring coconut program daughter flag ripple fashion peace balance cool purchase rocket",
        "addr_sk17rhsqm2xsu3pyulu74rufpfpmcknn0j9v5fkqn55svz8nuq4sfrqfu5zhz8pgpq6r8gqye8g7m20y9y53pwt6af9nxwr9hqs5x4p0cx2tav2n426th85dn2v6gsue2tg7rp7nvgkj2c7csdlyx7pn5c66sd2ywjk")]
    public async Task Execute_Successfully_With_Correct_Bech32_Extended_Root_Key_When_Properties_Are_Valid(
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
        // TODO:
        //executionResult.Result.Should().Be(expectedBech32Key);
    }
}