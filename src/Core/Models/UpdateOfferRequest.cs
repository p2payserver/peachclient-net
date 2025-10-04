using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PeachClient.Models;
using SharpX.Extensions;

public sealed class InsertOfferRequest : IValidatableObject
{
    /// <summary>
    /// Type of offer.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required OfferType Type { get; init; }

    /// <summary>
    /// Range of sats to buy. First element is minimum, second is maximum.
    /// </summary>
    public required decimal[] Amount { get; set; }

    /// <summary>
    /// Maximum premium you are willing to pay (optional).
    /// </summary>
    public decimal? MaxPremium { get; set; }

    /// <summary>
    /// Currency keys mapped to allowed payment method IDs.
    /// </summary>
    public required Dictionary<string, List<string>> MeansOfPayment { get; init; }

    /// <summary>
    /// Payment data hashes: each payment method ID maps to field hashes.
    /// </summary>
    public required Dictionary<string, Dictionary<string, string>> PaymentData { get; init; }

    /// <summary>
    /// Bitcoin address to release funds to.
    /// </summary>
    public required string ReleaseAddress { get; init; }

    /// <summary>
    /// Buyer signature for releaseAddress control confirmation.
    /// </summary>
    public string? MessageSignature { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> validations = [];

        // Check type
        if (Type == OfferType.Ask && !MessageSignature.IsEmpty()) {
            validations.Add(new ValidationResult($"{nameof(MessageSignature)} must be empty for ask offers", [nameof(MessageSignature)]));
        }
        else if (Type == OfferType.Bid && MessageSignature.IsEmpty()) {
            validations.Add(new ValidationResult($"{nameof(MessageSignature)} cannot be empty for bid offers", [nameof(MessageSignature)]));
        }

        // Check Amount range
        if (Amount == null || Amount.Length != 2) {
            validations.Add(new ValidationResult($"{nameof(Amount)} must have exactly 2 elements (min and max)", [nameof(Amount)]));
        }
        else if (Amount[0] > Amount[1]) {
            validations.Add(new ValidationResult($"{nameof(Amount)} minimum must be less than or equal to maximum", [nameof(Amount)]));
        }

        // Optional: add basic Bitcoin address validation (regex or library)

        return validations;
    }
}
