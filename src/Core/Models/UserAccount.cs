namespace PeachClient.Models;

public record KeySignatureInfo(
    string PublicKey,
    string Message,
    string Signature,
    string? UniqueId = null
);

public record AuthenticationInfo(
    long Expiry,
    string AccessToken
);
