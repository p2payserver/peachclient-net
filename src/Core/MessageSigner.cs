// derived from: https://github.com/Nisaba/btcpayserver-plugins/blob/master/BTCPayServer.Plugins.Peach/Services/PeachService.cs#L312
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.Crypto;
using SharpX;

namespace PeachClient;

internal sealed class MessageSigner(ILogger logger)
{
    public Maybe<SignatureInfo> CreateSignature(string privateKeyHex)
    {
        var message = $"Peach Registration {new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()}";

        try {
            var master = ExtKey.Parse(privateKeyHex, Network.Main);
            var derived = master.Derive(new KeyPath("m/48'/0'/0'/0'"));

            var privKey = derived.PrivateKey;
            var pubKey = privKey.PubKey;

            using SHA256 sha256 = SHA256.Create();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] messageHash = sha256.ComputeHash(messageBytes);

            uint256 hash = new uint256(messageHash);
            ECDSASignature signature = privKey.Sign(hash);
            byte[] compactSig = signature.ToCompact();

            if (compactSig.Length != 64) {
                throw new InvalidOperationException($"Compact signature length is {compactSig.Length}, expected 64.");
            }
            return new SignatureInfo(pubKey.ToHex(), message, BitConverter.ToString(compactSig).Replace("-", "").ToLower()).ToJust();
        }
        catch (Exception ex) {
            logger.LogCritical(ex, "Failed to sign message");
            
            return Maybe.Nothing<SignatureInfo>();
        }
    }
}
