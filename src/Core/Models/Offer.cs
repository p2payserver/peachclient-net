namespace PeachClient.Models;

public record Offer(
    string Id,
    string Type,
    //User User,
    decimal[] Amount,
    //Dictionary<string, List<string>> MeansOfPayment,
    bool Online,
    DateTime? PublishingDate,
    decimal Premium,
    Dictionary<string, decimal> Prices,
    string Escrow
);

public sealed record OfferResponse(
    List<Offer> Offers,
    int Total,
    int Remaining
);
