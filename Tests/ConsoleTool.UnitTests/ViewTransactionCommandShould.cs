using Cscli.ConsoleTool.Transaction;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests;

public class ViewTransactionCommandShould
{
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("test")]
    [InlineData("tastnet")]
    [InlineData("Mainet")]
    [InlineData("mainet")]
    [InlineData("mainnetwork")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_Network_Is_Not_Valid(
        string network)
    {
        var command = new ViewTransactionCommand()
        {
            Network = network,
            CborHex = "83a500818258205f61dde2c9c05c104f81194f2cfc738b4463f098e46fa1ca62aedb1345561624000182825839005a99cb175eb944462d6bfd29d06e0a69defc091d8e5ecab740afac6f1922fcdeeb6df8592d78b20c6e22fdb73fa9446aad05626d78000b7f181982583900ae34b08e5a409e1e66683d0b04ff33bb7c5c1455046ad66ebe2b66d157664134527cd7cb875caa34251f7e063b337cd705649637c35df6c71bfffffffffffd1e5e021a0002e1ed031a037c8bb70758205fbac14ec74ffff7a1aa1b1acbf4bfed1ce774bf21a620092f647a04553b65d5a1008182582008c77407d35dab5e3f53ef4ad5066284ca269c8107e88de04fcdfb875de9a7055840d7250f2b39c8d3014aaa8fe6ba60f0b52a27240a6b8ae7f72d64079a4852acb4ecffb3559a5fb829d9868f0cd9806e567bc710f73513ba6eb9a699f71bb1800182a1183da1646e616d65783754657374202d204669786564206e6577206172726179207673206e6577206d617020666f7220656d707479207769746e6573732073657480",
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().StartWith("Invalid option --network must be either testnet or mainnet");
    }

    [Theory]
    [InlineData(null, "testnet")]
    [InlineData(null, "mainnet")]
    [InlineData("", "testnet")]
    [InlineData("", "mainnet")]
    [InlineData(" ", "testnet")]
    [InlineData("  ", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_CborHex_Is_Null_Or_Whitespace(
        string invalidCborHex, string network)
    {
        var command = new ViewTransactionCommand()
        {
            Network = network,
            CborHex = invalidCborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --cbor-hex is required");
    }

    [Theory]
    [InlineData("0", "testnet")]
    [InlineData("0", "mainnet")]
    [InlineData("fff", "testnet")]
    [InlineData("fff", "mainnet")]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "testnet")]
    [InlineData("addr1qyg4gu4vfd3775glq8rjm85x2crmysc920hf5qjj8m7rxef8jemaq77p80ka87tm4vyem0sqnuerpmtw2awtu76dl4jssqfzz4", "mainnet")]
    [InlineData("stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw", "testnet")]
    [InlineData("stake1uyneva7s00qnhmwnl9a6kzvahcqf7v3sa4h9wh970dxl6egwp9mlw", "mainnet")]
    public async Task Execute_Unsuccessfully_With_FailureInvalidOptions_When_CborHex_Is_Not_Valid_Hexadecimal(
        string invalidCborHex, string network)
    {
        var command = new ViewTransactionCommand()
        {
            Network = network,
            CborHex = invalidCborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        executionResult.Result.Should().Be($"Invalid option --cbor-hex {invalidCborHex} is not in hexadecimal format");
    }

    [Theory]
    [InlineData("84a50081825820dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f9201018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a5cf39dd9021a00029ee5031a03afdc580758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da0f582a11902a2a1636d73676d74687820666f72206c756e636880", "testnet",
@"{
  ""id"": ""n/a"",
  ""isValid"": true,
  ""transactionBody"": {
    ""inputs"": [
      {
        ""transactionId"": ""dcfe996519321071430c812525393f431d75208428852491e9c0c6788dad5f92"",
        ""transactionIndex"": 1
      }
    ],
    ""outputs"": [
      {
        ""address"": ""addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3"",
        ""value"": {
          ""lovelaces"": 420000000,
          ""nativeAssets"": []
        }
      },
      {
        ""address"": ""addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t"",
        ""value"": {
          ""lovelaces"": 1559469529,
          ""nativeAssets"": []
        }
      }
    ],
    ""mint"": [],
    ""fee"": 171749,
    ""ttl"": 61856856,
    ""auxiliaryDataHash"": ""0088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3d"",
    ""transactionStartInterval"": null
  },
  ""transactionWitnessSet"": null,
  ""auxiliaryData"": {
    ""metadata"": {
      ""674"": {
        ""msg"": ""thx for lunch""
      }
    }
  }
}")]
    [InlineData("84a500818258205853cc86af075cc547a5a20658af54841f37a5832532a704c583ed4f010184a501018282581d6079c37fabad08662e16ad4950edd63d2a2ab0b63f8a44e4a504511ef51a1908b10082581d60282e5ee5d1e89e04fa81382df239d6733409875d75b480c879f586001a43e80724021a0002c24d031a03afe00c0758200088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3da10081825820de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa584025f49ad0cb27c0a297ebd2237134be2803b19d11c6e52416d3d7beba130bbc2bd95eb9b7e9e7410d7efcd5c2abd338dd62100d86b26308636335b533873bb508f582a11902a2a1636d73676d74687820666f72206c756e636880", "testnet",
@"{
  ""id"": ""03818a7d5875bf67523dea56a37e5654e549f095b55888dc53bbe9ef42824125"",
  ""isValid"": true,
  ""transactionBody"": {
    ""inputs"": [
      {
        ""transactionId"": ""5853cc86af075cc547a5a20658af54841f37a5832532a704c583ed4f010184a5"",
        ""transactionIndex"": 1
      }
    ],
    ""outputs"": [
      {
        ""address"": ""addr_test1vpuuxlat45yxvtsk44y4pmwk854z4v9k879yfe99q3g3aagqqzar3"",
        ""value"": {
          ""lovelaces"": 420000000,
          ""nativeAssets"": []
        }
      },
      {
        ""address"": ""addr_test1vq5zuhh9685fup86syuzmu3e6eengzv8t46mfqxg086cvqqc5zr4t"",
        ""value"": {
          ""lovelaces"": 1139279652,
          ""nativeAssets"": []
        }
      }
    ],
    ""mint"": [],
    ""fee"": 180813,
    ""ttl"": 61857804,
    ""auxiliaryDataHash"": ""0088270ea98923127ef4c2e05b665b81b5a6c9fca1582eed1171ba17648f7b3d"",
    ""transactionStartInterval"": null
  },
  ""transactionWitnessSet"": {
    ""vKeyWitnesses"": [
      {
        ""verificationkey"": ""de9503426759fa18624657f5bcc932f38220ec9eceb262907caf2d198b6e0faa"",
        ""signature"": ""25f49ad0cb27c0a297ebd2237134be2803b19d11c6e52416d3d7beba130bbc2bd95eb9b7e9e7410d7efcd5c2abd338dd62100d86b26308636335b533873bb508""
      }
    ],
    ""nativeScripts"": []
  },
  ""auxiliaryData"": {
    ""metadata"": {
      ""674"": {
        ""msg"": ""thx for lunch""
      }
    }
  }
}")]
    public async Task Execute_Successfully_With_Correct_Json_When_Options_Are_Valid(string cborHex, string network, string expectedJson)
    {
        var command = new ViewTransactionCommand()
        {
            Network = network,
            CborHex = cborHex,
        };

        var executionResult = await command.ExecuteAsync(CancellationToken.None);

        executionResult.Outcome.Should().Be(CommandOutcome.Success);
        executionResult.Result.Should().Be(expectedJson);
    }
}
