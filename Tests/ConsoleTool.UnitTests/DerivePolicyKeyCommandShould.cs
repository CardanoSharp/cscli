using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DerivePolicyKeyCommandShould
{
    [Theory]
    [InlineData("English", null)]
    [InlineData("English", "")]
    [InlineData("Spanish", null)]
    [InlineData("Spanish", "")]
    [InlineData("Japanese", null)]
    [InlineData("Japanese", "")]
    [InlineData("ChineseSimplified", null)]
    [InlineData("ChineseSimplified", "")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Supplied(
        string language, string mnemonic)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --recovery-phrase is required");
    }

    [Theory]
    [InlineData("English", "shoe")]
    [InlineData("English", "baby laundry")]
    [InlineData("English", "gloom fatigue little")]
    [InlineData("English", "learn venue harvest fossil")]
    [InlineData("English", "daring please route manual orphan")]
    [InlineData("English", "once phrase win hawk before observe")]
    [InlineData("English", "anchor elite rare young flash chaos loop")]
    [InlineData("English", "salon expand attack move drip amateur second machine")]
    [InlineData("English", "script scale minute dial radar tissue spray another bubble benefit")]
    [InlineData("English", "eternal coin garment topic flag waste arch hobby tube great laptop donate lottery chief parade junior surge fortune zebra runway obey physical unknown logic fence")]
    [InlineData("Spanish", "bello")]
    [InlineData("Japanese", "なれる")]
    [InlineData("Italian", "fuggente")]
    [InlineData("ChineseSimplified", "厉")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Mnemonic_Is_Not_Of_Valid_Length(
        string language, string mnemonic)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --recovery-phrase must have the following word count (9, 12, 15, 18, 21, 24)");
    }

    [Theory]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("Australian", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("en-GB", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("Klingon", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("aenglish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported(
        string language, string mnemonic)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --language {language} is not supported");
    }

    [Theory]
    [InlineData("English", "dueño misil favor koala pudor aplicar guitarra fracaso talar")]
    [InlineData("Spanish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData("Japanese", "exigente pioneiro garrafa tarja genial dominado aclive tradutor fretar")]
    [InlineData("Italian", "りけん ぞんび こんすい たまる めだつ すんぽう つまらない せつりつ けんない")]
    [InlineData("French", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统")]
    [InlineData("ChineseSimplified", "danseur prouesse sauvage exquis cirque endosser saumon cintrer ratisser rompre pièce achat opinion cloporte orageux")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Words_Are_Invalid_For_Language_Word_List(
        string language, string mnemonic)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Contain("invalid words");
    }

    [Theory]
    [InlineData(-1, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData(-2147483647, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData(-4, "Spanish", "dueño misil favor koala pudor aplicar guitarra fracaso talar")]
    [InlineData(-80765454, "Italian", "exigente pioneiro garrafa tarja genial dominado aclive tradutor fretar")]
    [InlineData(-12, "ChineseSimplified", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Policy_Index_Is_Out_Of_Bounds(
        int policyIndex, string language, string mnemonic)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            PolicyIndex = policyIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --policy-index must be between 0 and 2147483647");
    }

    [Theory]
    [InlineData("fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "policy_sk1jpu3gph53zpy7dqln5mrwwzg2ngkru4a0tjefxsujpqjrztsl3frerkpsstt8d9g3xy9fd86n4yr39vyqvmklml7eenxvlv3l6vu77cujthkt")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain",
        "policy_sk1lq0vvz0vsh4j0amsq4508gtzkcyed29zz8wxk82ww26365kj5dz7jrkfxnllu8h9qydgjwd0z3hpd8fahfjzjhg2f6g3fxtxmlfulvct5g2r8")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity",
        "policy_sk1hq0pmg0f4t0vsw93yn3d4t9qcmua2l97jpfsvy2jedqmtm3gy3vnuhdmkrmaypjdy27xzkkjh4axxqt6pc9d5gdq7wvfnlpuxldctggr5pdlf")]
    public async Task Derive_Correct_Bech32_Policy_Signing_Key_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic
        };
        var englishSpecificCommand = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = "English"
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var englishSpecificCommandResult = await englishSpecificCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(englishSpecificCommandResult.Result);
    }

    [Theory]
    [InlineData("English", "afford attack bag emotion ensure detect effort bean luggage",
        "policy_sk1rzs6wnwd6a7ete3wk5x4m8qgs54mjqk6utrvcv5mvajvhxw6j3fxhunp6kpq8aer9sk8tk4kdqtr645xe8c00sychessdhrtvygk0fs9l8pw2")]
    [InlineData("English", "panther cave tornado remain snap leisure gown vote strategy apple away room shrug action reunion",
        "policy_sk13rfld8v4cvpp0eqp8d3gn2zwrwyk8hdshyfthclj3q6pgjkcw493m66652fqjkdy69pz3ccc64mlvm6s9qz7fr02jzgyk9r2evr06dccwda4e")]
    [InlineData("English", "safe mixture exhaust worth smart level donkey orphan hello ski want runway example fame gaze thumb voyage swift wire common move injury promote media",
        "policy_sk16plgzdumg9sae40z0kuxvfnftm5da7xuvnu5w79v462824l5h99jczck5tamf5zwrf0px2gh2mmqj9q80p5g8xvtfcn2u65z9aqj2pgs3glqp")]
    [InlineData("Spanish", "vitamina banda araña rincón queja copa guiso camello limón fase objeto martes náusea matar urgente sello emitir melón acuerdo herir litio pausa dar paella",
        "policy_sk12z5ccahwswmykmcm7rtakn672d6st52t0uz0jw7xstydz5taearlj3gd5e8l5ah6au62h0mhcf4hmvx4haavn22ptc6ja764pl8t6us64d9mc")]
    [InlineData("Japanese", "おうふく いってい にんか さとおや すばらしい ほせい てはい めんどう ぎいん にちじょう ほうりつ しなん げきとつ ききて ねんおし むしあつい げんそう えんそく ぎっちり さつたば ちきゅう あんい すわる せんか",
        "policy_sk1pr4fykp77ktx4p4vakfj55zceat8krvxqq5x37lmsf0nkp69j4dh8a0e5dvnsqgul8hdct00yzx2yy8vrzvjkr33jluxh8ne05ypnkcp650va")]
    [InlineData("ChineseSimplified", "驻 歇 职 熙 盐 剪 带 泰 轴 枯 奥 兵 曰 桂 导 睡 抵 宁 容 读 幻 悉 默 新",
        "policy_sk1lpfhx2uy546d6kzxx8nzwsvnmqqdv7reepefnkd3hmsda0asjed6kch97e2zjckpaq9zhryrry2s0vrfxn3mdmgt064ws436gfct76cprxjf4")]
    public async Task Derive_Correct_Bech32_Policy_Signing_Key_Defaulting_To_Zeroth_Indexes_When_Policy_Index_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic
        };
        var zeroIndexCommand = new DerivePolicyKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic,
            PolicyIndex = 0,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var englishSpecificCommandResult = await zeroIndexCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(englishSpecificCommandResult.Result);
    }

    [Theory]
    [InlineData(
        "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        "policy_sk10r7sav83m680w25pdtmg7ugpu28nrqjxsw54erkeulzamwx5sa8j9ente03nwksuryrj4hl2qmyypy6m8cdmjafmwpxc9wl2unqfmdq9jh8wr")]
    [InlineData(
        "English", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame",
        "policy_sk15rn8rquz3839rl2njrg5lvqeenukvxuk7yejm3whwrag9wczrdrmeujkfwcw46en5c66q5nqgx7h53axxckp6a0lvh7a8tayx8cwrhctnczw3")]
    [InlineData(
        "English", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird",
        "policy_sk1szwvgf8t3rff3k8m2xtveklvc9cdm6v32ns9gmc99ezg5cnv0ex0fntjjgvsx5u2fqcl3vqex25katntfp72u4e3cf9drv45nf5afvs3g4y7k")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco",
        "policy_sk1nzndjdqdrzqe9d3wecg5q3fq3rg5akux7u2knsgtpvd9udvpr4wgzlul5svmhasp44dwq0f66dzt20vy88xy4xwhv2hmpk8vp2ctnecechku4")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ",
        "policy_sk1qq95vxcck5js2zfz26x82r3ctyv359hw25spddcft9mctm25gewg674awfj293wymtp0t440zc4nuy5l7qt597y22k35m57gpdwd3ts8mq589")]
    [InlineData(
        "Italian", "sfacelo quadro bordo ballata pennuto scultore ridurre tesserato triturato palazzina ossidare riportare sgarbato vedova targato",
        "policy_sk1jze26fwya3q3ksgue3r4zftugcl97hxaucctc4jxwx49fk9aadw7mja2zf5qy0wgzjrf8w99t4rr7qg7dtlww6uc7gly52z7fltplnszvrjzw")]
    [InlineData(
        "ChineseSimplified", "胁 粒 了 对 拆 端 泼 由 烈 苏 筹 如 技 防 厚",
        "policy_sk1lr7pxmvvjdytg95ut2p8hc4tvshv97sw3tzf35833nest82tnfw45h230pfpxdf32csu2pmmjg0va6me2530c2qskl8tayyq900yh9gsa3k40")]
    public async Task Derive_Correct_Bech32_Policy_Signing_Key_Defaulting_To_Empty_Passphrase_When_Passphrase_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };
        var emptyPassSpecificCommand = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = "",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);
        var emptyPassCommandResult = await emptyPassSpecificCommand.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
        executionResult.Result.Should().Be(emptyPassCommandResult.Result);
    }

    [Theory]
    [InlineData("pass", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "policy_sk1wps5pvrqmtn26qpk9a42y2f9xrgsp72skl87xsf6l6s2dpe2qaz732ddtzf5dmvnu9ym37upjdstq9439kkvvl7z55zdmxgkdr5yaegjh6xmw")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "policy_sk14pz4quey5qqjctj9tneqx20wup57lp07dhvswdtqv8mdg5sl2pz73rswnuwejp8hh7sx7snp4qum6k32lu9vd09gjd5d7x3qknn3pccyzczsl")]
    [InlineData("pass", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "policy_sk1lryhh6qftedcfvswnvgtsvesvazf7kyldj8lcldaxee4pxvgndyway36kzhqa6n2pxt0v0wrq8fvfsgxw3ftf6z7a2mphumerzen7qc0a9drn")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "policy_sk16zdr79sg7arzv9jt0zxsa23rhxj95qcfknjp3mpakw8swv9udff5as9k2vyujuxralmuqlnwg3hfhtf27gx8mrswkr5m2e2qrdzt3rs4h7ed3")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "policy_sk12zg80r0p5u2r3dava9986u3tpu4hewm5wujkcj42u597emlgn3v048n3qtz6n9lckes2n6q92f5td9uvp2chn6jzxdaetx4m2vq78gqer5zqj")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "policy_sk1kqsaufrwj2v9q25j28j5hn8cjeqm28338p8amfv2lecwyefwvdv4kcqmpnuhg899c9nepz0z0yf2zjpuag5hjegp80ctdryltjr9rlgf9cxsg")]
    public async Task Derive_Correct_Bech32_Policy_Signing_Key_When_Passphrase_And_Language_Are_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string passPhrase, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passPhrase,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }

    [Theory]
    [InlineData(0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "policy_sk1jpu3gph53zpy7dqln5mrwwzg2ngkru4a0tjefxsujpqjrztsl3frerkpsstt8d9g3xy9fd86n4yr39vyqvmklml7eenxvlv3l6vu77cujthkt")]
    [InlineData(1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "policy_sk1yqwlmgqs8jg4se8tmf5j0m3rwnn4uzuwxuu4fwmyfrd60qtsl3fgmgu3wqhdz6rl90ppmz84hh7xxgnz6j2zra4z3d5ptw4l2nz53ysv6x7mq")]
    [InlineData(280916, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "policy_sk1fpdf3hm2hnczl209wj0tzt3rx3q6nr375w8vfjrz7pmd8qmsl3f0lpnqqvsw7qr7sp930tcazpgzx66yjq85scszz8304p4g0jxy74g3qrgca")]
    [InlineData(2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "policy_sk18zhcl060x7l5jp7cjz26m3fd0zv2gpea5jdsdrydcg2y9ptsl3fdrqd9lczddtmr0mvsgyczaym584s2hrvszpwm6554xrjpsdqlu7c5phkcs")]
    public async Task Derive_Correct_Bech32_Policy_Signing_Key_When_Policy_Index_Is_Supplied_And_Mnemonic_Is_Valid_For_Language(
        int policyIndex, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePolicyKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            PolicyIndex = policyIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }
}