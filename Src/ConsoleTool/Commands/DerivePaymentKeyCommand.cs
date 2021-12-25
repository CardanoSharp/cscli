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
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic is required");
        }
        if (AccountIndex < 0 || AccountIndex > MaxDerivationPathIndex)
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --account-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (AddressIndex < 0 || AddressIndex > MaxDerivationPathIndex)
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --address-index must be between 0 and {MaxDerivationPathIndex}");
        }
        if (!string.IsNullOrWhiteSpace(VerificationKeyFile) 
            && !Directory.Exists(Path.GetDirectoryName(VerificationKeyFile)))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --verification-key-file path does not exist");
        }
        if (!string.IsNullOrWhiteSpace(SigningKeyFile) 
            && !Directory.Exists(Path.GetDirectoryName(SigningKeyFile)))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --signing-key-file path does not exist");
        }
        if (!Enum.TryParse<WordLists>(Language, out var wordlist))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported");
        }
        var wordCount = Mnemonic.Split(' ', StringSplitOptions.TrimEntries).Length;
        if (!ValidMnemonicSizes.Contains(wordCount))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic must have the following word count ({string.Join(", ", ValidMnemonicSizes)})");
        }

        try
        {
            var mnemonicService = new MnemonicService();
            var mnemonic = mnemonicService.Restore(Mnemonic, wordlist);
            var rootPrvKey = mnemonic.GetRootKey(Passphrase);
            var paymentPath = $"m/1852'/1815'/{AccountIndex}'/0/{AddressIndex}";
            var paymentSkey = rootPrvKey.Derive(paymentPath);
            var paymentVkey = paymentSkey.GetPublicKey(false);
            // Write output to CBOR JSON file outputs if file paths supplied
            if (!string.IsNullOrWhiteSpace(VerificationKeyFile))
            {
                var vkeyCbor = new
                {
                    type = PaymentVKeyJsonTypeField,
                    description = PaymentVKeyJsonDescriptionField,
                    cborHex = $"5820{Convert.ToHexString(paymentVkey.Key)}"
                };
                await File.WriteAllTextAsync(VerificationKeyFile, JsonSerializer.Serialize(vkeyCbor, SerialiserOptions), ct).ConfigureAwait(false);
            }
            if (!string.IsNullOrWhiteSpace(SigningKeyFile))
            {
                var sKey = paymentSkey.Key[..32]; // first 32 bytes is the signing key
                var skeyCbor = new
                {
                    type = PaymentSKeyJsonTypeField,
                    description = PaymentSKeyJsonDescriptionField,
                    cborHex = $"5820{Convert.ToHexString(sKey)}"
                };
                await File.WriteAllTextAsync(SigningKeyFile, JsonSerializer.Serialize(skeyCbor, SerialiserOptions), ct).ConfigureAwait(false);
            }
            var bech32PaymentKey = Bech32.Encode(paymentSkey.Key, PaymentSigningKeyBech32Prefix);
            var result = CommandResult.Success(bech32PaymentKey);
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
}
