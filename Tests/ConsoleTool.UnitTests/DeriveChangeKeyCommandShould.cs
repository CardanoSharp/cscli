using Cscli.ConsoleTool.Wallet;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DeriveChangeKeyCommandShould
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
        var command = new DeriveChangeKeyCommand()
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
        var command = new DeriveChangeKeyCommand()
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
        var command = new DeriveChangeKeyCommand()
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
        var command = new DeriveChangeKeyCommand()
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
        var command = new DeriveChangeKeyCommand()
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
        var command = new DeriveChangeKeyCommand()
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
        "addr_xsk1zpteqdjut24wfax25epml2p26z32ftfdntuwymwg8gxh9ymsl3frnrz4ydh09t6t40unnjhudvfd33qsxymktxrw08202fgvhu0uueehkspvxy0cwpjaj8t5numd3mlm4jdz3tnzg0tynjyhafem7k7rycd0fauw")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain",
        "addr_xsk16pvgnqwpctk0wnyxghj3qr9jev9mzsn4g4yjc6lqrn9n24kj5dz4pv3nr88jn2ae2zmhl5n56atmdt55xl7lq9qt3vvelgv340avn5e9ns6n2rpfnqpn3gt6s376wc50spkjd57urwkfm8qq9m3lwdysug3ukla8")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity",
        "addr_xsk1dpnz2cf9mfkpfulnaskmzfstec9977jj8uxamu673hqspl3gy3vkzeqr9v69uc4ewpndkqvte4yl7umr4h7r6a67uq77j2sv56wam4j7s9pglvyexyp84swtkgy5fjjsfuw3g6l3xufpk4w3tpf33q49hstl359e")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveChangeKeyCommand()
        {
            Mnemonic = mnemonic
        };
        var englishSpecificCommand = new DeriveChangeKeyCommand()
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
        "addr_xsk1zq89tv9xxsjx2eqfpfvd8hrrczkfmd0znswgdzhe3mlvf276j3f29vxk70scryravmzwrlrq4y724llnd3rqlvlyjqdc5wx2v4p06m08s5f77ydtuu3u6yql806yxrrw75khyynd4rvhqvpdy4tfvuxjccdaw2k3")]
    [InlineData("English", "panther cave tornado remain snap leisure gown vote strategy apple away room shrug action reunion",
        "addr_xsk1hrq497unsspr5zaamxhgc0ja9y687wzrxywmnzmeunayx57cw495uw56p0weu40k90uk63m0v3gs9jz4gprrxrudvkkhlc8zwx97agg5wz4s8uhudrf0mmzljzursvcwzc0qjw0pgwuurstqur6rqp47zqka4xzt")]
    [InlineData("English", "safe mixture exhaust worth smart level donkey orphan hello ski want runway example fame gaze thumb voyage swift wire common move injury promote media",
        "addr_xsk1uzw6fvnrlnwvfu83p0zhyw4gr7x60ds3mc8r7lr9g9mzcc05h99satwljdsy08egw8tjxq2t8ms7vczqdkgkatv2fwj9xh5k345eq07p4zt6zhdd6t3jlxu4e5cwyjqgj0jvlf2slu2ejnzcqq0gwlr8tcqg6lwx")]
    [InlineData("Spanish", "vitamina banda araña rincón queja copa guiso camello limón fase objeto martes náusea matar urgente sello emitir melón acuerdo herir litio pausa dar paella",
        "addr_xsk1np33pz5qsapjhs6j3sruzmywnzlzhd0pnptexrg64t405hraearjjdcfmg9yfnuelcfvqk6vsskrqjund6kfdug7z0qelc8sc2y2p677qjft9zm777zu3e7x33yrnpdta388uar3h6avmqmfn4lw98hajc3w6hed")]
    [InlineData("Japanese", "おうふく いってい にんか さとおや すばらしい ほせい てはい めんどう ぎいん にちじょう ほうりつ しなん げきとつ ききて ねんおし むしあつい げんそう えんそく ぎっちり さつたば ちきゅう あんい すわる せんか",
        "addr_xsk1gqmdq29st3yu5rypca2l8zewey4zul32pgzseu4q4307kyz9j4d7wd04gg6mn30mlj459sf6uwfvnx25xquszp8q3dhut4x5qr8wuwxnvrcpyn0sz2kne6taz40ea5p8sm2n5p045hnxd4xx3tusudg6a5e6xkyy")]
    [InlineData("ChineseSimplified", "驻 歇 职 熙 盐 剪 带 泰 轴 枯 奥 兵 曰 桂 导 睡 抵 宁 容 读 幻 悉 默 新",
        "addr_xsk1epm82fq6jvpde0rd620snpagtkcj0sz2e76kpw8djk73tsasjeda3dawlgmadn8thy23ql4jdx8epw8gqdsajdelf3pwrexn5yzgaufk75ehhgmjqh80du0pv2769z2pxwmf9hatpypym6jvctydzalneyvsmj3w")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_Defaulting_To_Zeroth_Indexes_When_Account_Or_Address_Index_Are_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveChangeKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic
        };
        var zeroIndexCommand = new DeriveChangeKeyCommand()
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
        "addr_xsk1tzzukzsv3hpkxncsrkle0c6k0669pugtd9wxsnneps62pdx5sa86anpplw8xqdgp8ss7594jt8d0zqk88n0e3ejkw5glu7thdprlv8ddrsm07zp2r29llf6v6mm2gugn68l59aglv0fs0n4m43q5g0zwuu7x8ll9")]
    [InlineData(
        "English", "wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame",
        "addr_xsk1wrcy0x2d9jh6j9kfmcchd6yr03xdzrv3ky4zqqtj90k5fnqzrdrmrylppwxdwtgn93uqln6nve7tal5q3q6vk74m7tqpystdvwnktkkyres87danfk8x897y3xpvuhkxj5tjsag94p3syka65sqt6d5ghyfes3h7")]
    [InlineData(
        "English", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird",
        "addr_xsk1zqkeclfsyzryr23gfr5lkqh9sxjr24a8w4nx0h3pxwteg6nv0exy68c773ju6sn52vvyc3mmely87waj2fqava9gx6p3rprje0n227ddfjptc0u0h4s2esexw8lnkwwfdyvd6kx7d0mkct90y4ja48rxagqxw3zz")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco",
        "addr_xsk1kr6wekxrwmpp8s57aye5gak8exwe22ql5hfpcuhukpwlk0ypr4wtwczfvyh47cgckx7tc2akx0u5n67xn29vg9x8kqsx7wvyrv0yee2v7deqddsyz8xhh8yfazrqcuvrngegz2zmk9h7gngqjhq4c899ncmevvjl")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ",
        "addr_xsk1ypfncumd9ktlkldy3fjjfrftyvjnwr9nmxcc60g5kdg4lm25gewzz340z7ndsvdmh92377xwdzh0g9pux98csas99shftzm20p3ltaw7c2zp2028yhyp60qfkul2nqe69dldwqsw3rhm8527e49zuvurvch0a3ha")]
    [InlineData(
        "Italian", "sfacelo quadro bordo ballata pennuto scultore ridurre tesserato triturato palazzina ossidare riportare sgarbato vedova targato",
        "addr_xsk1zqtsenj9axcmxmn7q2fvakfkmq3uhmxqnmzff2x2zh08tm9aadwmgj8m489uk4rwmum6cktu2fsyrska0en4ua7ru3d45alkc0wukcqyc6vyg755tf9rgah6r3rzgys4er8svvdylswzw9mc5yxj4t909gzawahn")]
    [InlineData(
        "ChineseSimplified", "胁 粒 了 对 拆 端 泼 由 烈 苏 筹 如 技 防 厚",
        "addr_xsk19z8h2sh0rsaj8mv0k05wus9tvh29w87286sf3d9p5zgkl26tnfwhnlcrj70tgup2wqazu8htksc4ftm8mg89tn0c650vzlruaxm4sqcs5lslepke02w2xxc8mwj2fmtv26ntshje6thkey63ys9rm3cu8vrg4kdc")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_Defaulting_To_Empty_Passphrase_When_Passphrase_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveChangeKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };
        var emptyPassSpecificCommand = new DeriveChangeKeyCommand()
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
        "addr_xsk1tzqpmpy5ufktx7epuql0mlx7r62gutaxfwatnzfe37u9lrp2qaz7fm8p4l74s53xv06q7f6f0h7qs9a9z5fck8al59atrhdtv7stdruyy6tj6pl46k077pw47qxr8gwcud465hvkek2ve0aj5hsw24h8rcz6d9d9")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk19qt2y70trqtsra9kjwhgda67rmqvnkng5ht5wgr57qgkx4gl2pzjdqp8xlhnq2qqlvvtpvthk7ej7vg0zvwazhxd2p8ftkjq0qjufmzra5anpru7f670dddh6mg0dymu0d0nke5yuc99s5px4qlwvq5tsvjs97vh")]
    [InlineData("pass", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "addr_xsk14qzx8eya6ghmvcgr3930g0etf59z3e70j200r5u6lh9ga85gndydv2ehkk8sfa944g6rsu22m4rek03gynmct3jc0jycplx8nx7fv277aqvk2kpsh7mqfkfgf35a8v46vmekqy3pqpmy00gppxuag3myr56dfq5m")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "addr_xsk1qz2gr5kct2am8p9pmva38hq92lck5djd68qk7wg9fgejsv9udff74nnlenht0yqg2fd38uhpan06r9s0e6qwqtxwe7h3234m35r6td5vt34gcvq0xj45f5ta5tk066y7nq45updn6vlfzu8keeusqj3w4gj5ap4s")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "addr_xsk1xz8uk60urfzkpcd8le57705wndw70pdkwphgxpz5vv4ula0gn3vtc0c2sja005mm8cgzpdlnhmfggcqk5n9060j64dh6tmyhwlatlk7wjnusa7r6wcszggz88j6n84a8dwah9tgsaknpn2vwtesyqs5gkg552adn")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "addr_xsk1lq0l6llyq4qeky930neac02qjqpxgzdsexvnpn5g85n8ceewvdvlpvycskqkg4rawdcdjq2tl5wza5szcm8x7quxyrv4j092hhamqs63pfquk8k0z6galynpgzaw6j7j68y2gllqk7ldpncr548ke5qdjv2vr0dh")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_When_Passphrase_And_Language_Are_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string passPhrase, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveChangeKeyCommand()
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
        "addr_xsk1zpteqdjut24wfax25epml2p26z32ftfdntuwymwg8gxh9ymsl3frnrz4ydh09t6t40unnjhudvfd33qsxymktxrw08202fgvhu0uueehkspvxy0cwpjaj8t5numd3mlm4jdz3tnzg0tynjyhafem7k7rycd0fauw")]
    [InlineData(0, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1vr98e3srsscjx2lavpxkpcldg464yv2j2nr0f2q7h4krt9msl3fwlcfmfs0nevhvrpepw67gy3cy2au3trtqqrl62wnnzppsyc9zlhk0zg48qmkt7fc5he6vg4679ejhya40hteu0ln8paztq8qaq69qfcel4nx2")]
    [InlineData(1, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk15q3ejtysjfpzahkunttz0942unum4y5kgmy8zddnmd7wd9tsl3fw5xanzs0hrase0vwm0njwl949nzumq4p28u7vp5wm4g9rx0vsz7anmel0e0asfgrrqwdplr4zjq5s2pyhjn25vrgaxwpq775zlhn7kvwk8e93")]
    [InlineData(1, 1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1jzzmrjs2r3q4ke92t5m7vfjshp5s5g4fdmqyyd825r8k49tsl3f8mpztgpyvwqtvaavsaju3ex6km3f35k30e8la6zj9ktr00wrqy9tcdvmwnuehp0lywd4t9y472lvdfzsx43kuxegut2r4palv9rtk0sdp55rj")]
    [InlineData(280916, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1cpzzkhk6gd532hrrkq44mdkel3d2utqeumljskda8s4xr9nsl3f9mkv87ymqh78060qx2m69zzau66v00auhfml27tr9ge0rq7jp9pf3j96azyllrvdhvvk3d7l3sc9vm46wc9a45ju9ldm2c9pax4v2y52v2yrz")]
    [InlineData(2147483647, 0, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1nrhqrj5f9qf90d3v8349slt840mh46wsfz90ny3262atdrmsl3f9q66052uymt25scgm8ezfevm0x8lmvsmdjz0qjkv2h6fq6stx2mrkxv3xwp7kf4pk3rvwmw8uhjzcumwave6u6xjd64td9c4w6c8npystwj5s")]
    [InlineData(0, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1grkawae7q7sga89d2hpcw2rd5qafrv38zk2nm5n24xl2fymsl3fpqr2cfsd6rpasp4xh3smzkrejglzvg6es0f4d2ujnzlegjk63wx7m63dtpu52hu79s72x9w44urxez57ejfm6hk2lpwrxygkvvg9wtuu460v8")]
    [InlineData(2147483647, 2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "addr_xsk1wzy5paxnq6m0u2xlvvfa77wyzga2ysftql64fafz458tdytsl3f9x5aqsmuemcaatrkd5j064ampn4l8cs8znpc5v3ml97jza4p5hhxtd74m6juqrk3nrwwu0jxmfjquyjylf45qfms6r2xuufhmrpuhqyzwekkl")]
    public async Task Derive_Correct_Bech32_Extended_Payment_Signing_Key_When_Account_And_Address_Index_Is_Supplied_And_Mnemonic_Is_Valid_For_Language(
        int accountIndex, int addressIndex, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveChangeKeyCommand()
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