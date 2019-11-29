using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BestPrice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BestPriceController : ControllerBase
    {
        private readonly ILogger<BestPriceController> _logger;
        private readonly ICalculator _calculator;

        public BestPriceController(ILogger<BestPriceController> logger, ICalculator calculator)
        {
            _logger = logger;
            _calculator = calculator;
        }

        [HttpPost]
        public async Task<CalculationResponse> CalculateBestPrice(CalculationRequest req)
        {
            if (!req.Items.Any())
            {
                _logger.LogWarning("No valid inputs in the request received, skipping calculations...");
                return new CalculationResponse();
            }

            if (string.IsNullOrEmpty(req.CustomerNumber))
            {
                _logger.LogWarning("No customer number provided in the request received, input prices will be returned...");
            }

            return await _calculator.Calculate(req);
        }
    }
}
