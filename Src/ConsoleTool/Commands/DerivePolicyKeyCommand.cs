using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

// See https://cips.cardano.org/cips/cip1855/
public class DerivePolicyKeyCommand : ICommand
{
    public string? Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public string Passphrase { get; init; } = string.Empty;
    public int PolicyIndex { get; init; } = 0;
    public string? VerificationKeyFile { get; init; } = null;
    public string? SigningKeyFile { get; init; } = null;

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, wordList, validationErrors) = Validate();
        if (!isValid)
        {
            return CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, validationErrors));
        }

        var mnemonicService = new MnemonicService();
        try
        {
            var rootKey = mnemonicService.Restore(Mnemonic, wordList)
                .GetRootKey(Passphrase);
            var policySkey = rootKey.Derive($"m/1855'/1815'/{PolicyIndex}'");
            var policyVkey = policySkey.GetPublicKey(false);
            var bech32PolicyKey = Bech32.Encode(policySkey.Key, PolicySigningKeyBech32Prefix);
            var result = CommandResult.Success(bech32PolicyKey);
            // Write output to CBOR JSON file outputs if optional file paths are supplied
            if (!string.IsNullOrWhiteSpace(SigningKeyFile))
            {
                var skeyCbor = new TextEnvelope(
                    PaymentExtendedSKeyJsonTypeField, // required for cardano-cli compatibility
                    PaymentSKeyJsonDescriptionField,
                    KeyUtils.BuildCborHexPayload(policySkey.BuildExtendedSkeyWithVerificationKeyBytes()));
                await File.WriteAllTextAsync(SigningKeyFile, JsonSerializer.Serialize(skeyCbor, SerialiserOptions), ct).ConfigureAwait(false);
            }
            if (!string.IsNullOrWhiteSpace(VerificationKeyFile))
            {
                var vkeyCbor = new TextEnvelope(
                    PaymentExtendedVKeyJsonTypeField, // required for cardano-cli compatibility
                    PaymentVKeyJsonDescriptionField,
                    KeyUtils.BuildCborHexPayload(policyVkey.BuildExtendedVkeyBytes()));
                await File.WriteAllTextAsync(VerificationKeyFile, JsonSerializer.Serialize(vkeyCbor, SerialiserOptions), ct).ConfigureAwait(false);
            }
            return result;
        }
        catch (ArgumentException ex)
        {
            return CommandResult.FailureInvalidOptions(ex.Message);
        }
        catch (Exception ex)
        {
            return CommandResult.FailureUnhandledException("Unexpected error", ex);
        }
    }

    private (
        bool isValid, 
        WordLists wordList, 
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            validationErrors.Add(
                $"Invalid option --recovery-phrase is required");
        }
        if (PolicyIndex < 0 || PolicyIndex > MaxDerivationPathIndex)
        {
            validationErrors.Add(
                $"Invalid option --policy-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (!string.IsNullOrWhiteSpace(SigningKeyFile)
            && Path.IsPathFullyQualified(SigningKeyFile)
            && !Directory.Exists(Path.GetDirectoryName(SigningKeyFile)))
        {
            validationErrors.Add(
                $"Invalid option --signing-key-file path {SigningKeyFile} does not exist");
        }
        if (!string.IsNullOrWhiteSpace(VerificationKeyFile)
            && Path.IsPathFullyQualified(VerificationKeyFile)
            && !Directory.Exists(Path.GetDirectoryName(VerificationKeyFile)))
        {
            validationErrors.Add(
                $"Invalid option --verification-key-file path {VerificationKeyFile} does not exist");
        }
        if (!Enum.TryParse<WordLists>(Language, ignoreCase: true, out var wordlist))
        {
            validationErrors.Add(
                $"Invalid option --language {Language} is not supported");
        }
        var wordCount = Mnemonic?.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length;
        if (wordCount.HasValue && wordCount > 0 && !ValidMnemonicSizes.Contains(wordCount.Value))
        {
            validationErrors.Add(
                $"Invalid option --recovery-phrase must have the following word count ({string.Join(", ", ValidMnemonicSizes)})");
        }

        return (!validationErrors.Any(), wordlist, validationErrors);
    }
}
