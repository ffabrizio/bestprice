using BestPrice.Models;
using System.Collections.Generic;
using System.Linq;

namespace BestPrice
{
    public class CalculationRequest
    {
        public string CustomerNumber { get; set; }
        public string ContractTypeId { get; set; }
        public IEnumerable<CalculationInput> Items { get; set; } = Enumerable.Empty<CalculationInput>();
    }
}
