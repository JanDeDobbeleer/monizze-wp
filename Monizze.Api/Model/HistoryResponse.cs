using System.Collections.Generic;
using Newtonsoft.Json;

namespace Monizze.Api.Model
{
    public class HistoryResponse
    {
        [JsonProperty(PropertyName = "history")]
        public List<Transaction> Transactions { get; set; }

        public HistoryResponse()
        {
            Transactions = new List<Transaction>();
        }
    }
}
