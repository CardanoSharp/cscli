namespace Cscli.ConsoleTool
{
    public enum CommandOutcome
    {
        Success = 0,
        FailureInvalidOptions,
        FailureTimedOut,
        FailureCancelled,
        FailureUnhandled,
    }

    public class CommandResult
    {
        public CommandOutcome Outcome { get; }

        public string Result { get; }

        public CommandResult(CommandOutcome outcome, string result)
        {
            Outcome = outcome;
            Result = result;
        }

        public static CommandResult Success(string result) =>
            new(CommandOutcome.Success, result);

        public static CommandResult FailureInvalidOptions(string result) =>
            new(CommandOutcome.FailureInvalidOptions, result);

        public static CommandResult FailureTimedOut(string result) =>
            new(CommandOutcome.FailureTimedOut, result);

        public static CommandResult FailureCancelled(string result) =>
            new(CommandOutcome.FailureCancelled, result);

        public static CommandResult FailureUnhandled(string result) =>
            new(CommandOutcome.FailureUnhandled, result);
    }
}
