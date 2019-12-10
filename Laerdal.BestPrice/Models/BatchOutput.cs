using Newtonsoft.Json;

namespace Laerdal.BestPrice.Models
{
    public class BatchOutput
    {
        public int Deleted { get; set; }
        public int Added { get; set; }
        public bool Continuation { get; set; }
    }
}
