using System.Text.Encodings.Web;
using System.Text.Json;

namespace Cscli.ConsoleTool;

public static class Constants
{
    public const int MessageStandardKey = 674;
    public const int NftStandardKey = 721;
    public const int NftRoyaltyStandardKey = 777;
    public const int MaxMetadataStringLength = 64;
    public const string DefaultMnemonicLanguage = "English";
    public const int DefaultMnemonicCount = 24;
    public const int MaxDerivationPathIndex = Int32.MaxValue; // 2^31 - 1
    // Bech32 Prefixes https://cips.cardano.org/cips/cip5/
    public const string RootExtendedSigningKeyBech32Prefix = "root_xsk";
    public const string RootSigningKeyBech32Prefix = "root_sk";
    public const string AccountExtendedSigningKeyBech32Prefix = "acct_xsk";
    public const string AccountSigningKeyBech32Prefix = "acct_sk";
    public const string PaymentExtendedSigningKeyBech32Prefix = "addr_xsk";
    public const string PaymentSigningKeyBech32Prefix = "addr_sk";
    public const string StakeExtendedSigningKeyBech32Prefix = "stake_xsk";
    public const string StakeSigningKeyBech32Prefix = "stake_sk";
    public const string PolicySigningKeyBech32Prefix = "policy_sk";
    // JSON CBOR text envelopes from cardano-cli
    public const string PaymentSKeyJsonTypeField = "PaymentSigningKeyShelley_ed25519";
    public const string PaymentExtendedSKeyJsonTypeField = "PaymentExtendedSigningKeyShelley_ed25519_bip32";
    public const string PaymentSKeyJsonDescriptionField = "Payment Signing Key";
    public const string PaymentVKeyJsonTypeField = "PaymentVerificationKeyShelley_ed25519";
    public const string PaymentExtendedVKeyJsonTypeField = "PaymentExtendedVerificationKeyShelley_ed25519_bip32";
    public const string PaymentVKeyJsonDescriptionField = "Payment Verification Key";
    public const string StakeSKeyJsonTypeField = "StakeSigningKeyShelley_ed25519";
    public const string StakeExtendedSKeyJsonTypeField = "StakeExtendedSigningKeyShelley_ed25519_bip32";
    public const string StakeSKeyJsonDescriptionField = "Stake Signing Key";
    public const string StakeVKeyJsonTypeField = "StakeVerificationKeyShelley_ed25519";
    public const string StakeExtendedVKeyJsonTypeField = "StakeExtendedVerificationKeyShelley_ed25519_bip32";
    public const string StakeVKeyJsonDescriptionField = "Stake Verification Key";
    // Validation constraints
    public static readonly int[] ValidMnemonicSizes = { 9, 12, 15, 18, 21, 24 };
    // Default JSON Serialiser settings
    public static readonly JsonSerializerOptions SerialiserOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    // Commandline args switch mappings
    public static Dictionary<string, string> SwitchMappings => new()
    {
        { "--recovery-phrase", "mnemonic" },
        { "--verification-key-file", "verificationKeyFile" },
        { "--signing-key-file", "signingKeyFile" },
        { "--account-index", "accountIndex" },
        { "--address-index", "addressIndex" },
        { "--policy-index", "policyIndex" },
        { "--stake-account-index", "stakeAccountIndex" },
        { "--stake-address-index", "stakeAddressIndex" },
        { "--payment-address-type", "paymentAddressType" },
        { "--stake-address", "stakeAddress" },
        { "--cbor-hex", "cborHex" },
        { "--tx-id", "txId" },
        { "--signing-key", "signingKey" },
        { "--signing-keys", "signingKeys" },
        //{ "--output-format", "outputFormat" },
    };
}