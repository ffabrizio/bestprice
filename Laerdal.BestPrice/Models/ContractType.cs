using System.Collections.Generic;

namespace Laerdal.BestPrice.Models
{
    public class ContractType
    {
        public string ContractTypeId { get; set; }
        public IEnumerable<ContractRule> ContractRules { get; set; }
    }
}
