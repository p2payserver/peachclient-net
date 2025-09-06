using PeachClient;
using PeachClient.Models;
using SharpX;
using Xunit.Abstractions;

namespace PeachApiClientTests;

public class SearchOffersTests(ITestOutputHelper output)
{
    const int DEFAULT_DELAY_MS = 1000;

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Search_all_offer_types() => await SeachOffersAndAssertAsync(new OfferFilter
    {
        Type = OfferTypeFilter.All,
        Amount = [100_000, 1_000_000],
        MeansOfPayment = new() { ["EUR"] = ["sepa"] },
        MaxPremium = 12,
        MinReputation = 0.5
    });

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Search_ask_offer_types() => await SeachOffersAndAssertAsync(new OfferFilter
    {
        Type = OfferTypeFilter.Ask,
        Amount = [100_000, 1_000_000],
        MeansOfPayment = new() { ["EUR"] = ["sepa"] },
        MaxPremium = 12,
        MinReputation = 0.5
    },
    assert: offers => Assert.All(offers, o => Assert.Equal(OfferType.Ask, o.Type)));

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Search_bid_offer_types() => await SeachOffersAndAssertAsync(new OfferFilter
    {
        Type = OfferTypeFilter.Bid,
        Amount = [100_000, 1_000_000],
        MeansOfPayment = new() { ["EUR"] = ["sepa"] },
        MaxPremium = 12,
        MinReputation = 0.5
    },
    assert: offers => Assert.All(offers, o => Assert.Equal(OfferType.Bid, o.Type)));

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Search_offers_with_only_mandatory_filters() => await SeachOffersAndAssertAsync(new OfferFilter
    {
        Type = OfferTypeFilter.All
    });

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Search_offers_with_pagination() => await SeachOffersAndAssertAsync(new OfferFilter
    {
        Type = OfferTypeFilter.All,
    },
    pagination: new OfferPagination(0, 1),
    assert: offers => Assert.Single(offers),
    failOnEmpty: true);

    [Fact, Delay(DEFAULT_DELAY_MS)]
    public async Task Search_offers_with_sort() => await SeachOffersAndAssertAsync(new OfferFilter
    {
        Type = OfferTypeFilter.Ask,
    },
    pagination: new OfferPagination(0, 2),
    sort: OfferSortBy.HighestAmount,
    assert: offers =>
    {
        Assert.Equal(2, offers.Count);
        Assert.True(offers.ElementAt(0).Amount[0] > offers.ElementAt(1).Amount[0]);
    },
    failOnEmpty: true);

    private async Task SeachOffersAndAssertAsync(OfferFilter filter, OfferPagination? pagination = null,
        OfferSortBy? sort = null,
        Action<List<Offer>>? assert = null, bool failOnEmpty = false)
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var result = await client.SearchOffersAsync(filter, pagination: pagination, sort: sort);

        Assert.Equal(MaybeType.Just, result.Tag);
        var response = result.FromJust();
        if (response!.Total > 0) {
            Assert.NotEmpty(response.Offers);
            assert?.Invoke(response.Offers);
            output.WriteLine(ObjectDumper.Dump(response.Offers));
        }
        else {
            if (failOnEmpty) { Assert.Fail("No offers found"); }
            else { output.WriteLine("No offers found"); }
        }
    }
}
