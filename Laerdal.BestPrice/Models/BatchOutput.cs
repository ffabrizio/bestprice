using Newtonsoft.Json;

namespace Laerdal.BestPrice.Models
{
    public class BatchOutput
    {
        public string Message { get; set; }
        public int Deleted { get; set; }
        public int Added { get; set; }

        [JsonIgnore]
        public bool Continuation { get; set; }
    }
}
