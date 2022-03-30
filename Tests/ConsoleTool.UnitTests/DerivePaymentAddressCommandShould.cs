using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DerivePaymentAddressCommandShould
{
    [Theory]
    [InlineData(null, "Base")]
    [InlineData("", "Base")]
    [InlineData("a", "Base")]
    [InlineData("test", "Base")]
    [InlineData("testnet", "Base")]
    [InlineData("Mainet", "Base")]
    [InlineData("mainnet", "Base")]
    [InlineData(null, "Enterprise")]
    [InlineData("", "Enterprise")]
    [InlineData("a", "Enterprise")]
    [InlineData("test", "Enterprise")]
    [InlineData("testnet", "Enterprise")]
    [InlineData("Mainet", "Enterprise")]
    [InlineData("mainnet", "Enterprise")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_NetworkTag_Is_Invalid(
        string networkTag, string paymentAddressType)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
            PaymentAddressType = paymentAddressType,
            NetworkTag = networkTag
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --network-tag must be either Testnet or Mainnet");
    }

    [Theory]
    [InlineData(null, "Testnet")]
    [InlineData("", "Testnet")]
    [InlineData("a", "Testnet")]
    [InlineData("test", "Testnet")]
    [InlineData("base", "Testnet")]
    [InlineData("Stake", "Testnet")]
    [InlineData("enterprise", "Testnet")]
    [InlineData("Delegating", "Testnet")]
    [InlineData(null, "Mainnet")]
    [InlineData("", "Mainnet")]
    [InlineData("a", "Mainnet")]
    [InlineData("test", "Mainnet")]
    [InlineData("base", "Mainnet")]
    [InlineData("Stake", "Mainnet")]
    [InlineData("enterprise", "Mainnet")]
    [InlineData("Delegating", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Type_Is_Invalid(
        string paymentAddressType, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
            PaymentAddressType = paymentAddressType,
            NetworkTag = networkTag
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --payment-address-type {paymentAddressType} is not supported");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Supplied_Deriving_Enterprise_Address_Type(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --recovery-phrase is required");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Supplied_Deriving_Base_Address_Type(
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
        executionResult.Result.Should().Be("Invalid option --recovery-phrase is required");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Of_Valid_Length_Deriving_Enterprise_Address_Type(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --recovery-phrase must have the following word count (9, 12, 15, 18, 21, 24)");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Of_Valid_Length_Deriving_Base_Address_Type(
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
        executionResult.Result.Should().Be("Invalid option --recovery-phrase must have the following word count (9, 12, 15, 18, 21, 24)");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported_Deriving_Enterprise_Address_Type(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --language {language} is not supported");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported_Deriving_Base_Address_Type(
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Words_Are_Invalid_For_Language_Word_List_Deriving_Enterprise_Address_Type(
        string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Contain("invalid words");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Words_Are_Invalid_For_Language_Word_List_Deriving_Base_Address_Type(
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Account_Index_Is_Out_Of_Bounds_Deriving_Enterprise_Address_Type(
        int accountIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --account-index must be between 0 and 2147483647");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Account_Index_Is_Out_Of_Bounds_Deriving_Base_Address_Type(
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Address_Index_Is_Out_Of_Bounds_Deriving_Enterprise_Address_Type(
        int addressIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            AddressIndex = addressIndex,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --address-index must be between 0 and 2147483647");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Address_Index_Is_Out_Of_Bounds_Deriving_Base_Address_Type(
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Stake_Account_Index_Is_Out_Of_Bounds_Deriving_Enterprise_Address_Type(
        int accountIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            StakeAccountIndex = accountIndex,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --stake-account-index must be between 0 and 2147483647");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Stake_Account_Index_Is_Out_Of_Bounds_Deriving_Base_Address_Type(
        int accountIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            StakeAccountIndex = accountIndex,
            NetworkTag = networkTag,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --stake-account-index must be between 0 and 2147483647");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Stake_Address_Index_Is_Out_Of_Bounds_Deriving_Enterprise_Address_Type(
        int addressIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            StakeAddressIndex = addressIndex,
            PaymentAddressType = "Enterprise",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --stake-address-index must be between 0 and 2147483647");
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Stake_Address_Index_Is_Out_Of_Bounds_Deriving_Base_Address_Type(
        int addressIndex, string language, string mnemonic, string networkTag)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            StakeAddressIndex = addressIndex,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --stake-address-index must be between 0 and 2147483647");
    }

    [Theory]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Testnet",
        "addr_test1vqdgwyhh07nl5geppqglg45mnhayalz3rynr7vfcdzz5spcmlaz2y")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Testnet",
        "addr_test1vzxha8wnyley93q3g8xu49un3rcfa8cn72p9fhlexvusm8c0muydn")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Mainnet",
        "addr1vydgwyhh07nl5geppqglg45mnhayalz3rynr7vfcdzz5spcqhf79p")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Mainnet",
        "addr1vxxha8wnyley93q3g8xu49un3rcfa8cn72p9fhlexvusm8c5ngczk")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Enterprise_Address_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string networkTag, string expectedBech32Key)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise"
        };
        var englishSpecificCommand = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
            PaymentAddressType = "Enterprise",
            Language = "English",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var englishSpecificCommandResult = await englishSpecificCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(englishSpecificCommandResult.Result);
    }

    [Theory]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Testnet",
        "addr_test1qqdgwyhh07nl5geppqglg45mnhayalz3rynr7vfcdzz5spag0578hervgc3lutmdkjlsxljjafs02canfzy4kdjaztaqqmtghs")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Testnet",
        "addr_test1qzxha8wnyley93q3g8xu49un3rcfa8cn72p9fhlexvusm8mtk8vg0tpqxr0dwmaaakm07w39p4grset5aps8yytuvvjq8mqa0p")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Mainnet",
        "addr1qydgwyhh07nl5geppqglg45mnhayalz3rynr7vfcdzz5spag0578hervgc3lutmdkjlsxljjafs02canfzy4kdjaztaqrdkgm0")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Mainnet",
        "addr1qxxha8wnyley93q3g8xu49un3rcfa8cn72p9fhlexvusm8mtk8vg0tpqxr0dwmaaakm07w39p4grset5aps8yytuvvjqydaar7")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Base_Address_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string networkTag, string expectedBech32Key)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
            PaymentAddressType = "Base"
        };
        var englishSpecificCommand = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
            PaymentAddressType = "Base",
            Language = "English",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var englishSpecificCommandResult = await englishSpecificCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(englishSpecificCommandResult.Result);
    }

    [Theory]
    [InlineData(
        0, 0, 
        "English", "month normal able please smile electric month tube vanish put yard absurd color rate explain", 
        "", "Mainnet",
        "addr1v9rc8jjh5rpnct5cmvulhwl3ucz382at8n8gngjn5p2p2vssgxm4e")]
    [InlineData(
        128, 256, 
        "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", 
        "", "Testnet",
        "addr_test1vz57jm2zyuv2szl05v2py0ncymyckacd3044e25u336sgkqupgd69")]
    [InlineData(
        2147483647, 2147483647, 
        "English", "call calm deny negative spawn rail state domain special auto worry address ankle control hurdle mother please fiber help gasp drama firm window post", 
        "", "Mainnet",
        "addr1v93ahas99cnmscnc59snrdr8jrg64nn6f2na9wu6zvj3rnq89067j")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Enterprise_Address_When_All_Properties_Are_Valid(
        int accountIndex, int addressIndex, 
        string language, string mnemonic, 
        string passPhrase, string networkTag, 
        string expectedBech32Key)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passPhrase,
            PaymentAddressType = "Enterprise",
            NetworkTag = networkTag,
            AccountIndex = accountIndex,
            AddressIndex = addressIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }

    [Theory]
    [InlineData(
        0, 0,
        0, 0,
        "English", "month normal able please smile electric month tube vanish put yard absurd color rate explain", 
        "", "Mainnet",
        "addr1q9rc8jjh5rpnct5cmvulhwl3ucz382at8n8gngjn5p2p2vnvvzt6ncfy4x8y8dcj3hhnzqej0v6pq3cffn80jx4kyrysyf5wjx")]
    [InlineData(
        128, 256,
        512, 1024,
        "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", 
        "", "Testnet",
        "addr_test1qz57jm2zyuv2szl05v2py0ncymyckacd3044e25u336sgk835zfjuuqu0g7fsf65576qrdjc3jwg8me6jed3mtt86ypqvek2ae")]
    [InlineData(
        2147483647, 2147483647,
        0, 0,
        "English", "call calm deny negative spawn rail state domain special auto worry address ankle control hurdle mother please fiber help gasp drama firm window post", 
        "", "Mainnet",
        "addr1q93ahas99cnmscnc59snrdr8jrg64nn6f2na9wu6zvj3rnxpyuzx6r35mfkx4wuqpcr04staphs3rq7kv7lkqj6zwrtq3xcm8r")]
    [InlineData(
        2147483647, 2147483647,
        2147483647, 2147483647,
        "English", "tenant sign hawk account actual hill breeze until kidney resource roast good stumble spread trade detail bean junior whale impact post report bike announce",
        "", "Mainnet",
        "addr1qyczqhu6asxyzgdl8h8rksm30ryv3s4hzjrfgmg6605uf97pu36v0qaawdz3xxfffrlesl60xhrl5uzle603v5a3mk7qw45fv6")]
    [InlineData(
        1, 6,
        2, 1,
        "Spanish", "moreno diario asiento falda enseñar médula mambo tarde línea celoso escena asegurar poner banda lápiz azul yodo champú metro ombligo neón ajuste resto carga",
        "0ZYM4ND145", "Mainnet",
        "addr1q9k7g0qh9ljtvflgax8ecnf39a6cpwa4qxzl5mjdpq6f8546uvf3nzhru4yhxxudaua8yjs8n38wzra7sjkfsnqdpzxqks2vdy")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Base_Address_When_All_Properties_Are_Valid(
        int accountIndex, int addressIndex,
        int stakeAccountIndex, int stakeAddressIndex,
        string language, string mnemonic, 
        string passPhrase, string networkTag, 
        string expectedBech32Key)
    {
        var command = new DerivePaymentAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passPhrase,
            PaymentAddressType = "Base",
            NetworkTag = networkTag,
            AccountIndex = accountIndex,
            AddressIndex = addressIndex,
            StakeAccountIndex = stakeAccountIndex,
            StakeAddressIndex = stakeAddressIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }
}