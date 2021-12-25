using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Commands;

public class DeriveRootKeyCommand : ICommand
{
    public string? Mnemonic { get; init; }
    public string Language { get; init; } = DefaultMnemonicLanguage;
    public string Passphrase { get; init; } = string.Empty;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Mnemonic))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic is required"));
        }
        if (!Enum.TryParse<WordLists>(Language, out var wordlist))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported"));
        }

        var wordCount = Mnemonic.Split(' ', StringSplitOptions.TrimEntries).Length;
        if (!ValidMnemonicSizes.Contains(wordCount))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --mnemonic must have the following word count ({string.Join(", ", ValidMnemonicSizes)})"));
        }

        try
        {
            var mnemonicService = new MnemonicService();
            var mnemonic = mnemonicService.Restore(Mnemonic, wordlist);
            var rootPrvKey = mnemonic.GetRootKey(Passphrase);
            var rootKeyExtendedBytes = BuildExtendedRootKeyBytes(rootPrvKey);
            var bech32ExtendedRootKey = Bech32.Encode(rootKeyExtendedBytes, RootKeyExtendedBech32Prefix);
            var result = CommandResult.Success(bech32ExtendedRootKey);
            return ValueTask.FromResult(result);
        }
        catch (ArgumentException ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureInvalidOptions(ex.Message));
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }

    private static byte[] BuildExtendedRootKeyBytes(PrivateKey rootPrvKey)
    {
        var rootKeyExtended = new byte[96];
        Array.Copy(rootPrvKey.Key, rootKeyExtended, rootPrvKey.Key.Length);
        Array.Copy(rootPrvKey.Chaincode, 0, rootKeyExtended, rootPrvKey.Key.Length, rootPrvKey.Chaincode.Length);
        return rootKeyExtended;
    }
}
