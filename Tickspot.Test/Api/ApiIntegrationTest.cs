using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Tickspot.Api.Client;
using Tickspot.Api.Common;
using Logger = Tickspot.Common.Implementations.Logger;

namespace Tickspot.Test.Api
{
    [TestClass]
    public class ApiIntegrationTest
    {
        [TestMethod]
        public async Task LoginWithSuccess()
        {
            var credentialManager = new CredentialManager();
            var client = new TickSpotClient(new Logger(), credentialManager);
            ICredential credentials = new Credentials();
            var success = await client.Login(credentials.UserName, credentials.Password);
            Assert.IsTrue(success);
            Assert.IsTrue(credentialManager.HasCredentials());
            credentialManager.Clear();
        }
        
        [TestMethod]
        public async Task LoginWithFailure()
        {
            var credentialManager = new CredentialManager();
            var client = new TickSpotClient(new Logger(), credentialManager);
            ICredential credentials = new Credentials();
            var success = await client.Login(credentials.UserName, "crap");
            Assert.IsFalse(success);
            Assert.IsFalse(credentialManager.HasCredentials());
        }
    }
}
