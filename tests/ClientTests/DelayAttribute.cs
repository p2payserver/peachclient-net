using System.Reflection;
using Xunit.Sdk;

public class DelayAttribute : BeforeAfterTestAttribute
{
    private readonly TestsConfig _config = new();

    public override void After(MethodInfo methodUnderTest)
    {
        if (_config.UseRegTestUri) return;

        Thread.Sleep(TestsConfig.DelayForProdApiMs);
    }
}
