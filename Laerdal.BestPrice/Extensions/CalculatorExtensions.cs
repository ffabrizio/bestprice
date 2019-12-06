using Laerdal.BestPrice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laerdal.BestPrice.Extensions
{
    public static class CalculatorExtensions
    {
        public static IEnumerable<ContractRule> GetApplicableRules(this IEnumerable<ContractRule> contractRules, CalculationOutput item)
        {
            var validRules = contractRules.Where(_ => _.ValidFrom <= DateTime.UtcNow && _.ValidTo >= DateTime.UtcNow).ToArray();
            var applicableRules = validRules
                .Where(_ =>
                    (_.AttributeName == Constants.Sku && item.Sku == _.AttributeValue) ||
                    (_.AttributeName == Constants.ProductType && item.CalculationInput.ProductType == _.AttributeValue) ||
                    (_.AttributeName == Constants.ProductGroup && item.CalculationInput.ProductGroup == _.AttributeValue) ||
                    (_.AttributeName == Constants.ProductLine && item.CalculationInput.ProductLine == _.AttributeValue)
                ).ToArray();

            if (applicableRules.Any())
            {
                var tieredRules = applicableRules
                    .GroupBy(_ => $"{_.AttributeName}:{_.AttributeValue}")
                    .Select(_ => _.OrderByDescending(t => t.Quantity)
                        .FirstOrDefault(t => t.Quantity <= item.CalculationInput.Quantity));

                return tieredRules;
            }

            return applicableRules;
        }

        public static decimal CalculateDiscountedPrice(this ContractRule rule, CalculationOutput item)
        { 
            return item.ListPrice - item.ListPrice * rule.DiscountValue / 100;
        }

        public static IEnumerable<ContractedPrice> GetApplicablePrices(this IEnumerable<ContractedPrice> contractedPrices, CalculationOutput item)
        {
            var validPrices = contractedPrices.Where(_ => _.ValidFrom <= DateTime.UtcNow && _.ValidTo >= DateTime.UtcNow).ToArray();
            var applicablePrices = validPrices.Where(_ => _.Sku == item.Sku).ToArray();
            if (applicablePrices.Any())
            {
                var tieredPrices = applicablePrices
                    .GroupBy(_ => _.Quantity)
                    .OrderByDescending(_ => _.Key)
                    .FirstOrDefault(t => t.Key <= item.CalculationInput.Quantity)
                    .ToArray();

                return tieredPrices;
            }

            return applicablePrices;
        }

        public static decimal CalculateDiscountedPrice(this ContractedPrice contractedPrice, CalculationOutput item)
        {
            return contractedPrice.IsPercentageValue ?
                item.ListPrice - item.ListPrice * contractedPrice.DiscountValue / 100 :
                contractedPrice.DiscountValue;
        }
    }
}
