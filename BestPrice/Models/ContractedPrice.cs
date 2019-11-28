using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPrice.Models
{
    public class ContractedPrice
    {
        public string Sku { get; set; }
        public bool IsPercentageValue { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
    }
}
