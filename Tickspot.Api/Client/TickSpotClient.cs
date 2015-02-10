using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tickspot.Api.Common;
using Tickspot.Api.Model;
using Tickspot.Common.Interfaces;

namespace Tickspot.Api.Client
{
    public class TickSpotClient: RestClient
    {
        public TickSpotClient(ILogger logger, ICredentialManager credentialManager) : base(logger, credentialManager)
        {
        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await Request<List<Token>>("/api/v2/roles.json", null, new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password)))));
            if (!response.Success)
                return false;
            var tokens = response.Content as List<Token>;
            if (tokens == null)
                return false;
            try
            {
                foreach (var token in tokens)
                {
                    CredentialManager.SaveToken(token);
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetType() + " Could not save credential", e);
                return false;
            }
            return true;
        }
    }
}
