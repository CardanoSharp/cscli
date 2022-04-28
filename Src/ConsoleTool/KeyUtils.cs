﻿using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;

namespace Cscli.ConsoleTool;

public static class KeyUtils
{
    public static byte[] BuildNonExtendedSkeyWithVerificationKeyBytes(this PrivateKey prvKey)
    {
        var pubKey = prvKey.GetPublicKey(false);
        var skeyBytes = new byte[prvKey.Key.Length + pubKey.Key.Length];
        Array.Copy(prvKey.Key, skeyBytes, prvKey.Key.Length);
        Array.Copy(pubKey.Key, 0, skeyBytes, prvKey.Key.Length, pubKey.Key.Length);
        return skeyBytes;
    }

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
