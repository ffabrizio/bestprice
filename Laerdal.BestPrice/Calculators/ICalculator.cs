using Laerdal.BestPrice.Models;
using System.Threading.Tasks;

namespace Laerdal.BestPrice.Calculators
{
    public interface ICalculator
    {
        Task<CalculationResponse> Calculate(CalculationRequest req);
    }
}
