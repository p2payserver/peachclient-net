using PeachClient;
using PeachClient.Models;
using SharpX;

public class PeachApiClientTests
{
    [Fact]
    public async Task Search_all_offers()
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var result = await client.SearchOffersAsync(new OfferFilter(
            Type: null,
            Amount: [100_000, 1_000_000],
            MeansOfPayment: new() { ["EUR"] = ["sepa"] },
            MaxPremium: 12,
            MinReputation: 0.5
        ));

        Assert.Equal(MaybeType.Just, result.Tag);
        var response = result.FromJust();
        if (response!.Total > 0) {
            Assert.NotEmpty(response.Offers);
        }
    }

}
