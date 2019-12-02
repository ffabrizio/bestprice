﻿using Laerdal.BestPrice.Models;
using Newtonsoft.Json;

namespace Laerdal.BestPrice.Repository
{
    public class ContractRuleEntity : IContractRule
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("pk")]
        public string PartitionKey { get; set; }

        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
    }
}
