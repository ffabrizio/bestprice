using Laerdal.BestPrice.Models;
using Laerdal.BestPrice.Repository;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Laerdal.BestPrice.Calculators
{
    public class Calculator : ICalculator
    {
        private readonly ILogger<ICalculator> _logger;
        private readonly ICosmosRepository _repository;

        public Calculator(ICosmosRepository repository, ILogger<ICalculator> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CalculationResponse> Calculate(CalculationRequest req)
        {
            // Make a request to the repository and load rules for attribute-pricing and overriden prices on individual skus
            // It should be fast to load from a key/value store (e.g. Cosmos) but also SQL queries
            // The request should be awaited

            // Just load all the data for the requested 'contractTypeId'
            var contractType = await _repository.GetContractTypeAsync(req.ContractTypeId);

            // Just load all the data for the requested 'customerNumber'
            var customerPrices = await _repository.GetCustomerPricesAsync(req.CustomerNumber);

            // Prepare the response, all best prices are set to the list prices
            var res = new CalculationResponse
            {
                Items = req.Items.Select(_ => new CalculationOutput
                {
                    Sku = _.Sku,
                    ListPrice = _.ListPrice,
                    BestPrice = _.ListPrice
                }).ToArray()
            };

            // Loop through the items asynchronously
            Parallel.ForEach(req.Items, item =>
            {
                // Find the current sku
                var calculationOutput = res.Items.FirstOrDefault(_ => _.Sku == item.Sku);

                _logger.LogInformation("[{sku}] Calculating best price [{listPrice}]", item.Sku, item.ListPrice);

                if (contractType?.ContractRules != null)
                {
                    _logger.LogInformation("Found contract type {contractType}", req.ContractTypeId);

                    // Process all rules matching one of configured properties in the current sku
                    // Could this be generic without using reflection?
                    foreach (var rule in contractType.ContractRules.Where(_ =>
                        _.AttributeName == Constants.Sku && item.Sku == _.AttributeValue ||
                        _.AttributeName == Constants.ProductType && item.ProductType == _.AttributeValue ||
                        _.AttributeName == Constants.ProductGroup && item.ProductGroup == _.AttributeValue ||
                        _.AttributeName == Constants.ProductLine && item.ProductLine == _.AttributeValue
                    ))
                    {
                        var calculatedPrice = item.ListPrice - item.ListPrice * rule.DiscountValue / 100;

                        _logger.LogInformation("[{sku}] Contract type price: {calculatedPrice}. [{productType}, {productGroup}, {productLine}]",
                            item.Sku,
                            calculatedPrice,
                            item.ProductType,
                            item.ProductGroup,
                            item.ProductLine);

                        if (calculatedPrice > 0 && calculatedPrice < calculationOutput.BestPrice)
                        {
                            _logger.LogInformation("[{sku}] New best price: {calculatedPrice}", item.Sku, calculatedPrice);

                            // Replace the current best price if lower
                            calculationOutput.BestPrice = calculatedPrice;
                        }
                    }
                }

                if (customerPrices?.ContractedPrices != null)
                {
                    _logger.LogInformation("Found customer prices for customer number: {customerNumber}", req.CustomerNumber);

                    // Process all contracted prices for the current sku
                    foreach (var contractedPrice in customerPrices.ContractedPrices.Where(_ => _.Sku == item.Sku))
                    {
                        var calculatedPrice = contractedPrice.IsPercentageValue ?
                            item.ListPrice - item.ListPrice * contractedPrice.DiscountValue / 100 :
                            contractedPrice.DiscountValue;

                        _logger.LogInformation("[{sku}] Contracted price: {calculatedPrice}", item.Sku, calculatedPrice);

                        if (calculatedPrice > 0 && calculatedPrice < calculationOutput.BestPrice)
                        {
                            _logger.LogInformation("[{sku}] New best price: {calculatedPrice}", item.Sku, calculatedPrice);

                            // Replace the current best price if lower
                            calculationOutput.BestPrice = calculatedPrice;
                        }
                    }
                }
            });

            // Return the response
            return res;
        }
    }
}
