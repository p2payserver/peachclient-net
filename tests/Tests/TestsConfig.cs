public static class TestsConfig
{
    public static bool UseRegTestUri =>
        Convert.ToBoolean(Environment.GetEnvironmentVariable("UseRegTestUri"));

    public static TimeSpan DelayForProdApiMs =>
        TimeSpan.FromDays(Convert.ToDouble(Environment.GetEnvironmentVariable("DelayForProdApiMs")));
}
