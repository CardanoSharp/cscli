using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions;

namespace Cscli.ConsoleTool.Commands;

public class BechDecodeCommand : ICommand
{
    public string Address { get; init; } = string.Empty;

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(Address))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --address is required."));
        }
        if (!Bech32.IsValid(Address))
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                $"Invalid option --address {Address} is not a valid bech32 encoding."));
        }        
        
        try
        {
            var hex = Bech32
                .Decode(Address, out var ver, out var prefix)
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