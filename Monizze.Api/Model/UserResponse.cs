using Newtonsoft.Json;

namespace Monizze.Api.Model
{
    public class UserResponse
    {
        [JsonProperty(PropertyName = "user")]
        public Account User { get; set; }
    }
}
