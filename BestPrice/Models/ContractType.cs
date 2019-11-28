using System.Collections.Generic;

namespace BestPrice.Models
{
    public class ContractType
    {
        public string ContractTypeId { get; set; }
        public IEnumerable<ContractRule> ContractRules { get; set; }
    }
}
