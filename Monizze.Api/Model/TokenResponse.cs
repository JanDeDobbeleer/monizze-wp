using Newtonsoft.Json;

namespace Monizze.Api.Model
{
    public class TokenResponse
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
