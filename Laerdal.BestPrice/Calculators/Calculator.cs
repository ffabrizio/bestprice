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
            // Make a request to the repository and load rules for contract type and best prices on individual SKUs

            // Load all the rules for the requested 'contractTypeId'
            var contractType = await _repository.GetContractTypeAsync(req.ContractTypeId);

            // Load all the prices for the requested 'customerNumber'
            var customerPrices = await _repository.GetCustomerPricesAsync(req.CustomerNumber);

            // Prepare the response, all best prices are set to the list prices
            var res = new CalculationResponse
            {
                Items = req.Items.Select(_ => new CalculationOutput(_)).ToArray()
            };

            // Loop through the items in parallel
            Parallel.ForEach(res.Items, item =>
            {
                _logger.LogInformation("[{sku}] Calculating best price [{listPrice}]", item.Sku, item.ListPrice);

                if (contractType?.ContractRules != null)
                {
                    _logger.LogInformation("Found contract type {contractType}", req.ContractTypeId);

                    // Process all rules matching one of configured properties in the current SKU
                    foreach (var rule in contractType.ContractRules.Where(_ =>
                        (_.AttributeName == Constants.Sku && item.Sku == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductType && item.CalculationInput.ProductType == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductGroup && item.CalculationInput.ProductGroup == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductLine && item.CalculationInput.ProductLine == _.AttributeValue)
                    ))
                    {
                        var calculatedPrice = item.ListPrice - item.ListPrice * rule.DiscountValue / 100;

                        _logger.LogInformation("[{sku}] Contract type price: {calculatedPrice}. {rule}",
                            item.Sku,
                            calculatedPrice,
                            item.CalculationInput.ProductType,
                            item.CalculationInput.ProductGroup,
                            item.CalculationInput.ProductLine);

                        if (calculatedPrice > 0 && calculatedPrice < item.BestPrice)
                        {
                            _logger.LogInformation("[{sku}] New best price: {calculatedPrice}", item.Sku, calculatedPrice);

                            // Replace the current best price if lower
                            item.BestPrice = calculatedPrice;
                        }
                    }
                }

                if (customerPrices?.ContractedPrices != null)
                {
                    _logger.LogInformation("Found customer prices for customer number: {customerNumber}", req.CustomerNumber);

                    // Process all contracted prices for the current SKU
                    foreach (var contractedPrice in customerPrices.ContractedPrices.Where(_ => _.Sku == item.Sku))
                    {
                        var calculatedPrice = contractedPrice.IsPercentageValue ?
                            item.ListPrice - item.ListPrice * contractedPrice.DiscountValue / 100 :
                            contractedPrice.DiscountValue;

                        _logger.LogInformation("[{sku}] Contracted price: {calculatedPrice}", item.Sku, calculatedPrice);

                        if (calculatedPrice > 0 && calculatedPrice < item.BestPrice)
                        {
                            _logger.LogInformation("[{sku}] New best price: {calculatedPrice}", item.Sku, calculatedPrice);

                            // Replace the current best price if lower
                            item.BestPrice = calculatedPrice;
                        }
                    }
                }
            });

            // Return the response
            return res;
        }
    }
}
