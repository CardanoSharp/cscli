using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using System.Text;
using System.Text.Json;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class DeriveStakeKeyCommand : ICommand
{
    public string Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public int AccountIndex { get; init; } = 0;
    public int AddressIndex { get; init; } = 0;
    public string VerificationKeyFile { get; init; }
    public string? SigningKeyFile { get; init; } = null;

    public async ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic is required");
        }
        if (AccountIndex < 0 || AccountIndex > MaxPathIndex)
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --account-index must be between 0 and {MaxPathIndex}");
        }
        if (AddressIndex < 0 || AddressIndex > MaxPathIndex)
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --address-index must be between 0 and {MaxPathIndex}");
        }
        if (string.IsNullOrWhiteSpace(VerificationKeyFile))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --verification-key-file is required");
        }
        if (!Enum.TryParse<WordLists>(Language, out var wordlist))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported");
        }

        var wordCount = Mnemonic.Split(' ', StringSplitOptions.TrimEntries).Length;
        if (!GenerateMnemonicCommand.ValidSizes.Contains(wordCount))
        {
            return CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic must have the following word count ({string.Join(", ", GenerateMnemonicCommand.ValidSizes)})");
        }
        try
        {
            var mnemonicService = new MnemonicService();
            var mnemonic = mnemonicService.Restore(Mnemonic, wordlist);
            var rootPrvKey = mnemonic.GetRootKey();
            var paymentPath = $"m/1852'/1815'/{AccountIndex}'/2/{AddressIndex}";
            var paymentSkey = rootPrvKey.Derive(paymentPath);
            var paymentVkey = paymentSkey.GetPublicKey(false);
            // Write output to verification key output path
            var vkeyCbor = new
            {
                type = VKeyJsonTypeField,
                description = VKeyJsonDescriptionField,
                cborHex = $"5820{Convert.ToHexString(paymentVkey.Key)}"
            };
            await File.WriteAllTextAsync(VerificationKeyFile, JsonSerializer.Serialize(vkeyCbor, SerialiserOptions), ct).ConfigureAwait(false);
            var output = new StringBuilder($"vkey written to {VerificationKeyFile}");
            // Optional write signing key output path
            if (!string.IsNullOrWhiteSpace(SigningKeyFile))
            {
                var sKey = paymentSkey.Key[..32];
                var skeyCbor = new
                {
                    type = SKeyJsonTypeField,
                    description = SKeyJsonDescriptionField,
                    cborHex = $"5820{Convert.ToHexString(sKey)}"
                };
                await File.WriteAllTextAsync(SigningKeyFile, JsonSerializer.Serialize(skeyCbor, SerialiserOptions), ct).ConfigureAwait(false);
                output.AppendFormat("{0}skey written to {1}", Environment.NewLine, SigningKeyFile);
            }
            var result = CommandResult.Success(output.ToString());
            return result;
        }
        catch (Exception ex)
        {
            return CommandResult.FailureUnhandledException("Unexpected error", ex);
        }
    }
}
