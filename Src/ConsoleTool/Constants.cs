using System.Text.Json;

namespace Cscli.ConsoleTool;

public static class Constants
{
    public const string DefaultMnemonicLanguage = "English";
    public const int DefaultMnemonicCount = 24;
    public const int MaxDerivationPathIndex = Int32.MaxValue; // 2^31 - 1
    // Bech32 Prefixes https://cips.cardano.org/cips/cip5/
    public const string RootKeyExtendedBech32Prefix = "root_xsk";
    public const string PaymentSigningKeyBech32Prefix = "addr_xsk";
    public const string StakeSigningKeyBech32Prefix = "stake_xsk";
    // JSON CBOR envelopes from cardano-cli
    public const string PaymentSKeyJsonTypeField = "PaymentExtendedSigningKeyShelley_ed25519_bip32";
    public const string PaymentSKeyJsonDescriptionField = "Payment Signing Key";
    public const string PaymentVKeyJsonTypeField = "PaymentExtendedVerificationKeyShelley_ed25519_bip32";
    public const string PaymentVKeyJsonDescriptionField = "Payment Verification Key";
    public const string StakeSKeyJsonTypeField = "StakeExtendedSigningKeyShelley_ed25519_bip32";
    public const string StakeSKeyJsonDescriptionField = "Stake Signing Key";
    public const string StakeVKeyJsonTypeField = "StakeExtendedVerificationKeyShelley_ed25519_bip32";
    public const string StakeVKeyJsonDescriptionField = "Stake Verification Key";
    // Validation constraints
    public static readonly int[] ValidMnemonicSizes = { 9, 12, 15, 18, 21, 24 };
    // Default JSON Serialiser settings
    public static readonly JsonSerializerOptions SerialiserOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    // Commandline args switch mappings
    public static Dictionary<string, string> SwitchMappings => new()
    {
        { "--verification-key-file", "verificationKeyFile" },
        { "--signing-key-file", "signingKeyFile" },
        { "--account-index", "accountIndex" },
        { "--address-index", "addressIndex" },
        { "--stake-account-index", "stakeAccountIndex" },
        { "--stake-address-index", "stakeAddressIndex" },
        { "--payment-address-type", "paymentAddressType" },
        { "--network-tag", "networkTag" },
        //{ "--output-format", "outputFormat" },
    };
}
