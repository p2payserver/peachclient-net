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
    private readonly JsonSerializerOptions _offerSerializerOptions;
    private AuthenticationInfo? _authInfo = null;

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

        _offerSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public async Task<Maybe<SystemStatus>> GetSystemStatusAsync()
    {
        RestRequest request = new("system/status", Method.Get);
        SystemStatus? status = null;

        try
        {
            var response = await _client.ExecuteAsync<SystemStatus>(request);
            if (!IsSuccessfulResponse(nameof(GetOfferAsync), response))
            {
                return Maybe.Nothing<SystemStatus>();
            }
            status = response?.Data;
        }
        catch (Exception ex)
        {
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

        try
        {
            var response = await _client.ExecuteAsync<AveragePrice>(request);
            if (!IsSuccessfulResponse(nameof(GetOfferAsync), response))
            {
                return Maybe.Nothing<AveragePrice>();
            }
            price = response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to retrieve market price");
        }

        return price!.ToMaybe();
    }

    public async Task<OfferResponse?> SearchOffersAsync(OfferFilter filter,
        OfferPagination? pagination = null, OfferSortBy? sort = null, bool skipPgpFields = false)
    {
        DisallowNull(nameof(filter), filter);

        if (skipPgpFields)
        {
            _offerSerializerOptions.Converters.Add(new UserSkipPgpFieldsConverter());
        }

        RestResponse response;
        try
        {
            var request = new RestRequest("offer/search", Method.Post);
            if (pagination != null)
            {
                request.AddQueryParameter("page", pagination.PageNumber.ToString(), encode: false);
                request.AddQueryParameter("size", pagination.PageSize.ToString(), encode: false);
            }
            if (sort != null)
            {
                request.AddQueryParameter("sortBy", sort.ToString().ToLowerFirst());
            }
            request.AddJsonBody(filter);

            response = await _client.ExecuteAsync(request);
            if (!IsSuccessfulResponse(nameof(GetOfferAsync), response))
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to search offers");
            return null;
        }

        if (!response.IsSuccessful || response.Content.IsEmpty())
        {
            return null;
        }

        // NOTE: in case of a BID offer the JSON respose is so massive that will cause serialization to fail
        var jsonDoc = JsonDocument.Parse(response.Content!);
        var offers = new List<Offer>();

        int total = 0, remaining = 0;
        if (jsonDoc.RootElement.TryGetProperty("total", out var totalElement))
        {
            total = totalElement.GetInt32();
        }
        else
        {
            return BadSchema("total");
        }
        if (jsonDoc.RootElement.TryGetProperty("remaining", out var remainingElement))
        {
            remaining = remainingElement.GetInt32();
        }
        else
        {
            return BadSchema("remaining");
        }

        if (jsonDoc.RootElement.TryGetProperty("offers", out var offersElement))
        {
            foreach (var offerElement in offersElement.EnumerateArray())
            {
                var offer = offerElement.Deserialize<Offer>(_offerSerializerOptions);

                offers.Add(offer);
            }
        }
        else
        {
            return BadSchema("offers");
        }

        return new OfferResponse(offers, total, remaining);

        OfferResponse? BadSchema(string property) => _logger.PanicWith<OfferResponse?>(
            $"Unexpected response schema for offer search.\nCannot find '{property}' property", null);
    }

    public async Task<Maybe<Offer>> GetOfferAsync(string id)
    {
        DisallowNull(nameof(id), id);
        if (id.IsNumber()) throw new ArgumentException($"Invalid {nameof(id)} parameter format.", nameof(id));

        RestRequest request = new($"offer/{id}", Method.Get);
        Offer? offer = null;

        try
        {
            var response = await _client.ExecuteAsync<Offer>(request);
            if (!IsSuccessfulResponse(nameof(GetOfferAsync), response))
            {
                return Maybe.Nothing<Offer>();
            }
            offer = response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to retrieve market price");
        }

        return offer.ToMaybe();
    }

    public Task<Maybe<ErrorInfo>> RegisterAccount(KeySignatureInfo accountInfo)
        => SubmitIdentity(accountInfo, register: true);

    public Task<Maybe<ErrorInfo>> AuthenticateAccount(KeySignatureInfo accountInfo)
        => SubmitIdentity(accountInfo, register: false);

    public async Task<Maybe<User>> GetIdentity()
    {
        RestRequest request = new("user/me", Method.Get);
        AuthenticateRequest(request);
        User? user = null;

        try
        {
            var response = await _client.ExecuteAsync<User>(request);
            if (!IsSuccessfulResponse(nameof(GetOfferAsync), response))
            {
                return Maybe.Nothing<User>();
            }
            user = response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to retrieve logged-in user data");
        }

        return user.ToMaybe();
    }

    public async Task<bool> UpdateUserAccount(UpdateUserRequest updateData)
    {
        DisallowNull(nameof(updateData), updateData);

        RestRequest request = new("user", Method.Post);
        AuthenticateRequest(request);
        request.AddJsonBody(updateData);

        try
        {
            var response = await _client.ExecuteAsync<OperationResult>(request);
            if (!IsSuccessfulResponse(nameof(GetOfferAsync), response))
            {
                return false;
            }

            return response.Data!.Success;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to update user account");
        }

        return false;
    }

    private async Task<Maybe<ErrorInfo>> SubmitIdentity(KeySignatureInfo accountInfo, bool register)
    {
        RestRequest request = new(register ? "user/register" : "user/auth", Method.Post);
        request.AddJsonBody(accountInfo);

        ErrorInfo? error = null;
        try
        {
            var response = await _client.ExecuteAsync<AuthenticationInfo>(request);
            error = ValidateResponse(nameof(SubmitIdentity), response);
            _authInfo = response.Data!;
            _logger.LogDebug($"Token '{_authInfo.AccessToken.Substring(9)}..' successfully registered within the current client instance");
        }
        catch (Exception ex)
        {
            error = new ErrorInfo(ErrorLevel.Critical, $"Unexpected error: {ex.Message}", ex);
        }

        return error.ToMaybe();
    }

    private bool IsSuccessfulResponse(string method, RestResponse response)
    {
        var error = ValidateResponse(method, response);

        return error == null
            ? true : error.Level == ErrorLevel.Default
                ? _logger.FailWith(error.Message) : _logger.PanicWith(error.Message);
    }

    private static ErrorInfo? ValidateResponse(string method, RestResponse response)
    {
        if (response == null)
        {
            return new ErrorInfo(ErrorLevel.Critical, $"{method} received a NULL response");
        }

        if (!response.IsSuccessful)
        {
            return new ErrorInfo(ErrorLevel.Default,
                $"{method} REST call failed with code {(int)response.StatusCode}");
        }

        if (response is RestResponse<object> genericResponse && genericResponse.Data == null)
        {
            return new ErrorInfo(ErrorLevel.Critical, $"{method} received a NULL data object");
        }

        return null;
    }

    private void AuthenticateRequest(RestRequest request)
    {
        if (_authInfo == null) throw new InvalidOperationException(
            "Cannot authenticate request: authentication info is not initialized.");

        request.AddHeader("Authorization", $"Bearer {_authInfo.AccessToken}");
    }
}
