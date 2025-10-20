using NBitcoin;
using Xunit.Abstractions;

public class KeyGenerator_Utility(ITestOutputHelper output)
{
    [Fact]
    public void Generate_PrivateKey()
    {
        var master = new ExtKey(); // random seed
        string masterXprv = master.ToString(Network.Main);

        output.WriteLine($"Master Private Key (xprv for SignMessage): {masterXprv}");
    }
}
