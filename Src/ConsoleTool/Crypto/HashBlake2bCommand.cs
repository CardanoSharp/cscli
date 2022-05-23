using Blake2Fast;
using CardanoSharp.Wallet.Extensions;
using System.Collections.Immutable;

namespace Cscli.ConsoleTool.Crypto;

public class HashBlake2bCommand : ICommand
{
    private static readonly ImmutableArray<int> HashAlgorithmLengths = ImmutableArray.Create(new[]
        {
            160, // Assets
            224, // Keys/Addresses/SimpleScript/PlutusScript
            256, // Transactions
            512  // Wallet checksums
        });

    public int Length { get; init; } = 224;
    public string Value { get; init; } = string.Empty;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(Value))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --value is required"));
        }
        if (!HashAlgorithmLengths.Contains(Length))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --length {Length} is not supported"));
        }

        try
        {
            var digestLengthInBytes = Length / 8;
            var rawBytesValue = Convert.FromHexString(Value);
            var digest = Blake2b.ComputeHash(digestLengthInBytes, rawBytesValue);
            var digestHex = digest.ToStringHex();
            var result = CommandResult.Success(digestHex);
            return ValueTask.FromResult(result);
        }
        catch (FormatException ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureInvalidOptions($"Invalid option --value {ex.Message}"));
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }
}
