using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DeriveStakeKeyCommandShould
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
        var command = new DeriveStakeKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --mnemonic is required");
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
        var command = new DeriveStakeKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be("Invalid option --mnemonic must have the following word count (9, 12, 15, 18, 21, 24)");
    }

    [Theory]
    [InlineData("invalid", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("Australian", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("en-GB", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("Klingon", "shoe follow blossom remain learn venue harvest fossil found")]
    [InlineData("english", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported(
        string language, string mnemonic)
    {
        var command = new DeriveStakeKeyCommand()
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
        var command = new DeriveStakeKeyCommand()
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
        var command = new DeriveStakeKeyCommand()
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
        var command = new DeriveStakeKeyCommand()
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
        "stake_xsk15z26egywy2y70cd50jpx88ljucghl5q7uln6crsr9md209nsl3fpq2dcefv7qhp22v9zhuu53mhqsqgrt0tw6secexwrmyfu0tqe0v6us7py79708vqmyfydc2hcryt3na5zg5nsv8624xvqyw7596gjxy53a2pd")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain",
        "stake_xsk1hzf8uqsdfg2rw2xd5am046klnsau6zfvjfr60485eak2y4xj5dzmpfrshk7eqzsrmhqlzwjp7f79f8tgt9j6e7yrj3c87mvxzj2pc8g46wkg7h9y2xfzzkc4kel69zrsw28wryt9flah22dc7lqz9mr80grdg2kq")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity",
        "stake_xsk1az73rmjyrazrxu9z2l5xva6py2sld4ul0nvn88a7exfy47egy3v5ymv2u9f5d7mc8urawhlnnknzw4s56zjqukdkl86y7vw76ag6gzl2wsmp4gas6muj30kgckfw8yun8ympvzhk7qwydvk4eu0lec7gzy0573q4")]
    public async Task Derive_Correct_Bech32_Extended_Stake_Signing_Key_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveStakeKeyCommand()
        {
            Mnemonic = mnemonic
        };
        var englishSpecificCommand = new DeriveStakeKeyCommand()
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
        "stake_xsk1gzxhv32rqu4df7fzzdv9wkghr9k706d30g3gy9dt2rafagk6j3ffds3v6pp86vavh7cz9mv0p0s6fju4dejyfp93etswcx9jazsqd8gyrx5482uduadk9erydncdm6x7hcs92a9dz7scnyj6qcc78detlqrcrkw8")]
    [InlineData("English", "panther cave tornado remain snap leisure gown vote strategy apple away room shrug action reunion",
        "stake_xsk1jzh8y6ys4hczkrjwxv8cnujfewpzlfj3vs9xa89nvlcux5wcw49hkqepg6uraxw0zac2zf2f92e5c4nen39vvzhzf0dlhltjchkhrupx3dkkk86z6y283e0qrsgr3qztgzy2whdtryv75kqy56300lhdxy3jczw5")]
    [InlineData("English", "safe mixture exhaust worth smart level donkey orphan hello ski want runway example fame gaze thumb voyage swift wire common move injury promote media",
        "stake_xsk13z407z8p7w9kneftmdjfp3q0jrqe84kcymwvuzauj6w05el5h99mlv6he566cykay9knu685vhs25sq7n3dj57zam2nkznvugnvnc2vmk5aeafzjzyufxrz7cvpk4qsekm9g794gcshxmnen3ftj2v8x2gwlswu5")]
    [InlineData("Spanish", "vitamina banda araña rincón queja copa guiso camello limón fase objeto martes náusea matar urgente sello emitir melón acuerdo herir litio pausa dar paella",
        "stake_xsk14pyqq4zkzl2yanh3nwg0ggyw6dsgvqtxhvv22jfyrmrn6hnaear7ndd52h05y7xqlre7cxgk0hqm6fpxhqn36l5m06cc4t30cw60zwwrs50hepsxdc5kdjauxcpmfl5h0mcj22k0c0ftls7mcum68n87wqfw4tq4")]
    [InlineData("Japanese", "おうふく いってい にんか さとおや すばらしい ほせい てはい めんどう ぎいん にちじょう ほうりつ しなん げきとつ ききて ねんおし むしあつい げんそう えんそく ぎっちり さつたば ちきゅう あんい すわる せんか",
        "stake_xsk1mr7za80705grup84dteyyrx75y24layer37yjtupxmdmxy29j4d5txvh43lcqw86lwfvc9qazxk27ug8hhqzx3yufx4h7jpzpfzk0ccyh75swatw2rfcng3ae39tl54pfetadmgvuk8nxcepa7a6u9ag8uacdgcn")]
    [InlineData("ChineseSimplified", "驻 歇 职 熙 盐 剪 带 泰 轴 枯 奥 兵 曰 桂 导 睡 抵 宁 容 读 幻 悉 默 新",
        "stake_xsk13pvxplydmrjjqezzp00v5kjauzvlppkjdwlg334gl09l8j9sjeda094cvm0zktaptt2wj2xwq3v9twj7vednc2szfug7zphszqtg25sf5kgpg30s2m3eetfzpkfempz8uhuu0rp58fa7qppuly7dz0ta4uhf9tz2")]
    public async Task Derive_Correct_Bech32_Extended_Stake_Signing_Key_Defaulting_To_Zeroth_Indexes_When_Account_Or_Address_Index_Are_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveStakeKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic
        };
        var englishSpecificCommand = new DeriveStakeKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic,
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
        "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        "stake_xsk1gq08d6z56vp7ctgk6nhwtgg8qx58la3v4sxvk9zc6zyefww5sa8l3a67ct9qu0nhfps5n979npuqjdlacwvrzpacd2c9w0v60y6p65rmssrepcz68ejre3lvgstpmameu7x2zgp94ravysr7xq9f5dj7gc9hcymv")]
    [InlineData(
        "English", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame",
        "stake_xsk1xzmamtymxfuqplgam4z3znhwzw8yxrfz5mseuk46dv3wpnszrdrapuckecqksg9ah6ujuv3zs0y35ajrau7l5kr2aag00dtfghjqgwe94ydzgsg3ravdhp9a9df9gclnhdjg2sp2akegqxw0rs6audsglghglyk9")]
    [InlineData(
        "English", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird",
        "stake_xsk15z3x5mdtdet2xs0xxcnkav9dpv2jt8h8tc84nsg86fx7xmtv0exwu5vlk2rh0ak9wp93r5a8xv87mynl6pgvzzpfhpa79vruw0zx9e0fd6jkcferd0mv9qpsewca6hy2ewastp6343gqzh4s0f6g8knjgu852dg4")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco",
        "stake_xsk18zxzvl74ej800wxuyfffmpluew3wyvmcmh7h9jgm0q0m6sypr4wfwes2s09eaut70tgszxltf3fqpwsm0caaqf99xylyxvvdgrchwx2jdkxutrz2dvtxwfvpf4k8akayp7fcruetq82pvdsss4qzx6xpccejpptv")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ",
        "stake_xsk14pgl0ydhpewq23vthwjlyut4gvc3xvhuv88st4ugkf02n6z5gewv9htku4t8z53jkv8p2dvepjh3d99909lfz26u8rz0myhjm0rfthzlcu6yc3xtjhu5n6p4j4mg80excrfgquxhfanw3d3d3fz5td6n9c2m4svz")]
    [InlineData(
        "Italian", "sfacelo quadro bordo ballata pennuto scultore ridurre tesserato triturato palazzina ossidare riportare sgarbato vedova targato",
        "stake_xsk1kr9yfn2050uatef4sv5jqlmf8cu0zrzn7xka7edq0jq4p6daadwlgc35sj7g5pxl9rzkwc5mft7g78vqlx79svk4n0vgma4ej4ukppsfl4wlypnd84p8x373mlc5z6pmu8je5e5lg0pfuufkd7lh4g4wly0hjwu3")]
    [InlineData(
        "ChineseSimplified", "胁 粒 了 对 拆 端 泼 由 烈 苏 筹 如 技 防 厚",
        "stake_xsk15p03r0v64vyq282pv0cf4csjfjmkktx70k8as9tlq88ltf2tnfwk5t09n0etgqnw52rwqjdr7nhj657vmzcqlxxs6rqv62ca2lkq223x7futexaxjns7mzhd639tnnf834tyk5t6msmf9j3tsswskjnpa5zcw7ef")]
    public async Task Derive_Correct_Bech32_Extended_Stake_Signing_Key_Defaulting_To_Empty_Passphrase_When_Passphrase_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveStakeKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };
        var emptyPassSpecificCommand = new DeriveStakeKeyCommand()
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
        "stake_xsk1aq55va6vkn0es87wfm3yeyc8vxgy64u2symhkg8d3alunze2qazc6zqf8h4q34er7s2qswqqdlnnm8uhhu6qh8n5xhfz2x7z78hxn5xa9pj7lawqxkdxka47ducfh0z282qpj0glqwm5zylk0y3n74pcq5jtqz50")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk1azrm8cer9ygu3luarksyg8e6d0c6kv8sfdg92f88vpm95ksl2pz3u4rzny5ltw45mpr8yaduexerkf38rnvp36jm3qe0pgjtsmd0705eydysxuqy7y54a85u0xjxrsy4zphkd8hduzdwcjrhc9zjr6sgvg85y84k")]
    [InlineData("pass", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "stake_xsk1pp8r3dx22qwn2wjfsaqu7qpjlfjd50gq8eulpzgm8wvca2ygndytxs92ewgqmtvmve03z5njurrequdzch2qeaht50hu68ql6qpfxp3llelr4y7dl2hhkp943x4yr9x4v39p0txcukauxnqj0n4za8lvcc3rg25j")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "stake_xsk1rrefx40vsmf4nrccvvlnx2xld9twpu6v46egcrqls69fcd9udffnftg7rqmae7fzn266spmevr2jqw5465c6sczu6w0xfzz06m90289kazju2ldfkzhaxktltde8ll493y0zgugmw9wc8qd5wtqttndh4yvps4sp")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "stake_xsk1rppkyrlpg33uph2tsscd8aksh7yaun6uhc4syutqzwh2370gn3vfrrvklcp7yn9g05qekcwr057xel7l4s6se8d4l78ydu0nx9m7h54kq4c3ju27zuh2pdyut4faz5keqwmkngjc0xvppu3gladj739h9vwt9pq8")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "stake_xsk1uzr6m522vc24k6cqschjc7c34pnuf3m535384j0p9thxk63wvdvn44gm6kpl3mwkjrqxc6y4hlvdvjfmgr7tnpaecpcnkzxpa25phu4yj2l02eldftethfvkxv2kudwwk4y8lzt85ugycflac5awjts6sgys5zzr")]
    public async Task Derive_Correct_Bech32_Extended_Stake_Signing_Key_When_Passphrase_And_Language_Are_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string passPhrase, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveStakeKeyCommand()
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
        "stake_xsk15z26egywy2y70cd50jpx88ljucghl5q7uln6crsr9md209nsl3fpq2dcefv7qhp22v9zhuu53mhqsqgrt0tw6secexwrmyfu0tqe0v6us7py79708vqmyfydc2hcryt3na5zg5nsv8624xvqyw7596gjxy53a2pd")]
    [InlineData(1, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk1zz9zm6386lnttyqegxygcekfj26vymz82xqh94v2p2m2lynsl3f8mturlcgp0umq6jlmv2cgyyfs8j43k56hhxskh0qjgnghs6r94jgp7j4nqmk03f3mugd2dqj7p668fpvfrsv52p44ffqs788hn0ehuqawqs46")]
    [InlineData(0, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk19p5gx4jef904a9a2223hnqjzjw8je0uqvae0wfnwgl8lhytsl3fy80d86vu35h4tg0v3eedqapyd78qrz0l58qp6hxqppy35srvm5w47jeld5a3pwcv6f7vhx2ju3scctj3xhu38pj5dvam89ykme4pewy5eh25j")]
    [InlineData(1, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk1jq2tcshfsajht5kdn6023m0kh5d6svqm2aeq7sh0uwh2rymsl3fdap745jvey56sf0x7jzhlz66924lwtlepy00quzlyc8au2fgmq623q609uuxg55e2pgw7rpmmraku5h0wa8dnrz5k6frryx6hcnkxjcxp97fr")]
    [InlineData(280916, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk1uqnu3x4khtq6qf93ruqavnq56a2fetuljewf900kt8g4artsl3fdavqphclju97r9lvp4zq37g0k5vhy3yezyzexylzsgeav8ykucqs6ue6a8mh4ecz8vk97x4jv8rp3fktdvh7j0qv53244ykf2ucjnsymaulj2")]
    [InlineData(2147483647, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk1dqk7qf7vynastd3393fcnvhr4czxfsxm2t3pngqdfwzq49msl3frge0vgj6ln3u0r66fhagrsna065hx4vled9gx0earzwjxxdd0x75r99jdujv8hrf4xeadvyuwyspftt2nm55unl0ruwtza9459y7wgg45zskf")]
    [InlineData(0, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk1ypqydk5edhfnm4pmke8qs7dmkfsfz9jt49xcarg4nrgrhxtsl3f0j4dg677ryl3gphulnf9wmcwxrgf6es57tjk5m26gqgdx52p00pu0w8eknln52q5fq9rh2p0yc92z2f4yvxvllj05jwmaphjdlnlq5vudkmd7")]
    [InlineData(2147483647, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "stake_xsk1gpffr57em4sat3whw55cjnv9r8vhfwldaf8m9p7txa22rytsl3f8gtjyxp02qpnkk2j8as9c69yzgz72jn6jze4x6pe8v8yqmw3qnl8a03wdh58hgs458t0qmzmsntsqt98c62xzd2c0p0s2j2gcpdxqzqa244zs")]
    public async Task Derive_Correct_Bech32_Extended_Stake_Signing_Key_When_Account_And_Address_Index_Is_Supplied_And_Mnemonic_Is_Valid_For_Language(
        int accountIndex, int addressIndex, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveStakeKeyCommand()
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