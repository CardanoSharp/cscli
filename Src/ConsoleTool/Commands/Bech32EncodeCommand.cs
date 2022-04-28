using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions;

namespace Cscli.ConsoleTool.Commands;

public class Bech32EncodeCommand : ICommand
{
    public string Prefix { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(Value))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --value is required."));
        } 
        
        try
        {
            var rawBytesValue = Convert.FromHexString(Value);
            var hex = Bech32.Encode(rawBytesValue, Prefix);
            var result = CommandResult.Success(hex);
            return ValueTask.FromResult(result);
        }
        catch (FormatException ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureInvalidOptions($"Invalid --value {ex.Message}"));
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult(
                CommandResult.FailureUnhandledException("Unexpected error", ex));
        }
    }
}