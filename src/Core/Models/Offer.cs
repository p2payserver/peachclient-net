using System.Text.Json.Serialization;
using PeachClient.Serialization;

namespace PeachClient.Models;

public enum OfferType
{
    Ask = 0,
    Bid
}

public record Offer(
    string Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] OfferType Type,
    User User,
    [property: JsonConverter(typeof(DecimalArrayFlexibleConverter))] decimal[] Amount,
    Dictionary<string, List<string>> MeansOfPayment,
    bool Online,
    DateTime? PublishingDate,
    decimal? Premium,
    Dictionary<string, decimal> Prices,
    string? Escrow
);

public sealed record OfferResponse(
    List<Offer> Offers,
    int Total,
    int Remaining
);
