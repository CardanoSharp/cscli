using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;

namespace Cscli.ConsoleTool;

public static class KeyUtils
{
    private const int MinimumSigningKeyLength = 32;

    public static byte[] BuildBasicSkeyBytes(this PrivateKey prvKey)
        => prvKey.Key.Length > MinimumSigningKeyLength 
            ? prvKey.Key[..MinimumSigningKeyLength] : prvKey.Key;

    public static byte[] BuildExtendedSkeyBytes(this PrivateKey prvKey)
    {
        var extendedKeyBytes = new byte[prvKey.Key.Length + prvKey.Chaincode.Length];
        Array.Copy(prvKey.Key, extendedKeyBytes, prvKey.Key.Length);
        Array.Copy(prvKey.Chaincode, 0, extendedKeyBytes, prvKey.Key.Length, prvKey.Chaincode.Length);
        return extendedKeyBytes;
    }

    public static byte[] BuildExtendedSkeyWithVerificationKeyBytes(this PrivateKey prvKey)
    {
        var pubKey = prvKey.GetPublicKey(false);
        var extendedKeyBytes = new byte[prvKey.Key.Length + pubKey.Key.Length + prvKey.Chaincode.Length];
        Array.Copy(prvKey.Key, extendedKeyBytes, prvKey.Key.Length);
        Array.Copy(pubKey.Key, 0, extendedKeyBytes, prvKey.Key.Length, pubKey.Key.Length);
        Array.Copy(prvKey.Chaincode, 0, extendedKeyBytes, prvKey.Key.Length + pubKey.Key.Length, prvKey.Chaincode.Length);
        return extendedKeyBytes;
    }

    public static byte[] BuildExtendedVkeyBytes(this PublicKey pubKey)
    {
        var extendedKeyBytes = new byte[pubKey.Key.Length + pubKey.Chaincode.Length];
        Array.Copy(pubKey.Key, extendedKeyBytes, pubKey.Key.Length);
        Array.Copy(pubKey.Chaincode, 0, extendedKeyBytes, pubKey.Key.Length, pubKey.Chaincode.Length);
        return extendedKeyBytes;
    }

    public static string BuildCborHexPayload(byte[] keyPayload)
        => $"58{keyPayload.Length:x2}{Convert.ToHexString(keyPayload).ToLower()}";
}
