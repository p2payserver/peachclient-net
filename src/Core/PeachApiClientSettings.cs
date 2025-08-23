using System.ComponentModel.DataAnnotations;

namespace PeachClient;

public sealed class PeachApiClientSettings
{
    public const string SectionName = "peachApiClient";

    public Uri ApiEndpoint { get; set; } = new Uri("https://api.peachbitcoin.com/v1");

    [Range(10, 300000, ErrorMessage = $"{nameof(TimeoutMs)} must range between 10 to 300000 milliseconds")]
    public int TimeoutMs { get; set; } = 5000;
}
