using System.Collections.Generic;

namespace BestPrice.Models
{
    public class CustomerPrices
    {
        public string CustomerNumber { get; set; }
        public IEnumerable<ContractedPrice> ContractedPrices { get; set; }
    }
}
