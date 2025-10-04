using System.ComponentModel.DataAnnotations;

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

public sealed class UpdateUserRequest : IValidatableObject
{
    public string? PgpPublicKey { get; set; }

    /// <summary>
    /// Required if <see cref="PgpPublicKey"/> is provided.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Required if <see cref="PgpPublicKey"/> is provided.
    /// </summary>
    public string? PgpSignature { get; set; }

    /// <summary>
    /// Required if <see cref="PgpPublicKey"/> is provided.
    /// </summary>
    public string? Signature { get; set; }

    public string? FcmToken { get; set; }

    public string? ReferralCode { get; set; }

    /// <summary>
    /// Can be a fixed numeric fee rate or one of the string values like 'fastestFee'.
    /// </summary>
    public string? FeeRate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> validations = [];

        if (PgpPublicKey != null)
        {
            if (Message == null)
                validations.Add(new ValidationResult($"{nameof(Message)} is required when {nameof(PgpPublicKey)} is provided",
                    [nameof(Message)]));

            if (PgpSignature == null)
                validations.Add(new ValidationResult($"{nameof(PgpSignature)} is required when {nameof(PgpPublicKey)} is provided",
                    [nameof(PgpSignature)]));

            if (Signature == null)
                validations.Add(new ValidationResult($"{nameof(Signature)} is required when {nameof(PgpPublicKey)} is provided",
                    [nameof(Signature)]));
        }

        return validations;
    }
}
