using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DerivePaymentAddressCommandShould
{
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("test")]
    [InlineData("testnet")]
    [InlineData("Mainet")]
    [InlineData("mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_And_Type_Is_Valid_But_Network_Is_Invalid(
        string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
            PaymentAddressType = "Base",
            NetworkTag = networkTag
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --network-tag must be either Testnet or Mainnet");
    }

    [Theory]
    [InlineData("English", null, "Testnet")]
    [InlineData("English", null, "Mainnet")]
    [InlineData("English", "", "Mainnet")]
    [InlineData("English", "", "Testnet")]
    [InlineData("Spanish", null, "Testnet")]
    [InlineData("Spanish", null, "Mainnet")]
    [InlineData("Spanish", "", "Testnet")]
    [InlineData("Spanish", "", "Mainnet")]
    [InlineData("Japanese", null, "Testnet")]
    [InlineData("Japanese", null, "Mainnet")]
    [InlineData("Japanese", "", "Testnet")]
    [InlineData("Japanese", "", "Mainnet")]
    [InlineData("ChineseSimplified", null, "Testnet")]
    [InlineData("ChineseSimplified", null, "Mainnet")]
    [InlineData("ChineseSimplified", "", "Testnet")]
    [InlineData("ChineseSimplified", "", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Supplied_And_Network_Is_Valid(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --mnemonic is required");
    }

    [Theory]
    [InlineData("English", "shoe", "Testnet")]
    [InlineData("English", "baby laundry", "Testnet")]
    [InlineData("English", "gloom fatigue little", "Testnet")]
    [InlineData("English", "learn venue harvest fossil", "Testnet")]
    [InlineData("English", "daring please route manual orphan", "Testnet")]
    [InlineData("English", "once phrase win hawk before observe", "Testnet")]
    [InlineData("English", "anchor elite rare young flash chaos loop", "Testnet")]
    [InlineData("English", "salon expand attack move drip amateur second machine", "Testnet")]
    [InlineData("English", "script scale minute dial radar tissue spray another bubble benefit", "Testnet")]
    [InlineData("English", "eternal coin garment topic flag waste arch hobby tube great laptop donate lottery chief parade junior surge fortune zebra runway obey physical unknown logic fence", "Testnet")]
    [InlineData("Spanish", "bello", "Testnet")]
    [InlineData("Japanese", "なれる", "Testnet")]
    [InlineData("Italian", "fuggente", "Testnet")]
    [InlineData("ChineseSimplified", "厉", "Testnet")]
    [InlineData("English", "shoe", "Mainnet")]
    [InlineData("English", "baby laundry", "Mainnet")]
    [InlineData("English", "gloom fatigue little", "Mainnet")]
    [InlineData("English", "learn venue harvest fossil", "Mainnet")]
    [InlineData("English", "daring please route manual orphan", "Mainnet")]
    [InlineData("English", "once phrase win hawk before observe", "Mainnet")]
    [InlineData("English", "anchor elite rare young flash chaos loop", "Mainnet")]
    [InlineData("English", "salon expand attack move drip amateur second machine", "Mainnet")]
    [InlineData("English", "script scale minute dial radar tissue spray another bubble benefit", "Mainnet")]
    [InlineData("English", "eternal coin garment topic flag waste arch hobby tube great laptop donate lottery chief parade junior surge fortune zebra runway obey physical unknown logic fence", "Mainnet")]
    [InlineData("Spanish", "bello", "Mainnet")]
    [InlineData("Japanese", "なれる", "Mainnet")]
    [InlineData("Italian", "fuggente", "Mainnet")]
    [InlineData("ChineseSimplified", "厉", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Of_Valid_Length(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --mnemonic must have the following word count (9, 12, 15, 18, 21, 24)");
    }

    [Theory]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found", "Testnet")]
    [InlineData("Australian", "shoe follow blossom remain learn venue harvest fossil found", "Testnet")]
    [InlineData("en-GB", "shoe follow blossom remain learn venue harvest fossil found", "Testnet")]
    [InlineData("Klingon", "shoe follow blossom remain learn venue harvest fossil found", "Testnet")]
    [InlineData("english", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Australian", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("en-GB", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Klingon", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("english", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --language {language} is not supported");
    }

    [Theory]
    [InlineData("English", "dueño misil favor koala pudor aplicar guitarra fracaso talar", "Testnet")]
    [InlineData("Spanish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData("Japanese", "exigente pioneiro garrafa tarja genial dominado aclive tradutor fretar", "Testnet")]
    [InlineData("Italian", "りけん ぞんび こんすい たまる めだつ すんぽう つまらない せつりつ けんない", "Testnet")]
    [InlineData("French", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统", "Testnet")]
    [InlineData("ChineseSimplified", "danseur prouesse sauvage exquis cirque endosser saumon cintrer ratisser rompre pièce achat opinion cloporte orageux", "Testnet")]
    [InlineData("English", "dueño misil favor koala pudor aplicar guitarra fracaso talar", "Mainnet")]
    [InlineData("Spanish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    [InlineData("Japanese", "exigente pioneiro garrafa tarja genial dominado aclive tradutor fretar", "Mainnet")]
    [InlineData("Italian", "りけん ぞんび こんすい たまる めだつ すんぽう つまらない せつりつ けんない", "Mainnet")]
    [InlineData("French", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统", "Mainnet")]
    [InlineData("ChineseSimplified", "danseur prouesse sauvage exquis cirque endosser saumon cintrer ratisser rompre pièce achat opinion cloporte orageux", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Words_Are_Invalid_For_Language_Word_List(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Contain("invalid words");
    }

    [Theory]
    [InlineData(-1, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData(-2147483647, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData(-4, "Spanish", "dueño misil favor koala pudor aplicar guitarra fracaso talar", "Testnet")]
    [InlineData(-80765454, "Italian", "exigente pioneiro garrafa tarja genial dominado aclive tradutor fretar", "Testnet")]
    [InlineData(-12, "ChineseSimplified", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统", "Testnet")]
    [InlineData(-1, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    [InlineData(-2147483647, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    [InlineData(-4, "Spanish", "dueño misil favor koala pudor aplicar guitarra fracaso talar", "Mainnet")]
    [InlineData(-80765454, "Italian", "exigente pioneiro garrafa tarja genial dominado aclive tradutor fretar", "Mainnet")]
    [InlineData(-12, "ChineseSimplified", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Account_Index_Is_Out_Of_Bounds(
        int accountIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
            NetworkTag = networkTag,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --account-index must be between 0 and 2147483647");
    }

    [Theory]
    [InlineData(-1, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData(-3125, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData(-2, "Spanish", "dueño misil favor koala pudor aplicar guitarra fracaso talar", "Testnet")]
    [InlineData(-10, "Japanese", "りけん ぞんび こんすい たまる めだつ すんぽう つまらない せつりつ けんない", "Testnet")]
    [InlineData(-2147483647, "ChineseSimplified", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统", "Testnet")]
    [InlineData(-1, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    [InlineData(-3125, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    [InlineData(-2, "Spanish", "dueño misil favor koala pudor aplicar guitarra fracaso talar", "Mainnet")]
    [InlineData(-10, "Japanese", "りけん ぞんび こんすい たまる めだつ すんぽう つまらない せつりつ けんない", "Mainnet")]
    [InlineData(-2147483647, "ChineseSimplified", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Address_Index_Is_Out_Of_Bounds(
        int addressIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            AddressIndex = addressIndex,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --address-index must be between 0 and 2147483647");
    }
}