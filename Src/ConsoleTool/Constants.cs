using System.Text.Json;

namespace Cscli.ConsoleTool;

public static class Constants
{
    public const string DefaultMnemonicLanguage = "English";
    public const int DefaultMnemonicCount = 24;

    public const int MaxPathIndex = 2147483647; // 2^31 - 1

    public const string SKeyJsonTypeField = "PaymentSigningKeyShelley_ed25519";
    public const string VKeyJsonTypeField = "PaymentVerificationKeyShelley_ed25519";
    public const string SKeyJsonDescriptionField = "Payment Signing Key";
    public const string VKeyJsonDescriptionField = "";

    // Bech32 Prefixes https://cips.cardano.org/cips/cip5/
    public const string RootKeyExtendedBech32Prefix = "root_xsk";
    public const string PaymentSigningKeyBech32Prefix = "addr_sk";
    public const string PaymentVerificationKeyBech32Prefix = "addr_vk";
    public const string StakeSigningKeyBech32Prefix = "stake_sk";
    public const string StakeVerificationKeyBech32Prefix = "stake_vk";

    public static readonly JsonSerializerOptions SerialiserOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static Dictionary<string, string> SwitchMappings => new()
    {
        { "--verification-key-file", "verificationKeyFile" },
        { "--signing-key-file", "signingKeyFile" },
        { "--account-index", "accountIndex" },
        { "--address-index", "addressIndex" },
        //{ "--output-format", "outputFormat" },
    };
}
