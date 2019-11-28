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
            return await _calculator.Calculate(req);
        }
    }
}
