using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class DerivePaymentKeyCommand : ICommand
{
    public string? Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public string Passphrase { get; init; } = string.Empty;
    public int AccountIndex { get; init; } = 0;
    public int AddressIndex { get; init; } = 0;
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
            var paymentSkey = rootKey.Derive($"m/1852'/1815'/{AccountIndex}'/0/{AddressIndex}");
            var paymentVkey = paymentSkey.GetPublicKey(false);
            var paymentSkeyExtendedBytes = paymentSkey.BuildExtendedSkeyBytes();
            var bech32PaymentSkeyExtended = Bech32.Encode(paymentSkeyExtendedBytes, PaymentSigningKeyBech32Prefix);
            var result = CommandResult.Success(bech32PaymentSkeyExtended);
            // Write output to CBOR JSON file outputs if optional file paths are supplied
            if (!string.IsNullOrWhiteSpace(SigningKeyFile))
            {
                var paymentSkeyExtendedWithVkeyBytes = paymentSkey.BuildExtendedSkeyWithVerificationKeyBytes();
                var skeyCbor = new
                {
                    type = PaymentSKeyJsonTypeField,
                    description = PaymentSKeyJsonDescriptionField,
                    cborHex = KeyUtils.BuildCborHexPayload(paymentSkeyExtendedWithVkeyBytes)
                };
                await File.WriteAllTextAsync(SigningKeyFile, JsonSerializer.Serialize(skeyCbor, SerialiserOptions), ct).ConfigureAwait(false);
            }
            if (!string.IsNullOrWhiteSpace(VerificationKeyFile))
            {
                var paymentVkeyExtendedBytes = paymentVkey.BuildExtendedVkeyBytes();
                var vkeyCbor = new
                {
                    type = PaymentVKeyJsonTypeField,
                    description = PaymentVKeyJsonDescriptionField, 
                    cborHex = KeyUtils.BuildCborHexPayload(paymentVkeyExtendedBytes)
                };
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
        if (AccountIndex < 0 || AccountIndex > MaxDerivationPathIndex)
        {
            validationErrors.Add(
                $"Invalid option --account-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (AddressIndex < 0 || AddressIndex > MaxDerivationPathIndex)
        {
            validationErrors.Add(
                $"Invalid option --address-index must be between 0 and {MaxDerivationPathIndex}");
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
        if (!Enum.TryParse<WordLists>(Language, out var wordlist))
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
