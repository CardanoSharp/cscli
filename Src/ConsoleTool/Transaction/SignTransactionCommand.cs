using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;

namespace Cscli.ConsoleTool.Transaction;

public class SignTransactionCommand : ICommand
{
    public string? CborHex { get; init; }
    public string? SigningKeys { get; init; }

    public ValueTask<CommandResult> ExecuteAsync(CancellationToken ct)
    {
        var (isValid, txCborBytes, signingKeys, errors) = Validate();
        if (!isValid)
        {
            return ValueTask.FromResult(CommandResult.FailureInvalidOptions(
                string.Join(Environment.NewLine, errors)));
        }

        var tx = txCborBytes.DeserializeTransaction();
        if (tx.TransactionWitnessSet is null)
        {
            tx.TransactionWitnessSet = new TransactionWitnessSet
            {
                VKeyWitnesses = new List<VKeyWitness>(),
            };
        }

        foreach (var signingKey in signingKeys)
        {
            var vKey = signingKey.GetPublicKey(false);
            var keyExistsInWitnessSet = tx.TransactionWitnessSet.VKeyWitnesses.Any(kw => kw.VKey.Key.SequenceEqual(vKey.Key));
            if (!keyExistsInWitnessSet)
            {
                tx.TransactionWitnessSet.VKeyWitnesses.Add(new VKeyWitness { SKey = signingKey, VKey = vKey });
            }
        }
        txCborBytes = tx.Serialize(); // CardanoSharp signs all vkey witnesses upon Serialize()
        return ValueTask.FromResult(CommandResult.Success(txCborBytes.ToStringHex()));
    }

    private (
        bool isValid,
        byte[] txCborBytes,
        PrivateKey[] signingKeys,
        IReadOnlyCollection<string> validationErrors) Validate()
    {
        var txCborBytes = Array.Empty<byte>();
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(CborHex))
        {
            validationErrors.Add(
                $"Invalid option --cbor-hex is required");
        }
        else
        {
            try
            {
                txCborBytes = Convert.FromHexString(CborHex);
            }
            catch (FormatException)
            {
                validationErrors.Add(
                    $"Invalid option --cbor-hex {CborHex} is not in hexadecimal format");
            }
        }
        var bech32SigningKeys = SigningKeys?.Split(',') ?? Array.Empty<string>();
        var signingKeys = new PrivateKey[bech32SigningKeys.Length];
        if (string.IsNullOrWhiteSpace(SigningKeys))
        {
            validationErrors.Add("Invalid option --signing-keys is required");
            return (isValid: false, txCborBytes, signingKeys, validationErrors);
        }
        for (int i = 0; i < bech32SigningKeys.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(bech32SigningKeys[i]))
            {
                validationErrors.Add($"Invalid option --signing-keys[{i}] is empty or whitespace");
                continue;
            }
            if (!Bech32.IsValid(bech32SigningKeys[i]))
            {
                validationErrors.Add($"Invalid option --signing-keys[{i}] is not a valid Bech32-encoded key");
                continue;
            }
            signingKeys[i] = TxUtils.GetPrivateKeyFromBech32SigningKey(bech32SigningKeys[i]);
        }
        return (!validationErrors.Any(), txCborBytes, signingKeys, validationErrors);
    }
}
