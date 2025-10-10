namespace PeachClient;

public enum ErrorLevel
{
    Warning,
    Default,
    Critical
}

public record ErrorInfo(
    ErrorLevel Level,
    string Message,
    Exception? Exception = null
);

public record SignatureInfo(
    string Publickey,
    string Message,
    string Value
);
