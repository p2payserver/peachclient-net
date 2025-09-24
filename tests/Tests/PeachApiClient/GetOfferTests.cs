using PeachClient;
using PeachClient.Models;
using SharpX;
using SharpX.Extensions;
using Xunit.Abstractions;

namespace PeachApiClientTests;

public class GetOfferTests(ITestOutputHelper output)
{
    const int DEFAULT_DELAY_MS = 500;

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Get_an_ask_offer() => await GetOfferAndAssertAsync(OfferType.Ask);

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Get_a_bid_offer() => await GetOfferAndAssertAsync(OfferType.Bid);

    private async Task GetOfferAndAssertAsync(OfferType type)
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var offerId = await PickOfferIdAsync(client,
            type == OfferType.Ask ? OfferTypeFilter.Ask : OfferTypeFilter.Bid);

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

    private async Task<string> PickOfferIdAsync(PeachApiClient client, OfferTypeFilter type)
    {
        var response = await client.SearchOffersAsync(new OfferFilter
        {
            Type = type,
            Amount = [100_000, 1_000_000],
            MeansOfPayment = new() { ["EUR"] = ["sepa"] },
            MaxPremium = 12,
            MinReputation = 0.5
        });

        if (response == null || response.Total == 0) {
            Assert.Fail($"Unable to get any offer of type {type}");
        }

        output.WriteLine($"Found {response.Offers.Count} offers of type {type}");

        return response.Offers.Choice().Id;
    }
}
