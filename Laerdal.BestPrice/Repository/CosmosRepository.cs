using Laerdal.BestPrice.Extensions;
using Laerdal.BestPrice.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laerdal.BestPrice.Repository
{
    public class CosmosRepository : ICosmosRepository
    {
        private readonly Container _container;

        public CosmosRepository(
            CosmosClient client,
            string databaseName,
            string containerName)
        {
            _container = client.GetContainer(databaseName, containerName);
        }

        public async Task<ContractType> GetContractTypeAsync(string contractTypeId)
        {
            var contractType = AppCache.GetContractType(contractTypeId);
            if (contractType == null)
            {
                var iterator = _container.GetItemLinqQueryable<ContractRuleEntity>()
                    .Where(b => b.PartitionKey == contractTypeId)
                    .ToFeedIterator();

                var contractRules = new List<ContractRule>();

                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync())
                    {
                        contractRules.Add(item.ToModel());
                    }
                }

                contractType = new ContractType
                {
                    ContractTypeId = contractTypeId,
                    ContractRules = contractRules
                };

                AppCache.SetContractType(contractType);
            }

            return contractType;
        }

        public async Task<BatchOutput> UpsertContractTypeAsync(ContractType item)
        {
            var result = await DeleteAll(item.ContractTypeId);

            foreach (var rule in item.ContractRules)
            {
                var updatedItem = rule.ToEntity(item.ContractTypeId);

                await _container.CreateItemAsync(updatedItem);
                result.Added++;
            }

            AppCache.DeleteContractType(item.ContractTypeId);
            return result;
        }

        public async Task<BatchOutput> DeleteContractTypeAsync(string contractTypeId)
        {
            var result = await DeleteAll(contractTypeId);
            AppCache.DeleteContractType(contractTypeId);
            return result;
        }

        public async Task<CustomerPrices> GetCustomerPricesAsync(string customerNumber)
        {
            var customerPrices = AppCache.GetCustomerPrices(customerNumber);
            if (customerPrices == null)
            {
                var iterator = _container.GetItemLinqQueryable<ContractedPriceEntity>()
                    .Where(b => b.PartitionKey == customerNumber)
                    .ToFeedIterator();

                var contractedPrices = new List<ContractedPrice>();

                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync())
                    {
                        contractedPrices.Add(item.ToModel());
                    }
                }
                customerPrices = new CustomerPrices
                {
                    CustomerNumber = customerNumber,
                    ContractedPrices = contractedPrices
                };

                AppCache.SetCustomerPrices(customerPrices);
            }

            return customerPrices;
        }

        public async Task<BatchOutput> UpsertCustomerPricesAsync(CustomerPrices item)
        {
            var result = await DeleteAll(item.CustomerNumber);
            foreach (var price in item.ContractedPrices)
            {
                var updatedItem = price.ToEntity(item.CustomerNumber);

                await _container.CreateItemAsync(updatedItem);
                result.Added++;
            }

            AppCache.DeleteCustomerPrices(item.CustomerNumber);
            return result;
        }

        public async Task<BatchOutput> DeleteCustomerPricesAsync(string customerNumber)
        {
            var result = await DeleteAll(customerNumber);

            AppCache.DeleteCustomerPrices(customerNumber);
            return result;
        }

        private async Task<BatchOutput> DeleteAll(string pk)
        {
            var result = await _container.Scripts.ExecuteStoredProcedureAsync<BatchOutput>(Constants.SpId,
                partitionKey: new PartitionKey(pk),
                parameters: new dynamic[] { pk });

            var deleted = result.Resource.Deleted;
            while (result.Resource.Continuation)
            {
                result = await _container.Scripts.ExecuteStoredProcedureAsync<BatchOutput>(Constants.SpId,
                    partitionKey: new PartitionKey(pk),
                    parameters: new dynamic[] { pk });

                result.Resource.Deleted = deleted + result.Resource.Deleted;
                deleted = result.Resource.Deleted;
            }

            return result.Resource;
        }

    }
}
