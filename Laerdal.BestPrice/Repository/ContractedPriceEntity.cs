using Laerdal.BestPrice.Models;
using Newtonsoft.Json;

namespace Laerdal.BestPrice.Repository
{
    public class ContractedPriceEntity : IContractedPrice
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("pk")]
        public string PartitionKey { get; set; }

        public string Sku { get; set; }
        public bool IsPercentageValue { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
    }
}
