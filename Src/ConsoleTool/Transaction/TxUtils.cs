using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Models.Keys;

namespace Cscli.ConsoleTool.Transaction;

public static class TxUtils
{
    public static bool IsZero(this Balance value)
    {
        return value.Lovelaces == 0 && value.NativeAssets.Length == 0;
    }
    public static Balance Sum(this IEnumerable<Balance> values)
    {
        var lovelaces = 0UL;
        var nativeAssets = new Dictionary<(string PolicyId, string AssetNameHex), ulong>();
        foreach (var value in values)
        {
            lovelaces += value.Lovelaces;
            foreach (var nativeAsset in value.NativeAssets)
            {
                if (nativeAssets.ContainsKey((nativeAsset.PolicyId, nativeAsset.AssetName)))
                {
                    nativeAssets[(nativeAsset.PolicyId, nativeAsset.AssetName)] += nativeAsset.Quantity;
                    continue;
                }
                nativeAssets.Add((nativeAsset.PolicyId, nativeAsset.AssetName), nativeAsset.Quantity);
            }
        }
        return new Balance(
            lovelaces,
            nativeAssets.Select(nav => new NativeAssetValue(nav.Key.PolicyId, nav.Key.AssetNameHex, nav.Value)).ToArray());
    }

    public static Balance Subtract(this Balance lhsValue, Balance rhsValue)
    {
        static NativeAssetValue SubtractSingleValue(NativeAssetValue lhsValue, NativeAssetValue rhsValue)
        {
            return rhsValue == default
                ? lhsValue
                : new NativeAssetValue(lhsValue.PolicyId, lhsValue.AssetName, lhsValue.Quantity - rhsValue.Quantity);
        };

        if (rhsValue.NativeAssets.Length == 0)
            return new Balance(lhsValue.Lovelaces - rhsValue.Lovelaces, lhsValue.NativeAssets);

        var missingLhsValues = rhsValue.NativeAssets
            .Where(rna => !lhsValue.NativeAssets
                .Any(lna => lna.PolicyId == rna.PolicyId && lna.AssetName == rna.AssetName))
            .ToArray();
        if (missingLhsValues.Any())
            throw new ArgumentException("lhsValue is missing Native Assets found on rhsValue", nameof(rhsValue));

        var nativeAssets = lhsValue.NativeAssets
            .Select(lv => SubtractSingleValue(
                lv,
                rhsValue.NativeAssets.FirstOrDefault(
                    rv => rv.PolicyId == lv.PolicyId && rv.AssetName == lv.AssetName)))
            .Where(na => na.Quantity != 0)
            .ToArray();
        return new Balance(lhsValue.Lovelaces - rhsValue.Lovelaces, nativeAssets);
    }

    public static ulong CalculateMinUtxoLovelace(
        NativeAssetValue[] tokenBundle,
        int lovelacePerUtxoWord = 34482, // utxoCostPerWord in protocol params (could change in the future)
        int policyIdSizeBytes = 28, // 224 bit policyID (won't in forseeable future)
        bool hasDataHash = false) // for UTxOs with smart contract datum
    {
        // https://docs.cardano.org/native-tokens/minimum-ada-value-requirement#min-ada-valuecalculation
        const int fixedUtxoPrefixWords = 6;
        const int fixedUtxoEntryWithoutValueSizeWords = 27; // The static parts of a UTxO: 6 + 7 + 14 words
        const int coinSizeWords = 2; // since updated from 0 in docs.cardano.org/native-tokens/minimum-ada-value-requirement
        const int adaOnlyUtxoSizeWords = fixedUtxoEntryWithoutValueSizeWords + coinSizeWords;
        const int fixedPerTokenCost = 12;
        const int byteRoundUpAddition = 7;
        const int bytesPerWord = 8; // One "word" is 8 bytes (64-bit)
        const int fixedDataHashSizeWords = 10;

        var isAdaOnly = tokenBundle.Length == 0;
        if (isAdaOnly)
            return (ulong)lovelacePerUtxoWord * adaOnlyUtxoSizeWords; // 999978 lovelaces or 0.999978 ADA

        // Get distinct policyIDs and assetNames
        var policyIds = new HashSet<string>();
        var assetNameHexadecimals = new HashSet<string>();
        foreach (var customToken in tokenBundle)
        {
            policyIds.Add(customToken.PolicyId);
            assetNameHexadecimals.Add(customToken.AssetName);
        }

        // Calculate (prefix + (numDistinctPids * 28(policyIdSizeBytes) + numTokens * 12(fixedPerTokenCost) + tokensNameLen + 7) ~/8)
        var tokensNameLen = assetNameHexadecimals.Sum(an => an.Length) / 2; // 2 hexadecimal chars = 1 Byte
        var valueSizeWords = fixedUtxoPrefixWords + (
            (policyIds.Count * policyIdSizeBytes)
            + (tokenBundle.Length * fixedPerTokenCost)
            + tokensNameLen + byteRoundUpAddition) / bytesPerWord;
        var dataHashSizeWords = hasDataHash ? fixedDataHashSizeWords : 0;

        var minUtxoLovelace = lovelacePerUtxoWord
            * (fixedUtxoEntryWithoutValueSizeWords + valueSizeWords + dataHashSizeWords);

        return (ulong)minUtxoLovelace;
    }

    public static PrivateKey GetPrivateKeyFromBech32SigningKey(string bech32EncodedSigningKey)
    {
        var keyBytes = Bech32.Decode(bech32EncodedSigningKey, out _, out _);
        // Extended signing key "*_xsk" 64 bytes key + 32 bytes chain-code (optional)
        // or non-extended "*_sk" 32 bytes key
        return keyBytes.Length >= 64
            ? new PrivateKey(keyBytes[..64], keyBytes[64..])
            : new PrivateKey(keyBytes[..32], Array.Empty<byte>());
    }
}
