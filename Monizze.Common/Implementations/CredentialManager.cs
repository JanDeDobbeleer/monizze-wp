using System;
using System.Threading.Tasks;
using Monizze.Common.Interfaces;

namespace Monizze.Common.Implementations
{
    public partial class CredentialManager: ICredentialManager
    {
        private const string AccessToken = "1";
        private readonly IEncryptor _encryptor;

        public CredentialManager(IEncryptor encryptor)
        {
            _encryptor = encryptor;
        }

        public async Task<bool> IsLoggedIn()
        {
            try
            {
                return !string.IsNullOrWhiteSpace(await  GetToken());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> GetToken()
        {
            var token = await GetFromCache<string>(AccessToken);
            return token;
        }

        public async Task SaveToken(string token)
        {
            await SaveToCache(token, AccessToken);
        }

        public async Task Logout()
        {
            await ClearCache();
        }
    }
}
