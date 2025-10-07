public static class TestsConfig
{
    public static bool UseRegTestUri =>
        Convert.ToBoolean(Environment.GetEnvironmentVariable("UseRegTestUri"));

    public static TimeSpan DelayForProdApiMs =>
        TimeSpan.FromDays(Convert.ToDouble(Environment.GetEnvironmentVariable("DelayForProdApiMs")));

    public static string PublicKey => Environment.GetEnvironmentVariable("PublicKey")!;
    public static string Message => Environment.GetEnvironmentVariable("Message")!;
    public static string Signature => Environment.GetEnvironmentVariable("Signature")!;
}
