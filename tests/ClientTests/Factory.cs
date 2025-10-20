using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PeachClient;
using SharpX.Extensions;

public static class Factory
{
    private static readonly TestsConfig _config = new();

    public static PeachApiClient CreatePeachClient()
    {
        var settings = _config.UseRegTestUri
            ? new PeachApiClientSettings { ApiEndpoint = "https://api-regtest.peachbitcoin.com/v1".ToUri()! }
            : new PeachApiClientSettings { }; // default is Production URI

        return new(NullLogger<PeachApiClient>.Instance, Options.Create(settings));
    }
}
