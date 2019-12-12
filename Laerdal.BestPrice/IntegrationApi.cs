using Laerdal.BestPrice.Models;
using Laerdal.BestPrice.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laerdal.BestPrice
{
    public class IntegrationApi
    {
        private readonly ICosmosRepository _repository;

        public IntegrationApi(ICosmosRepository repository)
        {
            _repository = repository;

        }

        [FunctionName("GetContract")]
        public async Task<ContractType> GetContract(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "contract/{id}")]
            HttpRequestMessage req,
            string id,
            ILogger log)
        {

            log.LogInformation($"[admin] Loading contract [{id}]");

            return await _repository.GetContractTypeAsync(id);
        }

        [FunctionName("UpdateContract")]
        public async Task<BatchOutput> UpdateContract(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "contract/update")]
            ContractType req,
            ILogger log)
        {

            log.LogInformation($"[admin] Updating contract [{req.ContractTypeId}]");

            return await _repository.UpsertContractTypeAsync(req);
        }

        [FunctionName("DeleteContract")]
        public async Task<BatchOutput> DeleteContract(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "contract/delete")]
            ContractType req,
            ILogger log)
        {
            log.LogInformation($"[admin] Deleting contract [{req.ContractTypeId}]");

            return await _repository.DeleteContractTypeAsync(req.ContractTypeId);
        }

        [FunctionName("GetCustomerPrices")]
        public async Task<CustomerPrices> GetCustomerPrices(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customer/{id}")]
            HttpRequestMessage req,
            string id,
            ILogger log)
        {

            log.LogInformation($"[admin] Loading contracted prices [{id}]");

            return await _repository.GetCustomerPricesAsync(id);
        }

        [FunctionName("UpdateCustomerPrices")]
        public async Task<BatchOutput> UpdateCustomerPrices(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customer/update")]
            CustomerPrices req,
            ILogger log)
        {
            log.LogInformation($"[admin] Updating contracted prices [{req.CustomerNumber}]");

            return await _repository.UpsertCustomerPricesAsync(req);
        }

        [FunctionName("DeleteCustomerPrices")]
        public async Task<BatchOutput> DeleteCustomerPrices(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "customer/delete")]
            CustomerPrices req,
            ILogger log)
        {

            log.LogInformation($"[admin] Deleting contracted prices [{req.CustomerNumber}]");

            return await _repository.DeleteCustomerPricesAsync(req.CustomerNumber);
        }
    }
}
