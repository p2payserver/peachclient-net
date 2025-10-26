using PeachClient;
using PeachClient.Models;
using SharpX;
using Xunit.Abstractions;

namespace PeachApiClient_Tests;

public class SearchOffers(ITestOutputHelper output)
{
    const int DEFAULT_DELAY_MS = 1000;

    //[Fact, Delay()]
    //public async Task Search_all_offer_types() => await SeachOffersAndAssertAsync(new OfferFilter
    //{
    //    Type = OfferTypeFilter.All,
    //    Amount = [100_000, 1_000_000],
    //    MeansOfPayment = new() { ["EUR"] = ["sepa"] },
    //    MaxPremium = 12,
    //    MinReputation = 0.5
    //});

    [Fact, Delay()]
    public async Task Search_ask_offer_types() =>
        await SeachOffersAndAssertAsync(OfferTypeFilter.Sell,
        assert: offers => Assert.All(offers, o => Assert.Equal(OfferType.Ask, o.Type)));

    [Fact, Delay()]
    public async Task Search_bid_offer_types() =>
        await SeachOffersAndAssertAsync(OfferTypeFilter.Buy,
        assert: offers => Assert.All(offers, o => Assert.Equal(OfferType.Bid, o.Type)));

    //[Fact, Delay()]
    //public async Task Search_offers_with_only_mandatory_filters() => await SeachOffersAndAssertAsync(new OfferFilter
    //{
    //    Type = OfferTypeFilter.All
    //});

    [Fact, Delay()]
    public async Task Search_offers_with_pagination() => await SeachOffersAndAssertAsync(
    OfferTypeFilter.Sell,
    pagination: new OfferPagination(0, 1),
    assert: offers => Assert.Single(offers),
    failOnEmpty: true);

    [Fact, Delay()]
    public async Task Search_offers_with_sort() => await SeachOffersAndAssertAsync(
    OfferTypeFilter.Buy,
    pagination: new OfferPagination(0, 2),
    sort: OfferSortBy.HighestAmount,
    assert: offers =>
    {
        Assert.Equal(2, offers.Count);
        Assert.True(offers.ElementAt(0).Amount > offers.ElementAt(1).Amount);
    },
    failOnEmpty: true);

    [Fact, Delay()]
    public async Task Get_offers_without_pgp_fields() => await SeachOffersAndAssertAsync(
    OfferTypeFilter.Sell,
    pagination: new OfferPagination(0, 5),
    sort: OfferSortBy.HighestAmount,
    skipPgpFields: true,
    assert: offers =>
        Assert.All(offers, o =>
        {
            Assert.Null(o.User.PgpPublicKey);
            Assert.Null(o.User.PgpPublicKeyProof);
        }),
    failOnEmpty: true);

    private async Task SeachOffersAndAssertAsync(OfferTypeFilter offerType, OfferPagination? pagination = null,
        OfferSortBy? sort = null,
        Action<List<Offer>>? assert = null, bool failOnEmpty = false, bool skipPgpFields = false)
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var response = await client.SearchOffersAsync(offerType, pagination, sort, skipPgpFields);

        Assert.NotNull(response);
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
