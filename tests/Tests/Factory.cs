using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PeachClient;

public static class Factory
{
    public static PeachApiClient CreatePeachClient() => new(NullLogger<PeachApiClient>.Instance,
        Options.Create(new PeachApiClientSettings { }));
}
