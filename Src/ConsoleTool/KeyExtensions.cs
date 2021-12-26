using CardanoSharp.Wallet.Models.Keys;

namespace Cscli.ConsoleTool;

public static class KeyExtensions
{
    public static byte[] BuildExtendedKeyBytes(this PrivateKey prvKey)
    {
        var extendedKeyBytes = new byte[prvKey.Key.Length + prvKey.Chaincode.Length];
        Array.Copy(prvKey.Key, extendedKeyBytes, prvKey.Key.Length);
        Array.Copy(prvKey.Chaincode, 0, extendedKeyBytes, prvKey.Key.Length, prvKey.Chaincode.Length);
        return extendedKeyBytes;
    }

    public static byte[] BuildExtendedKeyBytes(this PublicKey pubKey)
    {
        var extendedKeyBytes = new byte[pubKey.Key.Length + pubKey.Chaincode.Length];
        Array.Copy(pubKey.Key, extendedKeyBytes, pubKey.Key.Length);
        Array.Copy(pubKey.Chaincode, 0, extendedKeyBytes, pubKey.Key.Length, pubKey.Chaincode.Length);
        return extendedKeyBytes;
    }
}
