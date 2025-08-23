namespace PeachClient.Models;

public record class OfferFilter(
    string? Type,
    List<decimal> Amount,
    Dictionary<string, List<string>> MeansOfPayment,
    decimal MaxPremium,
    double MinReputation
);
