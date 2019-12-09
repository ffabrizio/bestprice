using System;

namespace Laerdal.BestPrice.Models
{
    public class ContractRule : IContractRule
    {
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public override string ToString()
        {
            return $"[{AttributeName} / {AttributeValue}] {DiscountValue}%";
        }
    }
}
