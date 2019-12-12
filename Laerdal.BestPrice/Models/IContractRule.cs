using System;

namespace Laerdal.BestPrice.Models
{
    public interface IContractRule
    {
        string ProductGroup { get; set; }
        string ProductLine { get; set; }
        string ProductType { get; set; }

        decimal DiscountValue { get; set; }
        decimal Quantity { get; set; }
        DateTime ValidFrom { get; set; }
        DateTime ValidTo { get; set; }
    }
}