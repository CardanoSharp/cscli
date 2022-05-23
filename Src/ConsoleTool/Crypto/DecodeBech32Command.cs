using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions;

namespace Cscli.ConsoleTool.Crypto;

public class DecodeBech32Command : ICommand
{
    public string Value { get; init; } = string.Empty;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(Value))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --value is required."));
        }
        if (!Bech32.IsValid(Value))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --value {Value} is not a valid bech32 encoding."));
        }

        try
        {
            var hex = Bech32
                .Decode(Value, out var ver, out var prefix)
                .ToStringHex();
            var result = CommandResult.Success(hex);
            return ValueTask.FromResult(result);
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }
}