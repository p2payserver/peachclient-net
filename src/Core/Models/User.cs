namespace PeachClient.Models;

public record Disputes(
    int Opened,
    int Won,
    int Lost
);

public sealed class User
{
    public string Id { get; set; }
    public DateTime CreationDate { get; set; }  
    public int Trades { get; set; }
    public double Rating { get; set; }
    public double HistoryRating { get; set; }
    public int RatingCount { get; set; }
    public double PeachRating { get; set; }
    public double UserRating { get; set; }
    public double RecentRating { get; set; }
    public List<string> Medals { get; set; } = [];
    public Disputes Disputes { get; set; }
    public string? PgpPublicKey { get; set; }
    public string? PgpPublicKeyProof { get; set; }
}
