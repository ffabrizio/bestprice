using System;

namespace Laerdal.BestPrice.Models
{
    public interface IContractedPrice
    {
        decimal DiscountValue { get; set; }
        bool IsPercentageValue { get; set; }
        decimal Quantity { get; set; }
        string Sku { get; set; }

        DateTime ValidFrom { get; set; }
        DateTime ValidTo { get; set; }
    }
}