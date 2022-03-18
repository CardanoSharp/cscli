using Cscli.ConsoleTool.Commands;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class DeriveRootKeyCommandShould
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
        var command = new DeriveRootKeyCommand()
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
        var command = new DeriveRootKeyCommand()
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
        var command = new DeriveRootKeyCommand()
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
        var command = new DeriveRootKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Contain("invalid words");
    }

    [Theory]
    [InlineData("fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "root_xsk1lr3cx6gq4za83wzg86c6zkgtgneduls79q3jfvsxxt95qlnsl3fx50ze4squpnf765a7hpalypwmhayqdk75qzmcky3af23zttvrsedhjr79k4kzyawfd69780ndrf94m02t5rh90nl922a6h2652yxapuaec7du")]
    [InlineData("trap report remind insane change toy rotate suggest misery vault language mind bone hen mountain",
        "root_xsk1zz42r64vuyfg5fm29syvfrurcnzq3sqatsnrdqus0w92ss7j5dzkyu52ufx78revvjfvqh4s6yamvf2rd3fwkwpj3qdp9y3ywr0vceq7r08zh9292q2jqf6rs9lf2xpucnnamgr8tts5qw602tjuzzd6jspgpgt2")]
    [InlineData("since cook close prosper slush luggage observe neglect fit arm twelve grief evolve illegal seven destroy joke hand useless knee silent wasp protect purity",
        "root_xsk1mp5v07fpudmmwvvsqj38nn8l3hkeuhes53r5f2xpevdqe6pgy3v7a8tjtkmd677ge7n8w55gfyk974ee06a794nah2lwpuusln0hf82la4ec2e34jtqhk7pzprvwvtmn9uk5s2xvca8332guj6g4ku6djut4hs9r")]
    public async Task Derive_Correct_Bech32_Extended_Root_Signing_Key_Defaulting_To_English_When_Passphrase_And_Language_Are_Not_Supplied_And_Mnemonic_Is_Valid_English(
        string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveRootKeyCommand()
        {
            Mnemonic = mnemonic
        };
        var englishSpecificCommand = new DeriveRootKeyCommand()
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
    [InlineData(
        "English", "rapid limit bicycle embrace speak column spoil casino become evolve unknown worry letter team laptop unknown false elbow bench analyst dilemma engage pulse plug",
        "root_xsk13qf2w4jsex0a27xjpwvv68jlk6vhlfql6la88wv495x9mfx5sa8h2xx5wxzx6ghgg504nqkaw55cev37xer2mpwcast76zk98zt7c35eagfuj4wnzv3pgmzxlfhucqg03s4c8ha6jg8j6y2h6ghdplpelqjtss3r")]
    [InlineData(
        "English","wagon extra crucial bomb lake thumb diamond damp carry window mammal name load barely mixed decorate boss cancel sadness anxiety swim friend bracket frame",
        "root_xsk1crr05lc9w35hdmz4qrgzv2895jvksqsk5lk58zt8l4kvntczrdr5we87q8mg0pfsefey2wsejpjp8hh94c8y3l976uha5qlmcsezpc7r975hd5vg30jfuu0g3qhux6ynjw7t5mamg57tkelpwukzcyrxty7s0qs7")]
    [InlineData(
        "English", "tobacco toe brand law piece awkward awkward angle hint hen flame morning drop oxygen mother mom click because bulk seminar strong above toilet bird",
        "root_xsk1rrqsavyh9ktz599kgd8jwmvmnhh2wavp6swqs7nh5vw6zktv0exg6h8quvuemwwcd3tr9pr3jmyh6ap0rqsn8dmafrl5u4p35vw50pe2r43t0yujkcarqygam5j0c3yjag0m0k2hxpuwd2eage9nhaduwct9dgnx")]
    [InlineData(
        "Spanish", "pensar leve rosca hueso ombligo cerrar guion molde bonito misa recaer amargo sodio noria tosco",
        "root_xsk17r385qlkz0g4fgn9fcyxw5cxtu9qju6wweravmgn7pd4z2ypr4wdh4erc8ww0n3d389hwrelggchvlu57ulxefjzepmdzx9jwx8z3lymeeemhjel5tdkqffhe75jwxrs3l4xv97lnqve7zn0svplp33aku4ywq68")]
    [InlineData(
        "Japanese", "たたかう しのぐ きひん てまえ むさぼる なれる しあさって りゆう ろめん げいのうじん したて たんか さんみ けんとう うけもつ",
        "root_xsk1yz4dsxpn79vdyxwe6pr7k3y5pzsljd22p8s2seushdcxdkj5gew0dvlyl83qe0cu85pkaplc5l3q4zm2p7tw59shdtw5rrhgd2utjgz3namtvt4n2gu2uexl9rnksqtqa2r76dxjngjxvj34ez53zy0zg5cr4qf6")]
    [InlineData(
        "Italian", "danzare diametro valanga piffero finire sbadiglio veterano fato frigo saraceno mirino fuggente sapere dondolo mitra",
        "root_xsk1ezkc853wqv3cynuqfpkp6hls45q85sueep5wmyhuuu4n7d0ksfzp4p8lkfz0zvvszt56hmynkvuphh822gsk08k4lx0anju88yxheuv9fduj2ffh0pmclxwwh6razk2cqn9cjqtk5w8r34fq09rf6tr3agqtvk4d")]
    [InlineData(
        "ChineseSimplified", "能 林 手 近 层 锭 先 伏 杯 彩 季 住 该 厉 亭",
        "root_xsk1vzx4pqxh4368mgwq289v569g6sflppxtcky00h4t3l7akr08we8lh50he0szdkz72ul8hew9cxy2peg4w5emmk5ux54lnjzjfrgnnsxczprrdlg53etdl7l3mextxqr3f90v5pka94m6y9ajy37crcy6859d79tp")]
    public async Task Derive_Correct_Bech32_Extended_Root_Signing_Key_Defaulting_To_Empty_Passphrase_When_Passphrase_Is_Not_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveRootKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
        };
        var emptyPassSpecificCommand = new DeriveRootKeyCommand()
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
        "root_xsk1xzdq5chjgyzztg4926alf6fhe57aa48u5xkwt03qrcy7y7p2qazmsnmjyk0ejtzuy6feg4unz49r30e3ea7myz6ml6pz0uw55m7p6862wzcq3hz7cpxm5q4r5h7s9ct4wfeg3682fwz9p5cd6yu8v46l6q25dfu5")]
    [InlineData("Gxxk3A32Qch8ZcwtUib3g9gSs&#abSx01&Hfpf!VAcz72bz2G5", "English", "fitness juice ankle box prepare gallery purse narrow miracle next soccer category",
        "root_xsk1fqmvj05d5uw934zx8xqydvvdpyxgg67jnrecfhp9rqjrs3gl2pzn9akgzpwwq0472jwquwsq3gamnk27zsythggzsdzqaehqm38ragj0lsjcgw3ad98g5s9uwl0zvwdx8eqj8sj3qv4a564xgrhqzmmtyypkv5r7")]
    [InlineData("pass", "Spanish", "producto lluvia atún elefante bello lavar tatuaje chivo muñeca vaina cisne regir almeja llama muleta",
        "root_xsk1wrmyvj6sqfm0gpxntn3w3vfday55zu0szl88hjzl9rya9d0wyet6nfgnw80zcddll24ftef8mcdcg42kwyaacqnysfc2lflmffax8nhh8mfhgr88hv242k0rmxhvfscmw9aalw4pmmaa3grhlftt6ssd4yq05vsr")]
    [InlineData("#SCVh0%vyMWmBj!BR4EYIrUBB4UM%c5yvS8&S!m0W2PNFJX4di", "Spanish", "producto lluvia atún elefante bello lavar tatuaje chivo muñeca vaina cisne regir almeja llama muleta",
        "root_xsk1xpmalqmnpsux7hewqj9raz4xvavh9773pv9589ls58eujglhuag7hc0h6vwhyjzduh6vwrj687j2te4nsvt85jycvtgtz2l7luwvpx2x7e74tvjdt4866reqcv7h89stakzqa52xyxc696hvvl880ukj7c4jj9lz")]
    [InlineData("pass", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "root_xsk1nrxaqdjjz6vgm9c890w3u7u4udfeyhmf3a3879r46mnh3c0gn3vxd82r56q7myyvwh9m3h53st2wjxaqn6kvyneln5fwle82r0wlmecufzw0m3mdfn3uam5tpawtk0qrfs5tpxq2wpul264w5ay2zwsccuws6kc0")]
    [InlineData("AhkYZ!!*ng@x56#cN5wE83ugz9JD8Xju#fm3De9AWKYCyDSDZE", "Japanese", "あまい るいさい はづき になう ことがら ふしぎ がっさん えんそく きうい みかん うんどう いだく せっけん にってい とおい",
        "root_xsk1krrd6navw95dcwtlzhq0tmq9gd030kx4xunhj0zyawln7kpwvdvsqev5u9plyndq2dtkc5pj6e0w8g63t8y6fy32cfye7wpdyuja623mht5h5ryn8mt7r5h5clu4nsyaa8dfh674k0cuh468vk2dnjltegy3dcd3")]
    public async Task Derive_Correct_Bech32_Extended_Root_Signing_Key_When_Passphrase_And_Language_Are_Supplied_And_Mnemonic_Is_Valid_For_Language(
        string passPhrase, string language, string mnemonic, string expectedBech32Key)
    {
        var command = new DeriveRootKeyCommand()
        {
            Mnemonic = mnemonic,
            Language = language,
            Passphrase = passPhrase,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedBech32Key);
    }
}