using BestPrice.Models;
using System.Collections.Generic;

namespace BestPrice
{
    public class CalculationRequest
    {
        public string CustomerNumber { get; set; }
        public string ContractTypeId { get; set; }
        public IEnumerable<CalculationInput> Items { get; set; }
    }
}
