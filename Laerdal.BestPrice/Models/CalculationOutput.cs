using Newtonsoft.Json;

namespace Laerdal.BestPrice.Models
{
    public class CalculationOutput
    {
        public CalculationOutput(CalculationInput calculationInput)
        {
            CalculationInput = calculationInput;
            Sku = calculationInput.Sku;
            ListPrice = calculationInput.ListPrice;
            BestPrice = calculationInput.ListPrice;
        }

        public string Sku { get; set; }
        public decimal ListPrice { get; set; }
        public decimal BestPrice { get; set; }

        [JsonIgnore]
        public CalculationInput CalculationInput { get; private set; }
    }
}
