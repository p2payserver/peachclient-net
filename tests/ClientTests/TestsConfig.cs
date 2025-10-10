public class TestsConfig
{
    public bool UseRegTestUri =>
        Convert.ToBoolean(Environment.GetEnvironmentVariable("UseRegTestUri"));

    public static TimeSpan DelayForProdApiMs =>
        TimeSpan.FromDays(Convert.ToDouble(Environment.GetEnvironmentVariable("DelayForProdApiMs")));

    public string PublicKey => Environment.GetEnvironmentVariable("PublicKey")!;
    public string Message => Environment.GetEnvironmentVariable("Message")!;
    public string Signature => Environment.GetEnvironmentVariable("Signature")!;
    public string PrivateKey => Environment.GetEnvironmentVariable("PrivateKey")!;
}
