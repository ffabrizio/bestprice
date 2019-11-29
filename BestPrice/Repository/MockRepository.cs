using BestPrice.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BestPrice.Repository
{
    public class MockRepository : IBestPriceRepository
    {
        private readonly IHostEnvironment _environment;

        public IEnumerable<ContractType> ContractTypes { get; set; }
        public IEnumerable<CustomerPrices> CustomerPrices { get; set; }

        public MockRepository(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public ContractType GetContractType(string contractTypeId)
        {
            if (string.IsNullOrEmpty(contractTypeId))
            {
                return null;
            }

            var data = LoadRepository();
            return data.ContractTypes.FirstOrDefault(_ => _.ContractTypeId == contractTypeId);
        }

        public CustomerPrices GetCustomerPrices(string customerNumber)
        {
            if (string.IsNullOrEmpty(customerNumber))
            {
                return null;
            }

            var data = LoadRepository();
            return data.CustomerPrices.FirstOrDefault(_ => _.CustomerNumber == customerNumber);
        }

        private MockRepository LoadRepository()
        {
            Task.Delay(1000);

            var data = File.ReadAllText(Path.Combine(_environment.ContentRootPath, "data.json"));
            return JsonConvert.DeserializeObject<MockRepository>(data);
        }
    }
}
