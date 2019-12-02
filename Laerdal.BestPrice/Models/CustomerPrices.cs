using Laerdal.BestPrice.Models;
using System.Collections.Generic;

namespace BestPrice.Models
{
    public class CustomerPrices
    {
        public string CustomerNumber { get; set; }
        public IEnumerable<IContractedPrice> ContractedPrices { get; set; }
    }
}
