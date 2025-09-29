namespace PeachClient.Models;

public record KeySignatureInfo(
    string PublicKey,
    string Message,
    string Signature
);

public record AuthInfo(
    long Expiry,
    string AccessToken
);
