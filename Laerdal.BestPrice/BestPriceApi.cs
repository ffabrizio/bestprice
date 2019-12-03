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

        [FunctionName("bestprice")]
        public async Task<CalculationResponse> CalculateBestPrice(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            CalculationRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var calculation = await _calculator.Calculate(req);
            return calculation;
        }
    }
}
