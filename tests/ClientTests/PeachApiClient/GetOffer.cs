using PeachClient;
using PeachClient.Models;
using SharpX;
using SharpX.Extensions;
using Xunit.Abstractions;

namespace PeachApiClient_Tests;

public class GetOffer(ITestOutputHelper output)
{
    [Fact, Delay()]
    public async Task Get_an_ask_offer() => await GetOfferAndAssertAsync(OfferType.Ask);

    [Fact, Delay()]
    public async Task Get_a_bid_offer() => await GetOfferAndAssertAsync(OfferType.Bid);

    private async Task GetOfferAndAssertAsync(OfferType type)
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var offerId = await PickOfferIdAsync(client,
            type == OfferType.Ask ? OfferTypeFilter.Sell : OfferTypeFilter.Buy);

        var result = await client.GetOfferAsync(offerId);

        Assert.True(result.IsJust());
        var offer = result.FromJust()!;
        output.WriteLine(ObjectDumper.Dump(offer));
        switch (type) {
            case OfferType.Ask:
                Assert.Equal(OfferType.Ask, offer.Type);
                break;
            default:
                Assert.Equal(OfferType.Bid, offer.Type);
                break;
        }
    }

    private async Task<int> PickOfferIdAsync(PeachApiClient client, OfferTypeFilter type)
    {
        var response = await client.SearchOffersAsync(type);

        if (response == null || response.Total == 0) {
            Assert.Fail($"Unable to get any offer of type {type}");
        }

        output.WriteLine($"Found {response.Offers.Count} offers of type {type}");

        return response.Offers.Choice().Id;
    }
}
