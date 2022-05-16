using Cscli.ConsoleTool.Crypto;
using Cscli.ConsoleTool.Query;
using Cscli.ConsoleTool.Transaction;
using Cscli.ConsoleTool.Wallet;
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
            "query" => ParseQueryCommands(intent, args), 
            "transaction" => ParseTransactionCommands(intent, args), 
            "bech32" or "blake2b" => ParseCryptoCommands(intent, args),
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

    private static ICommand ParseQueryCommands(string intent, string[] args) =>
       intent switch
       {
           "query tip" => BuildCommand<QueryTipCommand>(args),
           "query protocol-parameters" => BuildCommand<QueryProtocolParametersCommand>(args),
           "query info account" => BuildCommand<QueryAccountInfoCommand>(args),
           "query asset account" => BuildCommand<QueryAccountAssetCommand>(args),
           "query info address" => BuildCommand<QueryAddressInfoCommand>(args),
           _ => new ShowInvalidArgumentCommand(intent)
       };

    private static ICommand ParseTransactionCommands(string intent, string[] args) =>
       intent switch
       {
           "transaction submit" => BuildCommand<SubmitTransactionCommand>(args),
           _ => new ShowInvalidArgumentCommand(intent)
       };

    private static ICommand ParseCryptoCommands(string intent, string[] args) =>
       intent switch
       {
           "bech32 encode" => BuildCommand<EncodeBech32Command>(args),
           "bech32 decode" => BuildCommand<DecodeBech32Command>(args),
           "blake2b hash" => BuildCommand<HashBlake2bCommand>(args),
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
