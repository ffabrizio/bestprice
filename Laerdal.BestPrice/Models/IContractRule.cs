using System;

namespace Laerdal.BestPrice.Models
{
    public interface IContractRule
    {
        string AttributeName { get; set; }
        string AttributeValue { get; set; }
        decimal DiscountValue { get; set; }
        decimal Quantity { get; set; }
        DateTime ValidFrom { get; set; }
        DateTime ValidTo { get; set; }
    }
}