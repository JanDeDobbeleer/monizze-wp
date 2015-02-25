using System;
using Newtonsoft.Json;

namespace Monizze.Api.Model
{
    public class Transaction
    {
        [JsonProperty(PropertyName = "transaction_id")]
        public string Transactionid { get; set; }
        [JsonProperty(PropertyName = "merchant_name")]
        public string MerchantName { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
        [JsonProperty(PropertyName = "date")]
        public string TimeStamp { get; set; }
    }
}
