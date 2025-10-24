using System.Text.Json.Serialization;
using PeachClient.Serialization;

namespace PeachClient.Models;

public enum OfferTypeFilter
{
    Sell = 0,
    Buy
}

// DEPRECATED: OfferFilter no longer used after Peach API breaking change
// Offer search is now a simple GET request with only type filter
/*
public record class OfferFilter
{
    [property: JsonConverter(typeof(OfferTypeFilterConverter))]
    public OfferTypeFilter Type { get; init; }
    public List<decimal>? Amount { get; set; }
    public Dictionary<string, List<string>>? MeansOfPayment { get; set; }
    public decimal? MaxPremium { get; set; }
    public double? MinReputation { get; set; }
}
*/

public record class OfferPagination(int PageNumber, int PageSize);

public enum OfferSortBy
{
   BestReputation,
   HighestAmount,
   HighestPrice,
   LowestPremium
}
