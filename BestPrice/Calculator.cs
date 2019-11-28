using BestPrice.Models;
using BestPrice.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace BestPrice
{
    public class Calculator : ICalculator
    {

        private readonly IBestPriceRepository _repository;

        public Calculator(IBestPriceRepository repository)
        {
            _repository = repository;
        }

        public async Task<CalculationResponse> Calculate(CalculationRequest req)
        {
            var contractType = _repository.GetContractType(req.ContractTypeId);
            var customerPrices = _repository.GetCustomerPrices(req.CustomerNumber);

            var res = new CalculationResponse
            {
                Items = req.Items.Select(_ => new CalculationOutput
                {
                    Sku = _.Sku,
                    ListPrice = _.ListPrice,
                    BestPrice = _.ListPrice
                }).ToArray()
            };

            Parallel.ForEach(req.Items, item =>
            {
                var calculationOutput = res.Items.FirstOrDefault(_ => _.Sku == item.Sku);

                if (contractType != null)
                {
                    foreach (var rule in contractType.ContractRules.Where(_ =>
                        (_.AttributeName == Constants.Sku && item.Sku == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductType && item.ProductType == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductGroup && item.ProductGroup == _.AttributeValue) ||
                        (_.AttributeName == Constants.ProductLine && item.ProductLine == _.AttributeValue)
                    ))
                    {
                        var calculatedPrice = item.ListPrice - (item.ListPrice * rule.DiscountValue / 100);
                        if (calculatedPrice > 0 && calculatedPrice < calculationOutput.BestPrice)
                        {
                            calculationOutput.BestPrice = calculatedPrice;
                        }
                    }
                }

                if (customerPrices != null)
                {
                    foreach (var contractedPrice in customerPrices.ContractedPrices.Where(_ => _.Sku == item.Sku))
                    {
                        var calculatedPrice = contractedPrice.IsPercentageValue ?
                            item.ListPrice - (item.ListPrice * contractedPrice.DiscountValue / 100) :
                            contractedPrice.DiscountValue;

                        if (calculatedPrice > 0 && calculatedPrice < calculationOutput.BestPrice)
                        {
                            calculationOutput.BestPrice = calculatedPrice;
                        }
                    }
                }
            });

            return res;
        }
    }
}
