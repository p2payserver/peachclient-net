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

        MessageSigner signer = Factory.CreateSigner();

        var signResult = signer.CreateSignature(_config.PrivateKey);
        Assert.Equal(MaybeType.Just, signResult.Tag);
        var signInfo = signResult.FromJust();

        output.WriteLine("Pulick key: " + signInfo.Publickey);
        output.WriteLine("Message:    " + signInfo.Message);
        output.WriteLine("Signature:  " + signInfo.Value);
        var uniqueId = Guid.NewGuid().ToString().Replace("-", "");
        output.WriteLine("Unique ID:  " + uniqueId);

        PeachApiClient client = Factory.CreatePeachClient();

        var keyInfo = new KeySignatureInfo(signInfo.Publickey, signInfo.Message,
            signInfo.Value, uniqueId);
        var regResult = await client.RegisterAccountAsync(keyInfo);
        if (regResult.MatchJust(out var error)) {
            Assert.Fail(error!.Message);
        }

        output.WriteLine("Account registred");
    }


    [Fact(Skip = "Test must be manually activated")]
    public async Task Authenticate_account()
    {
        if (_config.UseRegTestUri) {
            Assert.Fail("Register_account test is intended for Peach production environment");
        }
        if (_config.PublicKey.IsEmpty() || _config.Message.IsEmpty() || _config.Signature.IsEmpty()) {
            Assert.Fail("Authentication data must be configured in test.runsettings file");
        }

        PeachApiClient client = Factory.CreatePeachClient();

        var keyInfo = new KeySignatureInfo(_config.PublicKey, _config.Message,
            _config.Signature);
        var regResult = await client.AuthenticateAccountAsync(keyInfo);
        if (regResult.MatchJust(out var error)) {
            Assert.Fail(error!.Message);
        }

        output.WriteLine("Account authenticated");
    }
}
