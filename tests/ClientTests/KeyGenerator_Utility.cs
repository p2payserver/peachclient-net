using NBitcoin;
using NBitcoin.DataEncoders;
using Xunit.Abstractions;

public class KeyGenerator_Utility(ITestOutputHelper output)
{
    [Fact(Skip = "Manually run this to generate Peach-compatible keys")]
    public void Generate_PeachKeys()
    {
        // 1. Generate random master key
        var master = new ExtKey(); // random seed
        string masterXprv = master.ToString(Network.Main); // <-- correct format for ExtKey.Parse

        // 2. Derive m/48'/0'/0'/0'
        var derived = master.Derive(new KeyPath("m/48'/0'/0'/0'"));
        var privateKey = derived.PrivateKey;
        var publicKey = privateKey.PubKey.Compress();

        // 3. Output everything
        output.WriteLine("=== Peach-compatible key generation ===");
        output.WriteLine($"Master Private Key (xprv for SignMessage): {masterXprv}");
        output.WriteLine($"Derived Private Key WIF: {privateKey.GetWif(Network.Main)}");
        output.WriteLine($"Compressed Public Key (hex): {Encoders.Hex.EncodeData(publicKey.ToBytes())}");
        output.WriteLine("=======================================");
    }
}

