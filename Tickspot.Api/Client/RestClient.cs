using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tickspot.Api.Common;
using Tickspot.Common.Interfaces;

namespace Tickspot.Api.Client
{
    public struct Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public struct Response
    {
        public bool Success { get; set; }
        public object Content { get; set; }
    }

    public class RestClient: StoreToCache
    {
        private const string BaseUrl = "https://www.tickspot.com";

        protected readonly ICredentialManager CredentialManager;

        public RestClient(ILogger logger, ICredentialManager credentialManager) : base(logger)
        {
            CredentialManager = credentialManager;
        }

        public async Task<Response> Request<T>(string resource, List<Parameter> parameters, AuthenticationHeaderValue authHeader = null) where T : class, new() 
        {
            using (var client = GetDefaultClient())
            {
                client.DefaultRequestHeaders.Authorization = authHeader ?? new AuthenticationHeaderValue("Token", string.Format("token={0}", CredentialManager.GetTokenForId(resource.Split('/')[0])));
                var response = await client.GetAsync(resource);
                if (!response.IsSuccessStatusCode)
                    return new Response {Content = null, Success = false};
                var contentString = await response.Content.ReadAsStringAsync();
                try
                {
                    var content = JsonConvert.DeserializeObject<T>(contentString);
                    return new Response {Content = content, Success = true};
                }
                catch (Exception e)
                {
                    Logger.Error(GetType() + " error logging in ", e);
                    return new Response {Content = null, Success = false};
                }
            }
        }

        private HttpClient GetDefaultClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            client.DefaultRequestHeaders.Add("User-Agent", "TickspotAppWP (me@example.com)");
            return client;
        }
    }
}
