using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Laerdal.BestPrice.Models;
using Laerdal.BestPrice.Calculators;

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
        public async Task<CalculationResponse> Run(
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
