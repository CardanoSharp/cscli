using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Wallet;

public class ConvertVerificationKeyCommand : ICommand
{
    private static readonly HashSet<string> SupportedSigningKeyPrefixes = new()
    {
        RootExtendedSigningKeyBech32Prefix, RootSigningKeyBech32Prefix,
        AccountExtendedSigningKeyBech32Prefix, AccountSigningKeyBech32Prefix,
        PaymentExtendedSigningKeyBech32Prefix, PaymentSigningKeyBech32Prefix,
        StakeExtendedSigningKeyBech32Prefix, StakeSigningKeyBech32Prefix,
        PolicySigningKeyBech32Prefix
    };

    public string? SigningKey { get; init; }
    public string? VerificationKeyFile { get; init; } = null;

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(SigningKey))
            return CommandResult.FailureInvalidOptions(
                "Invalid option --sigining-key is required");

        if (!Bech32.IsValid(SigningKey))
            return CommandResult.FailureInvalidOptions(
                "Invalid option --sigining-key is not in bech32 format - please see https://cips.cardano.org/cips/cip5/");

        var signingKeyBytes = Bech32.Decode(SigningKey, out _, out var sKeyPrefix);
        if (!SupportedSigningKeyPrefixes.Contains(sKeyPrefix))
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --sigining-key with prefix '{sKeyPrefix}' is not supported");

        var signingKey = new PrivateKey(signingKeyBytes[..64], signingKeyBytes[64..]);
        var verificationKey = signingKey.GetPublicKey(false);
        
        var vKeyPrefix = sKeyPrefix.Replace("sk", "vk");
        var bech32VKey = verificationKey.Chaincode.Length == 0
            ? Bech32.Encode(verificationKey.Key, vKeyPrefix)
            : Bech32.Encode(verificationKey.BuildExtendedVkeyBytes(), vKeyPrefix);
        if (!string.IsNullOrWhiteSpace(VerificationKeyFile))
        {
            var vkeyCborTextEnvelope = BuildTextEnvelope(sKeyPrefix, verificationKey);
            if (vkeyCborTextEnvelope is not null)
                await File.WriteAllTextAsync(VerificationKeyFile, JsonSerializer.Serialize(vkeyCborTextEnvelope, SerialiserOptions), ct).ConfigureAwait(false);
        }

        return CommandResult.Success(bech32VKey);
    }

    private static TextEnvelope? BuildTextEnvelope(string sKeyPrefix, PublicKey vKey) => sKeyPrefix switch
    {
        PaymentExtendedSigningKeyBech32Prefix => new TextEnvelope(
                PaymentExtendedVKeyJsonTypeField,
                PaymentVKeyJsonDescriptionField,
                KeyUtils.BuildCborHexPayload(vKey.BuildExtendedVkeyBytes())),
        PaymentSigningKeyBech32Prefix => new TextEnvelope(
                PaymentVKeyJsonTypeField,
                PaymentVKeyJsonDescriptionField,
                KeyUtils.BuildCborHexPayload(vKey.Key)),
        // cardano-cli compatibility requires us to use non-extended verification keys
        StakeExtendedSigningKeyBech32Prefix => new TextEnvelope(
                StakeVKeyJsonTypeField,
                StakeVKeyJsonDescriptionField,
                KeyUtils.BuildCborHexPayload(vKey.Key)),
        // cardano-cli compatibility requires us to use non-extended verification keys
        StakeSigningKeyBech32Prefix => new TextEnvelope(
                StakeVKeyJsonTypeField,
                StakeVKeyJsonDescriptionField,
                KeyUtils.BuildCborHexPayload(vKey.Key)),
        // cardano-cli compatibility requires us to use extended payment verification keys for policy keys
        PolicySigningKeyBech32Prefix => new TextEnvelope(
                PaymentExtendedVKeyJsonTypeField,
                PaymentVKeyJsonDescriptionField,
                KeyUtils.BuildCborHexPayload(vKey.BuildExtendedVkeyBytes())),
        _ => null
    };
}
