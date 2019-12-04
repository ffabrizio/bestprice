using Laerdal.BestPrice.Calculators;
using Laerdal.BestPrice.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Laerdal.BestPrice
{
    public class BestPriceApi
    {
        private readonly ICalculator _calculator;

        public BestPriceApi(ICalculator calculator)
        {
            _calculator = calculator;
        }

        [FunctionName("BestPrice")]
        public async Task<CalculationResponse> CalculateBestPrice(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "bestprice")]
            CalculationRequest req,
            ILogger log)
        {
            log.LogInformation($"Executing with input: {req.CustomerNumber}");

            var calculation = await _calculator.Calculate(req);
            return calculation;
        }
    }
}
