namespace Laerdal.BestPrice.Models
{
    public class CalculationInput
    {
        public string Sku { get; set; }
        public string ProductType { get; set; }
        public string ProductLine { get; set; }
        public string ProductGroup { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Quantity { get; set; } = 1;
    }
}
