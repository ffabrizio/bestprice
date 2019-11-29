using BestPrice.Models;
using System.Collections.Generic;
using System.Linq;

namespace BestPrice
{
    public class CalculationResponse
    {
        public IEnumerable<CalculationOutput> Items { get; set; } = Enumerable.Empty<CalculationOutput>();
    }
}
