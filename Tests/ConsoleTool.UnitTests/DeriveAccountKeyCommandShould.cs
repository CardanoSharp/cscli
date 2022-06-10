using Cscli.ConsoleTool.Wallet;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DeriveAccountKeyCommandShould
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
        var command = new DeriveAccountKeyCommand()
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
        var command = new DeriveAccountKeyCommand()
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
    [InlineData("eenglish", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Language_Is_Not_Supported(
        string language, string mnemonic)
    {
        var command = new DeriveAccountKeyCommand()
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
        var command = new DeriveAccountKeyCommand()
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
        var command = new DeriveAccountKeyCommand()
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
    [InlineData("fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "acct_xsk1hqh2sk2jc6cwt20qypzdq6qldkxwyq8ydu97sxk0xwm6artsl3frtqnk4w2zlyr906g7udqy8hpz3axuc4yu4qd5uwhzmht2sg82ljhjf7vtdg3kajfural6u9exd234payu4s782fqqssmca0xle5r46cumaetd")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain",
        "acct_xsk1gzspgayu90wczfueqz0m33mppwu5txdljsur0p7r0vnygnxj5dzjaztfrute8p27acsnxrfhhwzupsrjh5md2tm9pdf4rfc80h5ph5gwe9m6nhdxtuwhe034r6nwxmyz7yschrqre9epyak7u0l0q76tccrpw8sv")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity",
        "acct_xsk1vzdtzrrw0x0veeru95aa9savc5lak2kx5qhcdpgt9davpuegy3vnjwpy25q3lwmjklezqjtrga5ftfgjfj2tdwers780rs05qz5rtgpt8vsh08uxzvlyum529fgwgsgam8xnsthjcrvapw8fx43vu5n5hup0ynz8")]
    public async Task Derive_Correct_Bech32_Extended_Account_Signing_Key_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveAccountKeyCommand()
        {
            Mnemonic = mnemonic
        };
        var englishSpecificCommand = new DeriveAccountKeyCommand()
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
        "acct_xsk1mqx3kdpwhwa4ul877eztp5wurww4d4nt9k2f389y6lyed8w6j3fg9zuvggwe2qgmvphm8zsw0r9zy8g7a4s4yfwk6vyqxpla6uxv3yvg6p35tqptkag6wrhjm8qdk2835symvpg8vwv90m5mywg4whrpg5mncf0q")]
    [InlineData("English", "panther cave tornado remain snap leisure gown vote strategy apple away room shrug action reunion",
        "acct_xsk1xpj8x86n78t2ashqqewejgnc89ns7gpvdp7zak8lehsazjwcw49e5rxudltazn57xmv6kms3pcceuamjnmef04dsm2qgrwk7ctdk7ccvzctg4pqe3lkj7sqj6gemhxrtvqwxp2ac66t2cnxffx7exxzuaya8w6la")]
    [InlineData("English", "safe mixture exhaust worth smart level donkey orphan hello ski want runway example fame gaze thumb voyage swift wire common move injury promote media",
        "acct_xsk10prx2emv8as0e8q4ny922f6q6f9x6t60xp9vd6f5qgpukkl5h99ucqsvvlhnjwmdjpaznjrkpp57sgc9x6sn8pxtq34rdfypg3xvefzespd2pg2pm6nxhgkugfs5d4rwyuh02ywwdftlzzwtm8k6429dzqdghxje")]
    [InlineData("Spanish", "vitamina banda araña rincón queja copa guiso camello limón fase objeto martes náusea matar urgente sello emitir melón acuerdo herir litio pausa dar paella",
        "acct_xsk1vp4zls6xkmkh4526cmw302dq3u8c3037esfruqal0tr454raearutg3tu8dlu3ulfw448z9ln0hjnnwfh2kgu4xgwgqhcn7wzzqhnan6dqvgr80k7vm82enupwg3mrt8tlxkwxmpd45p40n7h55pl7uxxqntznjn")]
    [InlineData("Japanese", "おうふく いってい にんか さとおや すばらしい ほせい てはい めんどう ぎいん にちじょう ほうりつ しなん げきとつ ききて ねんおし むしあつい げんそう えんそく ぎっちり さつたば ちきゅう あんい すわる せんか",
        "acct_xsk1azcqldnrvn6k32cju3ygz4t2n88tvve7c76ae7jf2g3l2p69j4d4mt04v0gt8p68p6azueypdp3a4hks5p4der9wxflplklmsxcs4aw02tamgauq9snvsyvafyv2g20am56hdlkpf42w6w4999ae6dd8pytq2k0s")]
    [InlineData("ChineseSimplified", "驻 歇 职 熙 盐 剪 带 泰 轴 枯 奥 兵 曰 桂 导 睡 抵 宁 容 读 幻 悉 默 新",
        "acct_xsk1zpzsel0fwpxr9v33zw9fw04almj3tsus7qj9ej3q2m8tp0asjedc6r9e2mj78sey0yn7hs9h0vw4zyms8yqttvlyg0uav7urshgufh3gc47zuflhreffg4grqt6s7hv6qr8ts8evqj3rgvzdpxayq6wqayj5q3rk")]
    public async Task Derive_Correct_Bech32_Extended_Account_Signing_Key_Defaulting_To_Zero_Account_Index_When_Index_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveAccountKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic
        };
        var zeroIndexCommand = new DeriveAccountKeyCommand()
        {
            Language = language,
            Mnemonic = mnemonic,
            AccountIndex = 0,
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
        "acct_xsk1gp8ejcw00et7q3h49hxca7w80d8r76m8qnu6ffw7mdaxht75sa8uk9d9lt0hm28zl8f3k0pqh4szgk5tqzj3nzl82kdnmgr4yv90sshylp6ypc3jtv5tht5z0phvg7avjxlv79dgv77e0h44mhrqn60chc4zevx2")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco",
        "acct_xsk1fpz0qtjpql9l4zy4ztxaxtrlw5f9yyzgjmhxtgx40hk3vvupr4wyujvkgkln63q5jwzpsazfvr6pykqazxt3y68payk2r0c4kzlu2t0uqpl3wm3m8zdkcp6ppxl29j9szy6u2jnn77lq3kadd7598m36jgacc9zl")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ",
        "acct_xsk1yzkyv5ydk797hv74n0y8gter6lmn5dym8fmwtnejuv4ulc65gewgxfre5pt6gwh7rszqey8t5ef3nhuykf0syul9asrcvflt6pjyspq2vekcw07wku59dg3p2nrk5c40ge0jqq4rl6sxgnvnkvv6hpzhkspjep6p")]
    [InlineData(
        "Italian", "sfacelo quadro bordo ballata pennuto scultore ridurre tesserato triturato palazzina ossidare riportare sgarbato vedova targato",
        "acct_xsk1pzw7ssh3gnheynsqmh5aquehm9ykzj58qeupatmunc3npedaadwnee80zxxrj42sj5wkvyl9l9ldhlrleda2ydj2jvnhchz689xs2y4zt9u95mt30r57gq6c3fqx9ajgag6ejaznmqyeg7eq9gwx6ajkqvpwl6r3")]
    [InlineData(
        "ChineseSimplified", "胁 粒 了 对 拆 端 泼 由 烈 苏 筹 如 技 防 厚",
        "acct_xsk12rhhtunuvaq0j9yskhcfvc7fxqgadel00qal9n2p5etpdg6tnfwu298gpqlrdp2yayey55ucutz9j7j4gj725veyew9wg8xpn25qd2h6np8hf7pweapg5cqj0jgjtsh39lpqqnzf3rkvx7awxrqh3hdl65uljq9n")]
    public async Task Derive_Correct_Bech32_Extended_Account_Signing_Key_Defaulting_To_Empty_Passphrase_When_Passphrase_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveAccountKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };
        var emptyPassSpecificCommand = new DeriveAccountKeyCommand()
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
        "acct_xsk12zf85pyr96s6fwctaeys44kjqgg5r9defkse37ewjq02rqf2qazccn0qkphmkacyvlpk9zct7nthjsgxjshnjcr37j7vfckcjn8twhvk6qsy4qn6htjlmelcpwrks9y475vgjz3az3ql4etup6rql2y6eulg6nkx")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "acct_xsk1ezrpfuv42zn3fp344e6hc43dq6ttdef44dta6ukjsvhevnsl2pz66ktwgtwfgww468z0ayrj2trwy0drljmws6uw3ledrpe0es8hs5xpq6u7xhd5552uzr54uvft2encf8smmcmgdwsuqjg5gmjl63hk2stk2j37")]
    [InlineData("pass", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "acct_xsk1mplqu264p8dnnqzs00s2e6sfrkv3x7jqrmdvrf8y0etkmxvgndytr3kjuj8cukkp85dt7u70sxkg006zfl6wkhlxaemx2crnc3f9u7y2lm3f3myv7u989xsnaf9cyfmzq5f592zmqpj9gwrr8lvk9863gqruslr5")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "baba ancho crear casero quitar canica cabeza aliento aprobar broca adicto letal anciano peldaño voto",
        "acct_xsk1fpw5p4vlhhyhpt76shc7eeh3zqzp9qsy9qlga3puvvrhut9udffcg7rr3hj69284vrsxa54wta6ragk7xcdrracx3y7uunm4hfeymdh2v8f5wc5c0nnl5slxzskn3pm6gtwaaz7nmckcdu76h50wnztv3s7l9m5k")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "acct_xsk10z5pxvp3t59435nu2jrakjjwgp0zxmeclxr39jh5x95fdmlgn3vga8ywwxzg5gyeaq7gz9gd4zczcsx85f69als5zgdmmqwky87l78qasuw487mw6hj4khjrkyjzxgt604zqdpqv80hzanha3uhgk9lcfu05azq5")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "acct_xsk1grhz4lc2t2masglaqa94p4rcscrr2n7hegg57tjlj7jrucewvdv3jy87d8pknqs6a37kr9yqpe43s5mmaunp9v0t07lsdr0s86n4rnqreudtffv2fkw0lefpsakt32p6lkc25e23en8y2np4wss55e7c2g45myq4")]
    public async Task Derive_Correct_Bech32_Extended_Account_Signing_Key_When_Passphrase_And_Language_Are_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string passPhrase, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveAccountKeyCommand()
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
        "acct_xsk1hqh2sk2jc6cwt20qypzdq6qldkxwyq8ydu97sxk0xwm6artsl3frtqnk4w2zlyr906g7udqy8hpz3axuc4yu4qd5uwhzmht2sg82ljhjf7vtdg3kajfural6u9exd234payu4s782fqqssmca0xle5r46cumaetd")]
    [InlineData(1, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "acct_xsk1fqnh8vhpul5tza40nhhlgyk47yalmyru2rmavzywatyg3rtsl3fzmu3rxkh4f3vuyssj26steekduhcgj0s3lw9fu8xthdjc5mk6964ky2nj7rnp5ngjwtfnkd590q2c26gxqyj57ruc2xpgnz9gm2v34u03pzwz")]
    [InlineData(280916, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "acct_xsk1vz6xkyt2sqwa5rzcz3jms4dve2fff8f4j8fppaqz3pdytznsl3fvjnzxgyxlkf2k03qgpq8qwudgaaxh5k8njnfgfme9waq74xpnng2nmyudx4s8efvk5qfcx2yu0akvezecja2jp35eew3d5lqw8ztzq5c3hcjv")]
    [InlineData(2147483647, "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "acct_xsk1gp3kuxrvu7ug6u9x0v0zuse395ejhusfzf66axk423r5lztsl3ffp2aucmhad088la4gftscfflzqs4ldsfsv3470296qc3qt33ey5rqayc6fw39nmy4dg24d26sxncfp93lfj4sgfuvyjg5lx0qcfrevgewdxll")]
    public async Task Derive_Correct_Bech32_Extended_Account_Signing_Key_When_Account_And_Address_Index_Is_Supplied_And_Mnemonic_Is_Valid_For_Language(
        int accountIndex, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveAccountKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            AccountIndex = accountIndex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }
}
