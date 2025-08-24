using PeachClient;
using PeachClient.Models;
using SharpX;
using Xunit.Abstractions;

public class PeachApiClientTests(ITestOutputHelper output)
{
    [Fact]
    public async Task Search_all_offer_types() => await Seach_offers_and_assert(new OfferFilter(
        Type: OfferTypeFilter.All,
        Amount: [100_000, 1_000_000],
        MeansOfPayment: new() { ["EUR"] = ["sepa"] },
        MaxPremium: 12,
        MinReputation: 0.5
        ));

    [Fact]
    public async Task Search_ask_offer_types() => await Seach_offers_and_assert(new OfferFilter(
        Type: OfferTypeFilter.Ask,
        Amount: [100_000, 1_000_000],
        MeansOfPayment: new() { ["EUR"] = ["sepa"] },
        MaxPremium: 12,
        MinReputation: 0.5
        ),
        offers => Assert.All(offers, o => Assert.Equal(OfferType.Ask, o.Type)));

    [Fact]
    public async Task Search_bid_offer_types() => await Seach_offers_and_assert(new OfferFilter(
        Type: OfferTypeFilter.Bid,
        Amount: [100_000, 1_000_000],
        MeansOfPayment: new() { ["EUR"] = ["sepa"] },
        MaxPremium: 12,
        MinReputation: 0.5
        ),
        offers => Assert.All(offers, o => Assert.Equal(OfferType.Bid, o.Type)));

    [Fact]
    public async Task Search_all_offers_with_only_mandatory_filters() => await Seach_offers_and_assert(new OfferFilter(
        Type: OfferTypeFilter.All,
        Amount: [100_000, 1_000_000],
        MeansOfPayment: null,
        MaxPremium: null,
        MinReputation: null
    ));

    private async Task Seach_offers_and_assert(OfferFilter filter, Action<List<Offer>>? assertOnList = null)
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var result = await client.SearchOffersAsync(filter);

        Assert.Equal(MaybeType.Just, result.Tag);
        var response = result.FromJust();
        if (response!.Total > 0) {
            Assert.NotEmpty(response.Offers);
            assertOnList?.Invoke(response.Offers);
            output.WriteLine(ObjectDumper.Dump(response.Offers));
        }
        else {
            output.WriteLine("No offers found");
        }
    }
}
