namespace Cscli.ConsoleTool;

public record struct Utxo(string TxHash, int OutputIndex, TokenBundle TokenBundle);

public record struct TokenBundle(long LovelaceValue, NativeAssetValue[] NativeAssets);

public record struct NativeAssetValue(string PolicyId, string AssetNameHex, long Quantity);

public record WalletInfo(AccountInfo[] Accounts);

public record AccountInfo(string StakeAddress, string PaymentAddress, Utxo[] Utxos);

public record AddressInfo(string PaymentAddress, string? StakeAddress, Utxo[] Utxos);

public record TextEnvelope(string? Type, string? Description, string? CborHex);