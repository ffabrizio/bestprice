using BestPrice.Models;
using System.Collections.Generic;

namespace BestPrice
{
    public class CalculationResponse
    {
        public IEnumerable<CalculationOutput> Items { get; set; }
    }
}
