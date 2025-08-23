namespace PeachClient.Models;

public record class SystemStatus(
    string? Error,
    string? Status,
    long ServerTime);
