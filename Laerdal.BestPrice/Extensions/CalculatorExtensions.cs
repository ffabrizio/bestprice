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
                .Where(_ => _.IsMatch(item.CalculationInput)).ToArray();

            if (applicableRules.Any())
            {
                var tieredRules = applicableRules
                    .GroupBy(_ => _.Quantity)
                    .OrderByDescending(_ => _.Key)
                    .FirstOrDefault(t => t.Key <= item.CalculationInput.Quantity)
                    .ToArray();

                return tieredRules;
            }

            return applicableRules;
        }

        private static bool IsMatch(this ContractRule rule, CalculationInput input)
        {
            var ruleValues = new Dictionary<string, string>
            {
                { Constants.ProductGroup, rule.ProductGroup },
                { Constants.ProductLine, rule.ProductLine },
                { Constants.ProductType, rule.ProductType }
            };
            var inputValues = new Dictionary<string, string>
            {
                { Constants.ProductGroup, input.ProductGroup },
                { Constants.ProductLine, input.ProductLine },
                { Constants.ProductType, input.ProductType }
            };

            return ruleValues
                .Where(_ => !string.IsNullOrEmpty(_.Value))
                .All(_ => _.Value == inputValues[_.Key]);
        }

        public static decimal CalculateDiscountedPrice(this ContractRule rule, decimal listPrice)
        {
            return listPrice - listPrice * rule.DiscountValue / 100;
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

        public static decimal CalculateDiscountedPrice(this ContractedPrice contractedPrice, decimal listPrice)
        {
            return contractedPrice.IsPercentageValue ?
                listPrice - listPrice * contractedPrice.DiscountValue / 100 :
                contractedPrice.DiscountValue;
        }
    }
}
