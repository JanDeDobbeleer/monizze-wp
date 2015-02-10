using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Tickspot.Api.Client;

namespace Tickspot.Test.Api
{
    [TestClass]
    public class ApiTest
    {
        [TestMethod]
        public async Task Login()
        {
            var client = new RestClient();
            ICredential credentials = new Credentials();
            var success = await client.Login(credentials.UserName, credentials.Password);
            Assert.IsTrue(success);
        }
    }
}
