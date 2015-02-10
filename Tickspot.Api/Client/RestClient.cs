using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tickspot.Api.Client
{
    public class RestClient
    {
        private const string BaseUrl = "https://citylive.tickspot.com";
        private const string EntryUrl = "/api/v2/roles.json";
        
        public async Task<bool> Login(string username, string password)
        {
            var byteArray = Encoding.UTF8.GetBytes(username + ":" + password);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Add("User-Agent", "MyCoolApp (me@example.com)");
                var response = await client.GetAsync(EntryUrl);
                return (response.StatusCode.Equals(HttpStatusCode.OK));
            }
            
        }


    }
}
