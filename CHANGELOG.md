# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

- Refactored registration and authentication methods.
- Implemented `GetIdentity` method.
- Implemented `UpdateUserAccount` method.
- Implemented `CreateOffer` method.
- Refactored `SearchOffersAsync` for API breaking changes.

## [0.1.6-preview] - 2025-09-26

- Implemented registration and authentication methods.

## [0.1.5-preview] - 2025-09-24

- Implemented `Get_BTC_unit_price_in_EUR` test.
- Implemented way to limit `User` object size.
- Removed `Maybe<T>` from `SearchOffersAsync` method.

## [0.1.4-preview] - 2025-09-06

- Implemented `GetOfferAsync` method.
- Implemented `ValidateResponse` guard method.

## [0.1.3-preview] - 2025-08-31

- Perfected `Offer` type.
- Implemented `DelayAttribute` to not stress Peach server.

## [0.1.2-preview] - 2025-08-31

- Implemented pagination in `SearchOffersAsync` method.
- Implemented sort in `SearchOffersAsync` method.

## [0.1.1-preview] - 2025-08-29

- Updated definition of `OfferFilter` class.
