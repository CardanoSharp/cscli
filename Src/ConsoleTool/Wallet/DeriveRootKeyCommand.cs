﻿using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using static Cscli.ConsoleTool.Constants;

namespace Cscli.ConsoleTool.Wallet;

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
                $"Invalid option --recovery-phrase is required"));
        }
        if (!Enum.TryParse<WordLists>(Language, ignoreCase: true, out var wordlist))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --language {Language} is not supported"));
        }
        var wordCount = Mnemonic.Split(' ', StringSplitOptions.TrimEntries).Length;
        if (!ValidMnemonicSizes.Contains(wordCount))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --recovery-phrase must have the following word count ({string.Join(", ", ValidMnemonicSizes)})"));
        }

        var mnemonicService = new MnemonicService();
        try
        {
            var rootPrvKey = mnemonicService.Restore(Mnemonic, wordlist)
                .GetRootKey(Passphrase);
            var rootKeyExtendedBytes = rootPrvKey.BuildExtendedSkeyBytes();
            var bech32ExtendedRootKey = Bech32.Encode(rootKeyExtendedBytes, RootExtendedSigningKeyBech32Prefix);
            return ValueTask.FromResult(CommandResult.Success(bech32ExtendedRootKey));
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
}
