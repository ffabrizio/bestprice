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

        public async Task<ContractType> GetContractType(string contractTypeId)
        {
            if (string.IsNullOrEmpty(contractTypeId))
            {
                return null;
            }

            var cache = AppCache.GetContractType(contractTypeId);
            if (cache == null && !AppCache.IsNull(contractTypeId))
            {
                var connect = await LoadRepository();
                var data = connect.ContractTypes.FirstOrDefault(_ => _.ContractTypeId == contractTypeId);
                if (data == null)
                {
                    AppCache.AddNullKey(contractTypeId);
                    return null;
                }
                AppCache.SetContractType(data);
                cache = data;
            }

            return cache;
        }

        public async Task<CustomerPrices> GetCustomerPrices(string customerNumber)
        {
            if (string.IsNullOrEmpty(customerNumber))
            {
                return null;
            }

            var cache = AppCache.GetCustomerPrices(customerNumber);
            if (cache == null && !AppCache.IsNull(customerNumber))
            {
                var connect = await LoadRepository();
                var data = connect.CustomerPrices.FirstOrDefault(_ => _.CustomerNumber == customerNumber);
                if (data == null)
                {
                    AppCache.AddNullKey(customerNumber);
                    return null;
                }
                AppCache.SetCustomerPrices(data);
                cache = data;
            }

            return cache;
        }

        private async Task<MockRepository> LoadRepository()
        {
            await Task.Delay(1000);

            var data = File.ReadAllText(Path.Combine(_environment.ContentRootPath, "data.json"));
            return JsonConvert.DeserializeObject<MockRepository>(data);
        }
    }
}
