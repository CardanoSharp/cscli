using Cscli.ConsoleTool.Commands;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Cscli.ConsoleTool;

public static class CommandParser
{
    public static ICommand ParseArgsToCommand(string[] args)
    {
        if (args.Length == 0 || IsHelpOption(args[0]))
        {
            return new ShowBaseHelpCommand();
        }
        if (IsVersionOption(args[0]))
        {
            return new ShowVersionCommand();
        }
        var intent = DeriveIntent(args);
        var command = ParseCommands(intent, args);
        return command;
    }

    private static string DeriveIntent(string[] args)
    {
        // Intent comprises of the first few cli args that are not command arguments
        static bool IsCommandArgument(string arg) =>
            arg.StartsWith("-") || arg.StartsWith("/");

        var intent = new StringBuilder();
        foreach (var arg in args)
        {
            if (IsCommandArgument(arg))
                break;
            intent.Append($"{arg} ");
        }
        return intent.ToString().TrimEnd();
    }

    private static ICommand ParseCommands(string intent, string[] args) =>
        args[0] switch
        {
            "wallet" => ParseWalletCommands(intent, args),
            //"query" => ParseQueryCommands(intent, args), // TODO: query via Blockfrost/Koios integration
            //"tx" => ParseTxCommands(intent, args), // TODO: Easier Tx creation and submission via Blockfrost/Koios integration
            _ => new ShowInvalidArgumentCommand(intent)
        };

    private static ICommand ParseWalletCommands(string intent, string[] args) =>
        intent switch
        {
            "wallet recovery-phrase generate" => BuildCommand<GenerateMnemonicCommand>(args),
            "wallet key root derive" => BuildCommand<DeriveRootKeyCommand>(args),
            "wallet key payment derive" => BuildCommand<DerivePaymentKeyCommand>(args),
            "wallet key stake derive" => BuildCommand<DeriveStakeKeyCommand>(args),
            "wallet key policy derive" => BuildCommand<DerivePolicyKeyCommand>(args),
            "wallet address payment derive" => BuildCommand<DerivePaymentAddressCommand>(args),
            "wallet address stake derive" => BuildCommand<DeriveStakeAddressCommand>(args),
            _ => new ShowInvalidArgumentCommand(intent)
        };

    private static ICommand BuildCommand<T>(
        string[] args)
        where T : ICommand
    {
        var command = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args, Constants.SwitchMappings)
            .Build()
            .Get<T>();

        return command;
    }

    private static bool IsHelpOption(string arg)
    {
        return arg == "help" || arg == "-h" || arg == "--help";
    }

    private static bool IsVersionOption(string arg)
    {
        return arg == "version" || arg == "-v" || arg == "--version";
    }
}
