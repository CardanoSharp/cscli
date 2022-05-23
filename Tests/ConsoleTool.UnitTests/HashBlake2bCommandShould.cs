using Cscli.ConsoleTool.Crypto;
using FluentAssertions;
using Xunit;

namespace Cscli.ConsoleTool.UnitTests
{
    public class HashBlake2bCommandShould
    {
        [Theory]
        [InlineData(160, "20", "8307bbdd3537e6ed512e031521b5d857491594ed")]
        [InlineData(160, "d92b380b5413b76202056eea98b6bf579d52a54a44688c1f7f97b8237469636B65745F6D61647269645F3033", "14b10f77f116636f00a23fdbacf8a1d93bf52fe3")]
        [InlineData(160, "68656c6c6f", "b5531c7037f06c9f2947132a6a77202c308e8939")]
        [InlineData(224, "20", "3542acb3a64d80c29302260d62c3b87a742ad14abf855ebc6733081e")]
        [InlineData(224, "68656c6c6f", "a4963e4ea2aa9b4120672abfc4c4299ba365368fa5a3910d5c559fc5")]
        [InlineData(224, "8512f8eb5d0a26038f703d58e2b45123b6e441e2208cb37fc22605f0d215a26a", "e3f81c986990cfd80283944858ae50c2d82f6a79d330ce7014e0b7fd")]
        [InlineData(224, "1872bc5ecc95b419de3f72544a6656ceb9a813755544618bb6b4dcc230ed9721", "9df9179beb0ce89f84025e02ae11c18b3003e7690149caa662fafd01")]
        [InlineData(256, "20", "ae85d245a3d00bfde01f59f3c4fe0b4bfae1cb37e9cf91929eadcea4985711de")]
        [InlineData(256, "68656c6c6f", "324dcf027dd4a30a932c441f365a25e86b173defa4b8e58948253471b81b72cf")]
        [InlineData(256, "4D79206E616D65206973204F7A796D616E646961732C206B696E67206F66206B696E67733B204C6F6F6B206F6E206D7920776F726B732C207965204D69676874792C20616E642064657370616972212220546865206C6F6E6520616E64206C6576656C2073616E647320737472657463682066617220617761792E", "4fdc7eaf2f91482e42fb1213f19a38ab420bed1b88eb6025e9a9fc05204d649c")]
        [InlineData(512, "68656c6c6f", "e4cfa39a3d37be31c59609e807970799caa68a19bfaa15135f165085e01d41a65ba1e1b146aeb6bd0092b49eac214c103ccfa3a365954bbbe52f74a2b3620c94")]
        [InlineData(512, "4d79206e616d65206973204f7a796d616e646961732c206b696e67206f66206b696e67733b204c6f6f6b206f6e206d7920776f726b732c207965204d69676874792c20616e642064657370616972212220546865206c6f6e6520616e64206c6576656c2073616e647320737472657463682066617220617761792e", "9847f440d8d1ca88c781a7f672b940c8855ca582cf4dd1e7c11c460a08994fb349f7551262614875f1e241104d50af167c0dfa208a11ce6aad14ce624e7bd4a7")]
        public async Task Successfully_Hash_Values_When_Properities_Are_Invalid(
            int length, string value, string expectedDigestHex)
        {
            var command = new HashBlake2bCommand()
            {
                Length = length,
                Value = value
            };

            var executionResult = await command.ExecuteAsync(CancellationToken.None);

            executionResult.Result.Should().Be(expectedDigestHex);
        }

        [Theory]
        [InlineData(160, null)]
        [InlineData(160, "")]
        [InlineData(160, "feature damp short detail access install rookie spell parrot")]
        [InlineData(224, null)]
        [InlineData(224, "")]
        [InlineData(224, "addr_test1vr3ls8ycdxgvlkqzsw2ysk9w2rpdstm208fnpnnsznst0lgg4vjxk")]
        [InlineData(256, null)]
        [InlineData(256, "")]
        [InlineData(256, "0xAAA")]
        [InlineData(512, null)]
        [InlineData(512, "")]
        [InlineData(512, "policy_sk1czq9363h74e8asz4x8t0ptthgyty7rc2czgynkctv7lgfsrht4vadwd499q4n4w6cg8fnkexj9n4ukrnnle7k4zw4wuftmsgelns2acqctqrf")]
        public async Task Fail_Blake2b_Hashing_When_Value_Is_Invalid(
            int length, string value)
        {
            var command = new HashBlake2bCommand()
            {
                Length = length,
                Value = value
            };

            var executionResult = await command.ExecuteAsync(CancellationToken.None);

            executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        }

        [Theory]
        [InlineData(-224)]
        [InlineData(-160)]
        [InlineData(-100)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        [InlineData(255)]
        [InlineData(500)]
        [InlineData(1024)]
        [InlineData(4096)]
        public async Task Fail_Blake2b_Hashing_When_Length_Is_Invalid(int length)
        {
            var command = new HashBlake2bCommand
            {
                Length = length,
                Value = "68656C6C6F"
            };

            var executionResult = await command.ExecuteAsync(CancellationToken.None);

            executionResult.Outcome.Should().Be(CommandOutcome.FailureInvalidOptions);
        }
    }
}
