using System;

namespace Laerdal.BestPrice.Models
{
    public class ContractRule : IContractRule
    {
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Quantity { get; set; } = 1;
        public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddYears(1);

        public override string ToString()
        {
            return $"[{AttributeName} / {AttributeValue}] {DiscountValue}%";
        }
    }
}
