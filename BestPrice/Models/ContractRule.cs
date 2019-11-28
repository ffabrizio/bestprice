using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPrice.Models
{
    public class ContractRule
    {
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
    }
}
