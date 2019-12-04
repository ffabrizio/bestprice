using Laerdal.BestPrice.Models;
using System.Collections.Generic;
using System.Linq;

namespace Laerdal.BestPrice.Extensions
{
    public static class CalculatorExtensions
    {
        public static IEnumerable<ContractRule> GetApplicableRules(this IEnumerable<ContractRule> contractRules, CalculationOutput item)
        {
            return contractRules.Where(_ =>
                        (_.AttributeName == Constants.Sku && item.Sku == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductType && item.CalculationInput.ProductType == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductGroup && item.CalculationInput.ProductGroup == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductLine && item.CalculationInput.ProductLine == _.AttributeValue)
                    );
        }

        public static decimal CalculateDiscountedPrice(this ContractRule rule, CalculationOutput item)
        { 
            return item.ListPrice - item.ListPrice * rule.DiscountValue / 100;
        }

        public static IEnumerable<ContractedPrice> GetApplicablePrices(this IEnumerable<ContractedPrice> contractedPrices, CalculationOutput item)
        {
            return contractedPrices.Where(_ => _.Sku == item.Sku);
        }

        public static decimal CalculateDiscountedPrice(this ContractedPrice contractedPrice, CalculationOutput item)
        {
            return contractedPrice.IsPercentageValue ?
                item.ListPrice - item.ListPrice * contractedPrice.DiscountValue / 100 :
                contractedPrice.DiscountValue;
        }
    }
}
