using System.Reflection;
using Xunit.Sdk;

public class DelayAttribute : BeforeAfterTestAttribute
{
    public override void After(MethodInfo methodUnderTest)
    {
        if (TestsConfig.UseRegTestUri) return;

        Thread.Sleep(TestsConfig.DelayForProdApiMs);
    }
}
