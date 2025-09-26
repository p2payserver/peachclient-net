namespace PeachClient.Models;

public record KeySignatureInfo(
    string PublicKey,
    string Message,
    string Signature
);

public record TokenInfo(
    long Expiry,
    string AccessToken
);
