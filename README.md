# Peach Client .NET

Unofficial [Peach Bitcoin](https://peachbitcoin.com/index.html) API .NET client library.

Based on their official [API documentation](https://docs.peachbitcoin.com/#introduction).

**The project is incomplete and still in the early stages of development.**

## Target

- .NET 8.0


## Quick Start

```csharp
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<PeachApiClientSettings>().Configure(_ => {});
        services.AddSingleton<PeachApiClient>();
    });

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var client = host.Services.GetRequiredService<PeachApiClient>();

var response = await client.SearchOffersAsync(OfferFilterType.Sell,
    new OfferPagination(0, 10), OfferSortBy.LowestPremium);
if (response != null && !response.Offers.IsEmpty()) {
    foreach (var offer in response.Offers) {
        logger.LogInformation("{Offer}", ObjectDumper.Dump(offer));
    }
} 
```
