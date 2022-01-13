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
            "wallethd" => ParseWalletHdCommands(intent, args),
            "bech" => ParseBechCommands(intent, args),
            //"query" => ParseQueryCommands(intent, args), // TODO: query via Blockfrost/Koios integration
            //"tx" => ParseTxCommands(intent, args), // TODO: Easier Tx creation and submission via Blockfrost/Koios integration
            _ => new ShowInvalidArgumentCommand(intent)
        };

    private static ICommand ParseWalletHdCommands(string intent, string[] args) =>
        intent switch
        {
            "wallethd mnemonic gen" => BuildCommand<GenerateMnemonicCommand>(args),
            "wallethd key root derive" => BuildCommand<DeriveRootKeyCommand>(args),
            "wallethd key payment derive" => BuildCommand<DerivePaymentKeyCommand>(args),
            "wallethd key stake derive" => BuildCommand<DeriveStakeKeyCommand>(args),
            "wallethd address payment derive" => BuildCommand<DerivePaymentAddressCommand>(args),
            "wallethd address stake derive" => BuildCommand<DeriveStakeAddressCommand>(args),
            "bech decode" => BuildCommand<DeriveStakeAddressCommand>(args),
            _ => new ShowInvalidArgumentCommand(intent)
        };
    
    private static ICommand ParseBechCommands(string intent, string[] args) =>
        intent switch
        {
            "bech decode" => BuildCommand<BechDecodeCommand>(args),
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
