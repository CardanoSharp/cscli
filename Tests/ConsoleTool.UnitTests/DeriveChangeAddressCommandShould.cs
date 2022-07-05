using Cscli.ConsoleTool.Wallet;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DeriveChangeAddressCommandShould
{
    [Theory]
    [InlineData(null, "Base")]
    [InlineData("", "Base")]
    [InlineData("a", "Base")]
    [InlineData("test", "Base")]
    [InlineData("Mainet", "Base")]
    [InlineData(null, "Enterprise")]
    [InlineData("", "Enterprise")]
    [InlineData("a", "Enterprise")]
    [InlineData("test", "Enterprise")]
    [InlineData("Mainet", "Enterprise")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_NetworkTag_Is_Invalid(
        string network, string paymentAddressType)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
            PaymentAddressType = paymentAddressType,
            Network = network
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --network must be either testnet or mainnet");
    }

    [Theory]
    [InlineData(null, "Testnet")]
    [InlineData("", "Testnet")]
    [InlineData("a", "Testnet")]
    [InlineData("test", "Testnet")]
    [InlineData("based", "Testnet")]
    [InlineData("Stake", "Testnet")]
    [InlineData("enterprised", "Testnet")]
    [InlineData("delegating", "Testnet")]
    [InlineData(null, "Mainnet")]
    [InlineData("", "Mainnet")]
    [InlineData("a", "Mainnet")]
    [InlineData("test", "Mainnet")]
    [InlineData("baste", "Mainnet")]
    [InlineData("Stake", "Mainnet")]
    [InlineData("entorprise", "Mainnet")]
    [InlineData("Delegating", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Type_Is_Invalid(
        string paymentAddressType, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
            PaymentAddressType = paymentAddressType,
            Network = network
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
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
    [InlineData("englis", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Australian", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("en-GB", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Klingon", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("aenglish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported_Deriving_Enterprise_Address_Type(
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
    [InlineData("englis", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Australian", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("en-GB", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Klingon", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("anglish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported_Deriving_Base_Address_Type(
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        int accountIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
            Network = network,
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
        int accountIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
            Network = network,
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
        int addressIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        int addressIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        int accountIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            StakeAccountIndex = accountIndex,
            Network = network,
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
        int accountIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            StakeAccountIndex = accountIndex,
            Network = network,
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
        int addressIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
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
        int addressIndex, string language, string mnemonic, string network)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Network = network,
            StakeAddressIndex = addressIndex,
            PaymentAddressType = "Base",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --stake-address-index must be between 0 and 2147483647");
    }

    [Theory]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Testnet",
        "addr_test1vr9z8ypdcmq3l745tpp4yx80ug3764ph4xv086kwrvacrjstzjh95")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Testnet",
        "addr_test1vrtfjdhdmkcuh9x035d9s62cdr24enkq35t2jcusl9uc4nq4ymd3m")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Mainnet",
        "addr1v89z8ypdcmq3l745tpp4yx80ug3764ph4xv086kwrvacrjss2xt23")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Mainnet",
        "addr1v8tfjdhdmkcuh9x035d9s62cdr24enkq35t2jcusl9uc4nqwv0377")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Enterprise_Address_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string network, string expectedBech32Key)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Network = network,
            PaymentAddressType = "Enterprise"
        };
        var englishSpecificCommand = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Network = network,
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
        "addr_test1qr9z8ypdcmq3l745tpp4yx80ug3764ph4xv086kwrvacrj4g0578hervgc3lutmdkjlsxljjafs02canfzy4kdjaztaq5aejy8")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Testnet",
        "addr_test1qrtfjdhdmkcuh9x035d9s62cdr24enkq35t2jcusl9uc4nrtk8vg0tpqxr0dwmaaakm07w39p4grset5aps8yytuvvjqh0t2vv")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Mainnet",
        "addr1q89z8ypdcmq3l745tpp4yx80ug3764ph4xv086kwrvacrj4g0578hervgc3lutmdkjlsxljjafs02canfzy4kdjaztaqhtyjgc")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Mainnet",
        "addr1q8tfjdhdmkcuh9x035d9s62cdr24enkq35t2jcusl9uc4nrtk8vg0tpqxr0dwmaaakm07w39p4grset5aps8yytuvvjq5ek2qn")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Base_Address_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string network, string expectedBech32Key)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Network = network,
            PaymentAddressType = "Base"
        };
        var englishSpecificCommand = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Network = network,
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
        "addr1vyqpf2v38xs2j5hd5rd7m0dqz5jpcpn55pzmyc7wdu3hrygkvn7ks")]
    [InlineData(
        128, 256, 
        "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", 
        "", "Testnet",
        "addr_test1vpqmuqzy9s3p202fyjuszxymql3w0ldsdxfj2cj30p3g68c3aqry0")]
    [InlineData(
        2147483647, 2147483647, 
        "English", "call calm deny negative spawn rail state domain special auto worry address ankle control hurdle mother please fiber help gasp drama firm window post", 
        "", "Mainnet",
        "addr1vy344kk7kquw22mtr5l0ckylh8lxm0yd6rsvyzdf8jtlg6q38cswh")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Enterprise_Address_When_All_Properties_Are_Valid(
        int accountIndex, int addressIndex, 
        string language, string mnemonic, 
        string passPhrase, string network, 
        string expectedBech32Key)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passPhrase,
            PaymentAddressType = "Enterprise",
            Network = network,
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
        "addr1qyqpf2v38xs2j5hd5rd7m0dqz5jpcpn55pzmyc7wdu3hrytvvzt6ncfy4x8y8dcj3hhnzqej0v6pq3cffn80jx4kyryshm686f")]
    [InlineData(
        128, 256,
        512, 1024,
        "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", 
        "", "Testnet",
        "addr_test1qpqmuqzy9s3p202fyjuszxymql3w0ldsdxfj2cj30p3g68l35zfjuuqu0g7fsf65576qrdjc3jwg8me6jed3mtt86ypq4evjqh")]
    [InlineData(
        2147483647, 2147483647,
        0, 0,
        "English", "call calm deny negative spawn rail state domain special auto worry address ankle control hurdle mother please fiber help gasp drama firm window post", 
        "", "Mainnet",
        "addr1qy344kk7kquw22mtr5l0ckylh8lxm0yd6rsvyzdf8jtlg6xpyuzx6r35mfkx4wuqpcr04staphs3rq7kv7lkqj6zwrtqvqay20")]
    [InlineData(
        2147483647, 2147483647,
        2147483647, 2147483647,
        "English", "tenant sign hawk account actual hill breeze until kidney resource roast good stumble spread trade detail bean junior whale impact post report bike announce",
        "", "Mainnet",
        "addr1q965tlxgsf5ze4u932gzczeduv58hzptede343a90frtjfwpu36v0qaawdz3xxfffrlesl60xhrl5uzle603v5a3mk7q3pfk59")]
    [InlineData(
        1, 6,
        2, 1,
        "Spanish", "moreno diario asiento falda enseñar médula mambo tarde línea celoso escena asegurar poner banda lápiz azul yodo champú metro ombligo neón ajuste resto carga",
        "0ZYM4ND145", "Mainnet",
        "addr1qy454sacc2v3ted0wa9a0jmu498awl98t948ztvf2a7fhaa6uvf3nzhru4yhxxudaua8yjs8n38wzra7sjkfsnqdpzxqnw2xf5")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Base_Address_When_All_Properties_Are_Valid(
        int accountIndex, int addressIndex,
        int stakeAccountIndex, int stakeAddressIndex,
        string language, string mnemonic, 
        string passPhrase, string network, 
        string expectedBech32Key)
    {
        var command = new DeriveChangeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passPhrase,
            PaymentAddressType = "Base",
            Network = network,
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