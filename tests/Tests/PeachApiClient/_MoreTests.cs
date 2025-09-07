using PeachClient;
using PeachClient.Models;
using SharpX;
using SharpX.Extensions;
using Xunit.Abstractions;

namespace PeachApiClientTests;

public class _MoreTests(ITestOutputHelper output)
{
    [Fact]
    public async Task Get_BTC_unit_price_in_EUR()
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var result = await client.GetBtcMarketPriceAsync("EUR");

        Assert.Equal(MaybeType.Just, result.Tag);
        Assert.True(result.FromJust()!.Price > 0);
    }
}
