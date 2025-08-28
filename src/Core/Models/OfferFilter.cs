using System.Text.Json.Serialization;
using PeachClient.Serialization;

namespace PeachClient.Models;

public enum OfferTypeFilter
{
    All = 0,
    Ask,
    Bid
}

public record class OfferFilter(
    [property: JsonConverter(typeof(OfferTypeFilterConverter))] OfferTypeFilter Type,
    List<decimal>? Amount,
    Dictionary<string, List<string>>? MeansOfPayment,
    decimal? MaxPremium,
    double? MinReputation
);
