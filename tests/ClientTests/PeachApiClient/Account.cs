using System.Xml;
using PeachClient;
using PeachClient.Models;
using SharpX;
using SharpX.Extensions;
using Xunit.Abstractions;

namespace PeachApiClient_Tests;

public class Account(ITestOutputHelper output)
{
    private readonly TestsConfig _config = new();

    [Fact(Skip = "Test must be manually activated")]
    public async Task Register_account()
    {
        if (_config.UseRegTestUri) {
            Assert.Fail("Register_account test is intended for Peach production environment");
        }
        if (_config.PrivateKey.IsEmpty()) {
            Assert.Fail("Private key need to be configured in test.runsettings file");
        }

        PeachApiClient client = Factory.CreatePeachClient();

        var regResult = await client.RegisterAccountAsync(_config.PrivateKey);
        if (regResult.MatchJust(out var error)) {
            Assert.Fail(error!.Message);
        }

        output.WriteLine("Account registred");
    }


    [Fact(Skip = "Test must be manually activated")]
    public async Task Authenticate_account()
    {
        if (_config.UseRegTestUri) {
            Assert.Fail("Authenticate_account test is intended for Peach production environment");
        }
        if (_config.PrivateKey.IsEmpty()) {
            Assert.Fail("Private key need to be configured in test.runsettings file");
        }

        PeachApiClient client = Factory.CreatePeachClient();

        var regResult = await client.AuthenticateAccountAsync(_config.PrivateKey);
        if (regResult.MatchJust(out var error)) {
            Assert.Fail(error!.Message);
        }

        output.WriteLine("Account authenticated");
    }
}
