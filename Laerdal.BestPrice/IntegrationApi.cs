using Laerdal.BestPrice.Models;
using Laerdal.BestPrice.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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

        [FunctionName("updatecontract")]
        public async Task<UpdateResponse> UpdateContract(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "contract/update")]
            ContractType req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            await _repository.UpsertContractTypeAsync(req);

            return new UpdateResponse { Success = true };
        }

        [FunctionName("deletecontract")]
        public async Task<UpdateResponse> DeleteContract(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "contract/delete")]
            ContractType req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await _repository.DeleteContractTypeAsync(req.ContractTypeId);

            return new UpdateResponse { Success = true };
        }

        [FunctionName("updatecustomerprices")]
        public async Task<UpdateResponse> UpdateCustomerPrices(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customer/update")]
            CustomerPrices req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await _repository.UpsertCustomerPricesAsync(req);

            return new UpdateResponse { Success = true };
        }

        [FunctionName("deletecustomerprices")]
        public async Task<UpdateResponse> DeleteCustomerPrices(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "customer/delete")]
            CustomerPrices req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            await _repository.DeleteCustomerPricesAsync(req.CustomerNumber);

            return new UpdateResponse { Success = true };
        }
    }
}
