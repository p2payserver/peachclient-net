namespace PeachClient.Models;

public record Disputes(
    int Opened,
    int Won,
    int Lost
);

public sealed record User(
    string Id,
    DateTime CreationDate,
    int Trades,
    double Rating,
    double HistoryRating,
    int RatingCount,
    double PeachRating,
    double UserRating,
    double RecentRating,
    List<string> Medals,
    Disputes Disputes,
    string PgpPublicKey,
    string PgpPublicKeyProof
);
