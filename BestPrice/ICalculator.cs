using System.Threading.Tasks;

namespace BestPrice
{
    public interface ICalculator
    {
        Task<CalculationResponse> Calculate(CalculationRequest req);
    }
}
