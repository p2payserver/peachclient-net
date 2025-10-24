# Peach Client .NET

## Overview
Unofficial C# client library for Peach Bitcoin API - a peer-to-peer platform for non-KYC Bitcoin trading. Currently in early development.

**Official API Docs**: https://docs.peachbitcoin.com/

## Architecture

### Core Components
- **PeachApiClient** (`src/Core/PeachApiClient.cs`) - Main API client with RestSharp HTTP wrapper
- **MessageSigner** (`src/Core/MessageSigner.cs`) - ECDSA signature generation for authentication
- **Models** (`src/Core/Models/`) - DTOs for offers, users, filters, requests

### Key Dependencies
- NBitcoin 9.0.1 - Bitcoin crypto operations
- RestSharp 112.1.0 - HTTP client
- SharpX 8.3.6 - Functional utilities (Maybe monad, guards)
- Microsoft.Extensions.* - Logging, DI, Options pattern

## Authentication Flow

Uses public key cryptography with BIP32 key derivation:

1. Derive key from master using path `m/48'/0'/0'/0'`
2. Sign message: `"Peach Registration {unix_timestamp_ms}"`
3. Create ECDSA signature (SHA256 hash, compact 64-byte format)
4. Submit to `/user/register` or `/user/auth` with public key + signature
5. Receive JWT access token (valid 60 min)
6. Include token in `Authorization: Bearer {token}` header

**Implementation**: `MessageSigner.CreateSignature()` in `MessageSigner.cs:13-42`

## API Methods

### Public Endpoints (No Auth)
- `GetSystemStatusAsync()` - System health check
- `GetBtcMarketPriceAsync(fiat)` - BTC price in specified fiat
- `SearchOffersAsync(offerType, pagination, sort)` - Search buy/sell offers (GET request to `/offer/search/sell` or `/offer/search/buy`)
- `GetOfferAsync(id)` - Get specific offer details

### Authenticated Endpoints
- `RegisterAccountAsync(privateKey)` - Create new account
- `AuthenticateAccountAsync(privateKey)` - Login to existing account
- `GetIdentity()` - Retrieve current user profile
- `UpdateUserAccount(updateData)` - Update user settings (PGP, FCM, referral, fees)
- `CreateOfferAsync(insertData)` - Post new buy/sell offer

## Data Models

### Offer
```csharp
Offer(Id, Type, User, Amount[], MeansOfPayment, Online, PublishingDate, Premium, Prices, Escrow)
```
- **Type**: Ask (sell) or Bid (buy)
- **Amount**: [min, max] range in satoshis
- **MeansOfPayment**: `{ "EUR": ["revolut", "sepa"], ... }`
- **Premium**: Percentage above/below market

### User
```csharp
User { Id, CreationDate, Trades, Rating, Medals[], Disputes, PgpPublicKey, ... }
```

### OfferTypeFilter
```csharp
enum OfferTypeFilter { Sell, Buy }
```
**Note**: `OfferFilter` class has been deprecated after Peach API breaking change. Search is now simplified to GET requests with type-based routing.

## Special Implementation Notes

### Offer Search
- **Breaking change**: Changed from POST with `OfferFilter` body to simple GET with type-based routing
- Routes: `/offer/search/sell` (for Sell) or `/offer/search/buy` (for Buy)
- Still supports pagination and sorting via query parameters
- Large JSON → custom `JsonDocument` parsing
- `skipPgpFields` option to trim payload
- Implemented in `SearchOffersAsync()` (`PeachApiClient.cs:97-179`)

### Error Handling
- Levels: Warning / Default / Critical  
- `ValidateResponse()` checks nulls, HTTP status, data  
- Logging via `FailWith()` and `PanicWith()`

### Serialization
- Custom converters: decimals, offer filters, PGP skipping  
- Global JSON: camelCase, ignore nulls  
- Separate options for offer-specific serialization

## Testing

Test structure in `tests/ClientTests/`:
- `SearchOffers.cs` - Offer search functionality
- `GetOffer.cs` - Single offer retrieval
- `Account.cs` - Registration/authentication
- `_More.cs` - Additional test scenarios

# Implementation Policies
- Keep the code as clean and simple as possible, while adhering to the existing style
- Do not remove features arbitrarily unless explicitly requested
- Do not create unit tests unless explicitly requested
