using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PeachClient.Models;
using RestSharp;
using RestSharp.Serializers.Json;
using SharpX;
using SharpX.Extensions;
using static SharpX.Guard;

namespace PeachClient;

public sealed class PeachApiClient
{
    private readonly ILogger _logger;
    private readonly PeachApiClientSettings _settings;
    private readonly RestClient _client;

    public PeachApiClient(ILogger<PeachApiClient> logger,
        IOptions<PeachApiClientSettings> options)
    {
        _logger = logger;
        _settings = options.Value;

        _client = new(new RestClientOptions
        {
            BaseUrl = _settings.ApiEndpoint,
            Timeout = TimeSpan.FromMilliseconds(_settings.TimeoutMs),
        }, configureSerialization: x => x.UseSystemTextJson(
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }));
    }

    public async Task<Maybe<SystemStatus>> GetSystemStatusAsync()
    {
        RestRequest request = new("system/status", Method.Get);
        SystemStatus? status = null;
        try {
            var response = await _client.ExecuteAsync<SystemStatus>(request);
            status = response?.Data;
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, "Failed to retrieve Peach system status");
        }

        return status!.ToMaybe();
    }

    public async Task<Maybe<AveragePrice>> GetBtcMarketPriceAsync(string fiat)
    {
        DisallowNull(nameof(fiat), fiat);
        DisallowEmptyWhitespace(nameof(fiat), fiat);

        RestRequest request = new($"market/price/BTC{fiat}", Method.Get);
        AveragePrice? price = null;
        try {
            var response = await _client.ExecuteAsync<AveragePrice>(request);
            price = response?.Data;
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, "Failed to retrieve market price");
        }

        return price!.ToMaybe();
    }

    public async Task<Maybe<OfferResponse>> SearchOffersAsync(OfferFilter filter,
        OfferPagination? pagination = null, OfferSortBy? sort = null)
    {
        DisallowNull(nameof(filter), filter);

        RestResponse response;
        try {
            var request = new RestRequest("offer/search", Method.Post);
            if (pagination != null) {
                request.AddQueryParameter("page", pagination.PageNumber.ToString(), encode: false);
                request.AddQueryParameter("size", pagination.PageSize.ToString(), encode: false);
            }
            if (sort != null) {
                request.AddQueryParameter("sortBy", sort.ToString().ToLowerFirst());
            }
            request.AddJsonBody(filter);

            response = await _client.ExecuteAsync(request);
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, "Failed to search offers");
            return Maybe.Nothing<OfferResponse>();
        }

        if (!response.IsSuccessful || response.Content.IsEmpty()) {
            return Maybe.Nothing<OfferResponse>();
        }

        // NOTE: in case of a BID offer the JSON respose is so massive that will cause serialization to fail
        var jsonDoc = JsonDocument.Parse(response.Content!);
        var offers = new List<Offer>();

        int total = 0, remaining = 0;
        if (jsonDoc.RootElement.TryGetProperty("total", out var totalElement)) {
            total = totalElement.GetInt32();
        }
        else {
            return BadSchema("total");
        }
        if (jsonDoc.RootElement.TryGetProperty("remaining", out var remainingElement)) {
            remaining = remainingElement.GetInt32();
        }
        else {
            return BadSchema("remaining");
        }

        if (jsonDoc.RootElement.TryGetProperty("offers", out var offersElement)) {
            foreach (var offerElement in offersElement.EnumerateArray()) {
                var offer = offerElement.Deserialize<Offer>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

                offers.Add(offer);
            }
        }
        else {
            return BadSchema("offers");
        }

        return new OfferResponse(offers, total, remaining).ToJust();

        Maybe<OfferResponse> BadSchema(string property)
        {
            _logger.LogCritical($"Unexpected response schema for offer search.\nCannot find '{property}' property");
            return Maybe.Nothing<OfferResponse>();
        }
    }

    public async Task<Maybe<Offer>> GetOfferAsync(string id)
    {
        DisallowNull(nameof(id), id);
        if (id.IsNumber()) throw new ArgumentException($"Invalid {nameof(id)} parameter format.", nameof(id));

        RestRequest request = new($"offer/{id}", Method.Get);
        Offer? offer = null;
        try {
            var response = await _client.ExecuteAsync<Offer>(request);
            offer = response?.Data;
        }
        catch (Exception ex) {
            _logger.LogCritical(ex, "Failed to retrieve market price");
        }

        return offer.ToMaybe();
    }
}
