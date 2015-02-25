using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Monizze.Api.Client;
using Monizze.Common.Model;
using Logger = Monizze.Common.Implementations.Logger;

namespace Monizze.Test.Api
{
    [TestClass]
    public class ApiIntegrationTest
    {
        [TestMethod]
        public async Task Login()
        {
            var credentialManager = new CredentialManager();
            var client = new MonizzeClient(new Logger(), credentialManager);
            var credentials = new TestCredentials();
            var success = await client.Login(credentials.UserName, credentials.Password);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task GetAccount()
        {
            var credentialManager = new CredentialManager();
            var client = new MonizzeClient(new Logger(), credentialManager);
            var response = await client.GetAccount();
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Balance));
        }

        [TestMethod]
        public async Task GetTransactions()
        {
            var credentialManager = new CredentialManager();
            var client = new MonizzeClient(new Logger(), credentialManager);
            var transactions = await client.GetTransactions();
            Assert.IsTrue(transactions.Any());
        }
    }
}
