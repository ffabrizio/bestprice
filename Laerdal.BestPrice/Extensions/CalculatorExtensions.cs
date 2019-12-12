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
            var ruleValues = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(rule.ProductGroup))
                ruleValues.Add(Constants.ProductGroup, rule.ProductGroup);
            if (!string.IsNullOrEmpty(rule.ProductLine))
                ruleValues.Add(Constants.ProductLine, rule.ProductLine);
            if (!string.IsNullOrEmpty(rule.ProductType))
                ruleValues.Add(Constants.ProductType, rule.ProductType);

            var isMatch = true;
            foreach (var item in ruleValues)
            {
                if (item.Key == Constants.ProductGroup && item.Value != input.ProductGroup) isMatch = false;
                if (item.Key == Constants.ProductLine && item.Value != input.ProductLine) isMatch = false;
                if (item.Key == Constants.ProductType && item.Value != input.ProductType) isMatch = false;
            }

            return isMatch;
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
