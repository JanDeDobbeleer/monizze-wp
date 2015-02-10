using Newtonsoft.Json;

namespace Tickspot.Api.Model
{
    public class Token
    {
        [JsonProperty(PropertyName = "subscription_id")]
        public string SubscriptionId { get; set; }
        [JsonProperty(PropertyName = "company")]
        public string Company { get; set; }
        [JsonProperty(PropertyName = "api_token")]
        public string ApiToken { get; set; }
    }
}
