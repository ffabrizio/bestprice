using Laerdal.BestPrice.Models;
using Newtonsoft.Json;
using System;

namespace Laerdal.BestPrice.Repository
{
    public class ContractRuleEntity : IContractRule
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("pk")]
        public string PartitionKey { get; set; }

        public string ProductGroup { get; set; }
        public string ProductLine { get; set; }
        public string ProductType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
