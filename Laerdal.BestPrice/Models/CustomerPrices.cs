using System.Collections.Generic;

namespace Laerdal.BestPrice.Models
{
    public class CustomerPrices
    {
        public string CustomerNumber { get; set; }
        public IEnumerable<ContractedPrice> ContractedPrices { get; set; }
    }
}
