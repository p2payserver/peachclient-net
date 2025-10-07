using PeachClient;
using PeachClient.Models;
using Xunit.Abstractions;

namespace PeachApiClientTests;

public class AuthTests(ITestOutputHelper output)
{
    [Fact(Skip = "Test must be manually activated")]
    public async Task Register_account()
    {
        PeachApiClient client = Factory.CreatePeachClient();

        var keyInfo = new KeySignatureInfo(TestsConfig.PublicKey, TestsConfig.Message,
            TestsConfig.Signature);
        var regResult = await client.RegisterAccountAsync(keyInfo);
        if (regResult.MatchJust(out var error)) {
            Assert.Fail(error!.Message);
        }

        output.WriteLine("Account registred");
    }
}
