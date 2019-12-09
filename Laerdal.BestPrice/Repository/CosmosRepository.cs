using Laerdal.BestPrice.Extensions;
using Laerdal.BestPrice.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
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

        public async Task<BatchOutput> UpsertContractTypeAsync(ContractType item)
        {
            var result = await _container.Scripts.ExecuteStoredProcedureAsync<BatchOutput>(Constants.SpId,
                partitionKey: new PartitionKey(item.ContractTypeId),
                parameters: new dynamic[] { item.ContractTypeId });

            foreach (var rule in item.ContractRules)
            {
                var updatedItem = rule.ToEntity(item.ContractTypeId);

                await _container.CreateItemAsync(updatedItem);
                result.Resource.Added++;
            }

            AppCache.DeleteContractType(item.ContractTypeId);
            if (result.Resource.Deleted > 0)
            {
                result.Resource.Message = $"Updated contract rules for contract [{item.ContractTypeId}]";
            }
            else
            {
                result.Resource.Message = $"Created contract rules for contract [{item.ContractTypeId}]";
            }

            return result.Resource;
        }

        public async Task<BatchOutput> DeleteContractTypeAsync(string contractTypeId)
        {
            var result = await _container.Scripts.ExecuteStoredProcedureAsync<BatchOutput>(Constants.SpId,
                partitionKey: new PartitionKey(contractTypeId),
                parameters: new dynamic[] { contractTypeId });

            AppCache.DeleteContractType(contractTypeId);
            result.Resource.Message = $"Deleted contract rules for contract [{contractTypeId}]";
            return result.Resource;
        }

        public async Task<BatchOutput> UpsertCustomerPricesAsync(CustomerPrices item)
        {
            var result = await _container.Scripts.ExecuteStoredProcedureAsync<BatchOutput>(Constants.SpId,
                partitionKey: new PartitionKey(item.CustomerNumber),
                parameters: new dynamic[] { item.CustomerNumber });


            foreach (var price in item.ContractedPrices)
            {
                var updatedItem = price.ToEntity(item.CustomerNumber);

                await _container.CreateItemAsync(updatedItem);
                result.Resource.Added++;
            }

            AppCache.DeleteCustomerPrices(item.CustomerNumber);
            if (result.Resource.Deleted > 0)
            {
                result.Resource.Message = $"Updated contracted prices for customer [{item.CustomerNumber}]";
            }
            else
            {
                result.Resource.Message = $"Created contracted prices for customer [{item.CustomerNumber}]";
            }

            return result.Resource;
        }

        public async Task<BatchOutput> DeleteCustomerPricesAsync(string customerNumber)
        {
            var result = await _container.Scripts.ExecuteStoredProcedureAsync<BatchOutput>(Constants.SpId,
                partitionKey: new PartitionKey(customerNumber),
                parameters: new dynamic[] { customerNumber });

            AppCache.DeleteCustomerPrices(customerNumber);
            result.Resource.Message = $"Deleted contracted prices for customer [{customerNumber}]";
            return result.Resource;
        }
    }
}
