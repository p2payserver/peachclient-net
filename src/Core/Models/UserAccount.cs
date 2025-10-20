namespace PeachClient.Models;

internal record KeySignatureInfo(
    string PublicKey,
    string Message,
    string Signature,
    string? UniqueId = null
);

internal record AuthenticationInfo(
    long Expiry,
    string AccessToken
);
