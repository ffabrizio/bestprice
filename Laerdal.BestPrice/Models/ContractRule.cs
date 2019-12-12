using System;

namespace Laerdal.BestPrice.Models
{
    public class ContractRule : IContractRule
    {
        public string ProductGroup { get; set; }
        public string ProductLine { get; set; }
        public string ProductType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public override string ToString()
        {
            return $"[{ProductGroup} / {ProductLine} / {ProductType}] {DiscountValue}%";
        }
    }
}
