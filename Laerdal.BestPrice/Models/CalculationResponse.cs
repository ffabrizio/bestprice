using System.Collections.Generic;
using System.Linq;

namespace Laerdal.BestPrice.Models
{
    public class CalculationResponse
    {
        public IEnumerable<CalculationOutput> Items { get; set; } = Enumerable.Empty<CalculationOutput>();
    }
}
