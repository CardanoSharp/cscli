using Cscli.ConsoleTool.Wallet;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DerivePaymentKeyCommandShould
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
        var command = new DerivePaymentKeyCommand()
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
        var command = new DerivePaymentKeyCommand()
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
        var command = new DerivePaymentKeyCommand()
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
        var command = new DerivePaymentKeyCommand()
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
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Account_Index_Is_Out_Of_Bounds(
        int accountIndex, string language, string mnemonic)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --account-index must be between 0 and 2147483647");
    }

    [Theory]
    [InlineData(-1, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData(-3125, "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    [InlineData(-2, "Spanish", "dueño misil favor koala pudor aplicar guitarra fracaso talar")]
    [InlineData(-10, "Japanese", "りけん ぞんび こんすい たまる めだつ すんぽう つまらない せつりつ けんない")]
    [InlineData(-2147483647, "ChineseSimplified", "趋 富 吸 献 树 吾 秒 举 火 侦 佛 疑 机 察 统")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Address_Index_Is_Out_Of_Bounds(
        int addressIndex, string language, string mnemonic)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AddressIndex = addressIndex
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --address-index must be between 0 and 2147483647");
    }

    [Theory]
    [InlineData("fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1eq6alt578rfvylf8wvs3yvwdgu6gp6x90hxkalgq2evs9xrsl3fv3gszsell94j2uggl9hclf7lv7x03qgjz025j5aclp8eryy2t9vcqlmqjhn7tr4tlpafsn0a53ydx4ls2wwpz47zgx3uq5s8zn848nv8y72jz")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain",
        "addr_xsk1wqwmc825vssvpaqhswu272phfv60xkdw0jmyxq0apxuvx5kj5dzsaggm04quedulk8whsqqwrh5jrantzmk0m3ey3ju5s76myhaeydh96mur3xurt6a7gkdv4gl8wv8423sclen6vwaa4vqyryfx8vc2scg3q5kg")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity",
        "addr_xsk1wqtdwmgurwm95msl4dtwwsxzv398pldp5eq2d4sz8jr0r7egy3vhrs3ax585qpc9adpzyz0j6rgl06fwe2dnqk2yexvmggwyqwekm53aud90etcgu87hx4w669q0kalaaa3sh32tmrvf6vg8jdkq57thrvtg5xgh")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic
        };
        var englishSpecificCommand = new DerivePaymentKeyCommand()
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
        "addr_xsk19qllx5zukekr84yxwklj9r4fygs5tsmqc7yaeum08eaxp8k6j3fgwz5udkkn2uf02wn6afejvz275uxrls5w0r796rejdu2x8rmwl4yvflujwrmjuetppad48ysnrql4dem7pje90h29nvh72j88nrhums3yu2js")]
    [InlineData("English", "panther cave tornado remain snap leisure gown vote strategy apple away room shrug action reunion",
        "addr_xsk1jp5d7ssncqgqa79dfwvjfs34ednwlttth60kfud5t866657cw49hardn7csuj3kgs6ecxuuwp5xguukdnk5gdugx2znajae804nj8fchv0z3472gytcvx9nmm7fn7ymlzfv7xzemymqdzxv3rtr8z80jxyyy9u8f")]
    [InlineData("English", "safe mixture exhaust worth smart level donkey orphan hello ski want runway example fame gaze thumb voyage swift wire common move injury promote media",
        "addr_xsk1dralhyf6qtm92vuy69ydkzdpnfkrnknzjp3v9swtajhx2cl5h99m6x3xa9206l5hrdgzu9hfglwcag3nnhssnvn2wr82p72pcj0286lw9wj6ltjtkg4xmfhscsc2ct9kqtjusgrnz9jy3k20mz75vqfnycrh09hw")]
    [InlineData("Spanish", "vitamina banda araña rincón queja copa guiso camello limón fase objeto martes náusea matar urgente sello emitir melón acuerdo herir litio pausa dar paella",
        "addr_xsk13re67uydjzhkmepyn38kha3zufklq9d05uns5wm24a4rghraearuhhvswmgrfyyvazuhmys7zzxde86pyy432hzz8cp6498tr35c279yuutch8eqhfd0vyw643j0hk8pylam4c2xgjdpujx05p8wp08pvcuh98jt")]
    [InlineData("Japanese", "おうふく いってい にんか さとおや すばらしい ほせい てはい めんどう ぎいん にちじょう ほうりつ しなん げきとつ ききて ねんおし むしあつい げんそう えんそく ぎっちり さつたば ちきゅう あんい すわる せんか",
        "addr_xsk1hz9gnecunr22qvf8skne2arqjunzx8qu0jrw5mvzsglnz9z9j4d40jwrjyl5szt6v3pmqutrkv9cysmapqgf7vwgzcuh4h6mgkkcnp3zyjz09ucqcq9pk7kj30r5u59unnxfla868wy9w7x3fhacexrhpytev07c")]
    [InlineData("ChineseSimplified", "驻 歇 职 熙 盐 剪 带 泰 轴 枯 奥 兵 曰 桂 导 睡 抵 宁 容 读 幻 悉 默 新",
        "addr_xsk19qeumuvj2metz4tjz4fw473ggka0q02g2z5rdd4r5swk93asjedn0rkd72g4lhsam59332tjmqlgh55cn3suxfszkd2euz6awnkml77jmelmc6wrjzwsv2wf9320jdclsju2xzdggxl6sg52ym3yww70a5efzedz")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_Defaulting_To_Zeroth_Indexes_When_Account_Or_Address_Index_Are_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic
        };
        var zeroIndexCommand = new DerivePaymentKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic,
            AccountIndex = 0,
            AddressIndex = 0,
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
        "addr_xsk1sqltfrmwph2x9qarf9h3dtnpeuyavq34d2y2hqd5qfq2hdk5sa8jyjj360azt08e30zx5sh5025syp7l5ah9jsjxk808p9lgn2ag99ttp849evwpy8sdctu4sx2vc8sftdt4g8kfrgdxvrm2f6ul0l3l4uycfw8y")]
    [InlineData(
        "English", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame",
        "addr_xsk1uz8nprlx2vk5et594g4k9lu2c9lc7rl2hqml5dkxrmwrrjgzrdrc7s6q4t35ynwzjylcnl2wjr4hcg0te3mpu8jfpks7he6ly4fw3tl7crlckxtjphnuyl4tf9l6dlh84jzhejxm9vdh3drapdxapsh4cu4rjwrd")]
    [InlineData(
        "English", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird",
        "addr_xsk17qemn6k0cq0hm28f9r6xpjw459eae7ey07pn28p3akpnz6rv0ex2qavdnnqvmdyk9palrhjf5u5xk53k52zmsn9qe3au0my3f69ynk8gca680qzhq8zl5tmnaa0ysfuucejzmynj5xvcw7pmqmszfnfw6gv88mpw")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco",
        "addr_xsk1yzfgyh5crcz0h2k8enjatjv3mxkz7z6cnjpvwmfwv9al2dupr4w95rthz9xydgtpywce9k36gnsnvzx3f6y869azh0mltcnuzt5wt36uxyx0nsfgvdfkryysecl0azr7rv47t3adta8tntc2lhel9zl9656agmj5")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ",
        "addr_xsk19zw604nedsaq8tek9lw4feqnxgf93pklqqnpl820r5xk36j5gewydgualjrsy6v4yrsrmltmlktw8tpltx0e2trfs0f5jr7l5ysyu6y5rnszaqe8w68d0yxsz6enxjhwdgz2tp2lqyh75s8kl2pr77k3eqc587ws")]
    [InlineData(
        "Italian", "sfacelo quadro bordo ballata pennuto scultore ridurre tesserato triturato palazzina ossidare riportare sgarbato vedova targato",
        "addr_xsk1vzxza7vtsm653qye7g3a242yqpvjymuhf4q2lsxqrj6mm6aaadw5nwzp2ypjjep0aae8etnn5fnqgyjp5jfr8kpqtsdhx6f5qn6m09klrxyg66ukzjqgwc7dypc8t4zfhcmmduyjfquz62heml2ehe9saq7an6at")]
    [InlineData(
        "ChineseSimplified", "胁 粒 了 对 拆 端 泼 由 烈 苏 筹 如 技 防 厚",
        "addr_xsk1kq4fld3l75dhnce0z22vn8ge0z0w7yv6qxedsdsju9y4422tnfwl7lflvya0kr7r3kxdzx89rs5y8vldcl6zwuuvhfx4elwtlzj29d7hvnc8rke22uc76q33ex46qz5pc4m8y80mlghjatk8e6gmcx0v0qrawg2v")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_Defaulting_To_Empty_Passphrase_When_Passphrase_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };
        var emptyPassSpecificCommand = new DerivePaymentKeyCommand()
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
        "addr_xsk14qjvenfwld792m0cjk94qhqdwskgl9tx6kpwxx3apd56ap32qaz4ey8xhdk0hnzxkqs6edz0vlk4rshamqrm47wdg8z2ccrkrr7h2kxzsrl8e6kqkhj2hlwh6tpgfurjum9r4qnv280ew7fps0dyug2xfcpfn48s")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1rqfuacfx3vr9ldhh8gvleh4ut5yppv46xcj2saylhu7hukql2pzchcf3jj5ncpj2xlexr7urgdqvyx6ejafgp3rs3yzrryjtyflw5rtl8zpgnxzurvtprrweyskmtq5lxggvuyx35stppwjkwgv99ldzqg8566st")]
    [InlineData("pass", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "addr_xsk1jpet86kqq9tlz65ysptfg2px6f6c97a9r78al35yd3cgffygndy9vwyqu5r7s08xtuye2kd7uu3mch90gw4pft4zkx9pjuwy99cehxmuss4dgxw5eq604mmknjfu5v0nnjculxxcl5lzrvj5n2lv7l7fhgkvj0gf")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "addr_xsk18z4qye36ygmz7ye2zf4280qwzqfavpl9d5h50mpgv3v3qd9udfflgzz7xdm3n8w4n9juvz6wfzw4laqknk4n3d4sr04qvl0sh0u5xz0d8grpr6xp0yrhu26cmr728ldcyj7ndeqfm2natdnjsvckcypd9chyv5m3")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "addr_xsk1eqsek3s79wauyg23kj928knjkkq69w4g6a7z07ck7awrralgn3vdhv9qxxxcqa32s00nj27p9slmks6dg4jcae0xxts2tx3snpakpz9na78yxpjccfe2fyej5v2y2pgl9qshay9k7f9a4ur8nsrerg589sp4knqj")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "addr_xsk1xzcg3eg8d9tycn7434awna2mnuaex0ufjrkfgw8ekurmz6pwvdv54xcrfr95u997pj7ds3sz8kkt7jg8altq8elhgtrlapkmfmr8vpzgu87vsuaymugw76q7s970fz0n82tp273g8hcspt6va64e2pwfysr0r0ql")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_When_Passphrase_And_Language_Are_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string passPhrase, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePaymentKeyCommand()
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
    [InlineData(0, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1eq6alt578rfvylf8wvs3yvwdgu6gp6x90hxkalgq2evs9xrsl3fv3gszsell94j2uggl9hclf7lv7x03qgjz025j5aclp8eryy2t9vcqlmqjhn7tr4tlpafsn0a53ydx4ls2wwpz47zgx3uq5s8zn848nv8y72jz")]
    [InlineData(0, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1nzrsemw2663tg8hpf82cvmdak7ah3r5vua7ujddphjea39tsl3fdvhzgfyfxjwqnsnvra7m738d8wk7qmq8e6zh3lqw5aedjn43efpxelq0vfk9v6jkrkedlzmzn2vyevk9wh4uk36635dgttkzlz2yzggtm0f36")]
    [InlineData(1, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1xpa257c6fg0evfz3vd5nje96guca8jca5ghj2m4wry9nexrsl3fzl4tvt800jn9grxyan0qn2f09gdfe8j8nf7nyr5u7zd8txhfdqw2adl6nqv7tjxdwly328l4pzhqzfm0xq6hfp64savfx2j0yxtqruvj5tl8h")]
    [InlineData(1, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1gq3c3e6alyasazyfgp46yk2mnkndagqqatcuauxy56ktp9rsl3f9nq6a6cnt30zpx6unq47223yulm47kqfkwa3w8uwca53acg8e8e2rpcrdzhze2ga42utauj4ydk0n82mahutvlvngj4a5g5e5njzqngepzcqu")]
    [InlineData(280916, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk17p9mz3evgpggkum6d3wdafldsyt93w8lcuvvm2c20v5e4yrsl3fvhlcg9xp2p5agdz907sll9cssagpd5zj4cykc0v3gylrkcy5lrcnem6apes6yllsmdvxyuw2m9h4mcrnykhf4u54pdpj5dp7chvmfeus0afvg")]
    [InlineData(2147483647, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1eprkxlxvn8a2hdxv34ak9fyqs7a46t0ejhmspg29r2pz8ytsl3fy2z98y622ky42qcynrespx2k3v50zrnglsu0pvk0w7q6strsqqyjd6m9k6jherzk0dkvqh23jzqtvjh6fc75hvwxj9dmxkswtnf47aqzw3qau")]
    [InlineData(0, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk19z5hple4q3hdvalqwwd9xcnxf33syddjr9n9adq7ut2g3xnsl3fwalnv2hejxcjtdsmgs3eqylpxj50m2e7h0yf0yuwkgg0gd5ldr3lk0zuv6ntmgu9ukhs7tmeg2s59r3d7adhw2tdv7x02wrq5265a7g65607n")]
    [InlineData(2147483647, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1xpsx6thl6qx2kdep0frldxfwyne6v20atk45swyvyhgr39rsl3fwwm9kwvzk5sgsar78eh540a63a7445jv2scl40pk5yn26wrf6awrd4s3r57cn05a7svfluzednpgq3hzwr9am7vaa53vxzfldqadruvjyw870")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_When_Account_And_Address_Index_Is_Supplied_And_Mnemonic_Is_Valid_For_Language(
        int accountIndex, int addressIndex, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DerivePaymentKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
            AddressIndex = addressIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }
}