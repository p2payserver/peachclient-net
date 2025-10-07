using NBitcoin;
using NBitcoin.DataEncoders;
using Xunit.Abstractions;

public class KeyGenerator(ITestOutputHelper output)
{
    [Fact(Skip = "Test must be manually activated")]
    public void Generate_keys()
    {
        output.WriteLine("Generating keys for configuration in test.runsettings file");

        // Use a random mnemonic
        var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
        var rootKey = mnemonic.DeriveExtKey();

        // Derive m/48'/0'/0'/0/0
        var path = new KeyPath("m/48'/0'/0'/0/0");
        var derivedKey = rootKey.Derive(path);

        var privateKey = derivedKey.PrivateKey; // this is your ECDSA private key
        var publicKey = privateKey.PubKey.Compress(); // compressed public key

        output.WriteLine("Private Key (WIF): " + privateKey.GetWif(Network.Main));
        output.WriteLine("Public Key (Hex): " + Encoders.Hex.EncodeData(publicKey.ToBytes()));
    }
}
