using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DeriveStakeAddressCommandShould
{
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("test")]
    [InlineData("tastnet")]
    [InlineData("Mainet")]
    [InlineData("mainet")]
    [InlineData("mainnetwork")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Valid_But_Network_Is_Invalid(
        string networkTag)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
            NetworkTag = networkTag
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --network-tag must be either testnet or mainnet");
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
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Of_Valid_Length(
        string language, string mnemonic, string networkTag)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag
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
    [InlineData("engish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet")]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Australian", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("en-GB", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("Klingon", "shoe follow blossom remain learn venue harvest fossil found", "Mainnet")]
    [InlineData("aenglish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported(
        string language, string mnemonic, string networkTag)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag
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
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag
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
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
            NetworkTag = networkTag,
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
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            AddressIndex = addressIndex
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --address-index must be between 0 and 2147483647");
    }

    [Theory]
    [InlineData("fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1uz6rw83gnkzzut745ed0tflu8ku6p2ete5qq45jg422lgug5v4msa")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Testnet",
        "stake_test1uz5860rmu3kyvgl79akmf0cr0efw5c84vwe53z2mxew397smcymwk")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Testnet",
        "stake_test1up4mrky84ssrphkhd777mdhl8gjs65pcv46wscrjz97xxfqzqmqs4")]
    [InlineData("fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1ux6rw83gnkzzut745ed0tflu8ku6p2ete5qq45jg422lgugnxle5q")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain", "Mainnet",
        "stake1ux5860rmu3kyvgl79akmf0cr0efw5c84vwe53z2mxew397sujwe2t")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity", "Mainnet",
        "stake1u94mrky84ssrphkhd777mdhl8gjs65pcv46wscrjz97xxfq923z5g")]
    public async Task Derive_Correct_Bech32_Extended_Stake_Address_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string networkTag, string expectedBech32Key)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
        };
        var englishSpecificCommand = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
            Language = "English"
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var englishSpecificCommandResult = await englishSpecificCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(englishSpecificCommandResult.Result);
    }

    [Theory]
    [InlineData("English", "afford attack bag emotion ensure detect effort bean luggage", "Testnet",
        "stake_test1ur8dajpsvk0434khpymvxz8m4usfxy0x4gem269nxtq0m2g3g4h9q")]
    [InlineData("English", "panther cave tornado remain snap leisure gown vote strategy apple away room shrug action reunion", "Testnet",
        "stake_test1urg5x5sduuwxm7csz2knfcs94yj0uuhcjuf7rhrz3hcflfgk24vez")]
    [InlineData("English", "safe mixture exhaust worth smart level donkey orphan hello ski want runway example fame gaze thumb voyage swift wire common move injury promote media", "Testnet",
        "stake_test1uz4y5na9npfxe4pave0zrwzvjrjh2zkqucd30haxm2qh0agxz4g0p")]
    [InlineData("Spanish", "vitamina banda araña rincón queja copa guiso camello limón fase objeto martes náusea matar urgente sello emitir melón acuerdo herir litio pausa dar paella", "Testnet",
        "stake_test1uqn6pehgpegvcryq90rtvm2m79ygus4ygpptmmvtuq7f7lq29l29q")]
    [InlineData("Japanese", "おうふく いってい にんか さとおや すばらしい ほせい てはい めんどう ぎいん にちじょう ほうりつ しなん げきとつ ききて ねんおし むしあつい げんそう えんそく ぎっちり さつたば ちきゅう あんい すわる せんか", "Testnet",
        "stake_test1urg9k69ay0rwpsj3lg50ssv2xmrnfx70wys9gstrcheqfpswe4865")]
    [InlineData("ChineseSimplified", "驻 歇 职 熙 盐 剪 带 泰 轴 枯 奥 兵 曰 桂 导 睡 抵 宁 容 读 幻 悉 默 新", "Testnet",
        "stake_test1urk98s0grl47evvjj7kdfd267rn3fw97luvhsa3vp452q4gvugacy")]
    [InlineData("English", "afford attack bag emotion ensure detect effort bean luggage", "Mainnet",
        "stake1u88dajpsvk0434khpymvxz8m4usfxy0x4gem269nxtq0m2gkzl4pa")]
    [InlineData("English", "panther cave tornado remain snap leisure gown vote strategy apple away room shrug action reunion", "Mainnet",
        "stake1u8g5x5sduuwxm7csz2knfcs94yj0uuhcjuf7rhrz3hcflfg3qlwal")]
    [InlineData("English", "safe mixture exhaust worth smart level donkey orphan hello ski want runway example fame gaze thumb voyage swift wire common move injury promote media", "Mainnet",
        "stake1ux4y5na9npfxe4pave0zrwzvjrjh2zkqucd30haxm2qh0agpgl2tu")]
    [InlineData("Spanish", "vitamina banda araña rincón queja copa guiso camello limón fase objeto martes náusea matar urgente sello emitir melón acuerdo herir litio pausa dar paella", "Mainnet",
        "stake1uyn6pehgpegvcryq90rtvm2m79ygus4ygpptmmvtuq7f7lqd04gpa")]
    [InlineData("Japanese", "おうふく いってい にんか さとおや すばらしい ほせい てはい めんどう ぎいん にちじょう ほうりつ しなん げきとつ ききて ねんおし むしあつい げんそう えんそく ぎっちり さつたば ちきゅう あんい すわる せんか", "Mainnet",
        "stake1u8g9k69ay0rwpsj3lg50ssv2xmrnfx70wys9gstrcheqfpsfnl97f")]
    [InlineData("ChineseSimplified", "驻 歇 职 熙 盐 剪 带 泰 轴 枯 奥 兵 曰 桂 导 睡 抵 宁 容 读 幻 悉 默 新", "Mainnet",
        "stake1u8k98s0grl47evvjj7kdfd267rn3fw97luvhsa3vp452q4gtkzlue")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_Defaulting_To_Zeroth_Indexes_When_Account_Or_Address_Index_Are_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string networkTag, string expectedBech32Key)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Language = language,
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
        };
        var englishSpecificCommand = new DeriveStakeAddressCommand()
        {
            Language = language,
            Mnemonic = mnemonic,
            NetworkTag = networkTag,
            AccountIndex = 0,
            AddressIndex = 0,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var englishSpecificCommandResult = await englishSpecificCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(englishSpecificCommandResult.Result);
    }

    [Theory]
    [InlineData(
        "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Testnet",
        "stake_test1up6t0g6jym0passac8p9l4asv543wpm6cxtzd2atefst3ys0ggcx4")]
    [InlineData(
        "English", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame", "Testnet",
        "stake_test1uq389q4tfa00gkh3uvhmvrl3xcazdxaj6np2gst8t60us8qnm34h7")]
    [InlineData(
        "English", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird", "Testnet",
        "stake_test1uzzmth58uzhdt0wpm7f5aswl8x6xl8fp5d5d5yjgfmfun7s0g0cup")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco", "Testnet",
        "stake_test1uzrlwqzm4250d9v9xxm63ccm0rlk8fm9shw7xl8lqmky69gvnzsk4")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ", "Testnet",
        "stake_test1uqxshvd0f5642s2yvgtkr3dmy9vvpycqhwc5ms59car7pkqm0c6j7")]
    [InlineData(
        "Italian", "sfacelo quadro bordo ballata pennuto scultore ridurre tesserato triturato palazzina ossidare riportare sgarbato vedova targato", "Testnet",
        "stake_test1upv799t67vu7lwte3kuzr95qqzzxcpagak2v4r5w4yh49hcuewmj5")]
    [InlineData(
        "ChineseSimplified", "胁 粒 了 对 拆 端 泼 由 烈 苏 筹 如 技 防 厚", "Testnet",
        "stake_test1uqgc7j3js4jlneneckjl2xhpj2hy7xuk90h3t8cfrt4nq7q02jaly")]
    [InlineData(
        "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug", "Mainnet",
        "stake1u96t0g6jym0passac8p9l4asv543wpm6cxtzd2atefst3ysgzz6zg")]
    [InlineData(
        "English", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame", "Mainnet",
        "stake1uy389q4tfa00gkh3uvhmvrl3xcazdxaj6np2gst8t60us8q53mhnr")]
    [InlineData(
        "English", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird", "Mainnet",
        "stake1uxzmth58uzhdt0wpm7f5aswl8x6xl8fp5d5d5yjgfmfun7sgz96cu")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco", "Mainnet",
        "stake1uxrlwqzm4250d9v9xxm63ccm0rlk8fm9shw7xl8lqmky69gtegjjg")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ", "Mainnet",
        "stake1uyxshvd0f5642s2yvgtkr3dmy9vvpycqhwc5ms59car7pkqu9jckr")]
    [InlineData(
        "Italian", "sfacelo quadro bordo ballata pennuto scultore ridurre tesserato triturato palazzina ossidare riportare sgarbato vedova targato", "Mainnet",
        "stake1u9v799t67vu7lwte3kuzr95qqzzxcpagak2v4r5w4yh49hcmnyekf")]
    [InlineData(
        "ChineseSimplified", "胁 粒 了 对 拆 端 泼 由 烈 苏 筹 如 技 防 厚", "Mainnet",
        "stake1uygc7j3js4jlneneckjl2xhpj2hy7xuk90h3t8cfrt4nq7qgqclme")]
    public async Task Derive_Correct_Bech32_Stake_Address_Defaulting_To_Empty_Passphrase_When_Passphrase_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string networkTag, string expectedBech32Key)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
        };
        var emptyPassSpecificCommand = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = "",
            NetworkTag = networkTag,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var emptyPassCommandResult = await emptyPassSpecificCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(emptyPassCommandResult.Result);
    }

    [Theory]
    [InlineData("pass", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1up7rgh4ptxwjempslj4yels5cajyyy0vsxvdjpuyp76lmfqlsvrsg")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1upntcp7w02n4cltzq8ra3l8ej24cnxgm9u4v3jl9ka5xprc7wv325")]
    [InlineData("pass", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto", "Testnet",
        "stake_test1urthp0hc24m9cw6edd9240msrw09ep8yalkxyrs6262wh7c4wjjjx")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto", "Testnet",
        "stake_test1uznam3gt6wzzh4k2d6cf93gyq59euexud6mdqks2em8g88qmduuyd")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい", "Testnet",
        "stake_test1ur8khsttt3caulkcv4renuq3u8amsg7ta4v0d8zfyrwcrdq7024qh")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい", "Testnet",
        "stake_test1uzn4wausnh3w7nhh8ve6d7m5j84rh53xsvw00rzqa8cp9sqs99nxm")]
    [InlineData("pass", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1u97rgh4ptxwjempslj4yels5cajyyy0vsxvdjpuyp76lmfqc6xp54")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1u9ntcp7w02n4cltzq8ra3l8ej24cnxgm9u4v3jl9ka5xprceyxnwf")]
    [InlineData("pass", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto", "Mainnet",
        "stake1u8thp0hc24m9cw6edd9240msrw09ep8yalkxyrs6262wh7cjycskm")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto", "Mainnet",
        "stake1uxnam3gt6wzzh4k2d6cf93gyq59euexud6mdqks2em8g88qu8k7qs")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい", "Mainnet",
        "stake1u88khsttt3caulkcv4renuq3u8amsg7ta4v0d8zfyrwcrdqe9qhy2")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい", "Mainnet",
        "stake1uxn4wausnh3w7nhh8ve6d7m5j84rh53xsvw00rzqa8cp9sqh003zx")]
    public async Task Derive_Correct_Bech32_Stake_Address_When_Passphrase_And_Language_Are_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string passPhrase, string language, string mnemonic, string networkTag, string expectedBech32Key)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passPhrase,
            NetworkTag = networkTag
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }

    [Theory]
    [InlineData(0, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1uz6rw83gnkzzut745ed0tflu8ku6p2ete5qq45jg422lgug5v4msa")]
    [InlineData(0, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1upm5m5nruxpszuauaydgte5g0725caks5gjll0smu67hrsceq740h")]
    [InlineData(1, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1uqreguywem6p70hrpdnxty3t68jq288qwkeeyqyhxs2v2nsea5jjz")]
    [InlineData(1, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1uqvu882g4ky6ffyj5mdpwncj2hh4h3p0mnrcz3lr4awfytgjhf6aj")]
    [InlineData(280916, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1ur2gqkvypqjpu3fwzwk4tl037uxq7magdj868x29ca6guacu8wcjl")]
    [InlineData(2147483647, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1uqddv2wawf85cz056wc7cw9zpuhyutz2qsx03f8y9x7xescc4u678")]
    [InlineData(0, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1up6cxvmdj8z4urws2r6auwp5nu7c5639h2ycsq32p0f8y9crx7njw")]
    [InlineData(2147483647, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Testnet",
        "stake_test1uzekeny55vzsq5hhm5kppz0nyggs6vtul0uaztjltq2scjs69ukvw")]
    [InlineData(0, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1ux6rw83gnkzzut745ed0tflu8ku6p2ete5qq45jg422lgugnxle5q")]
    [InlineData(0, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1u9m5m5nruxpszuauaydgte5g0725caks5gjll0smu67hrsc725ht2")]
    [InlineData(1, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1uyreguywem6p70hrpdnxty3t68jq288qwkeeyqyhxs2v2ns7h7skl")]
    [InlineData(1, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1uyvu882g4ky6ffyj5mdpwncj2hh4h3p0mnrcz3lr4awfytg4arce0")]
    [InlineData(280916, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1u82gqkvypqjpu3fwzwk4tl037uxq7magdj868x29ca6guacmdy6kz")]
    [InlineData(2147483647, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1uyddv2wawf85cz056wc7cw9zpuhyutz2qsx03f8y9x7xescllkc66")]
    [InlineData(0, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1u96cxvmdj8z4urws2r6auwp5nu7c5639h2ycsq32p0f8y9cyv53kn")]
    [InlineData(2147483647, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category", "Mainnet",
        "stake1uxekeny55vzsq5hhm5kppz0nyggs6vtul0uaztjltq2scjsa0k5gn")]
    public async Task Derive_Correct_Bech32_Extended_Stake_Address_When_Account_And_Address_Index_Is_Supplied_And_Mnemonic_Is_Valid_For_Language(
        int accountIndex, int addressIndex, string language, string mnemonic, string networkTag, string expectedBech32Key)
    {
        var command = new DeriveStakeAddressCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            NetworkTag = networkTag,
            AccountIndex = accountIndex,
            AddressIndex = addressIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }
}