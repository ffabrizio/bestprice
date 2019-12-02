using System.Collections.Generic;

namespace Laerdal.BestPrice.Models
{
    public class CalculationRequest
    {
        public string CustomerNumber { get; set; }
        public string ContractTypeId { get; set; }
        public IEnumerable<CalculationInput> Items { get; set; }
    }
}
