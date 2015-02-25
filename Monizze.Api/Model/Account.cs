using System.Collections.Generic;
using Newtonsoft.Json;

namespace Monizze.Api.Model
{
    public class Account
    {
        [JsonProperty(PropertyName = "balance")]
        public string Balance { get; set; }
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "cardnumbers")]
        public List<string> CardNumbers { get; set; }
    }
}
