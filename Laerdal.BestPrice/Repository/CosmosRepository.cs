using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BestPrice.Models;
using Laerdal.BestPrice.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Laerdal.BestPrice.Repository
{
    public class CosmosRepository : ICosmosRepository
    {
        private Container _container;

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
                        contractRules.Add(
                            new ContractRule
                            {
                                AttributeName = item.AttributeName,
                                AttributeValue = item.AttributeValue,
                                DiscountValue = item.DiscountValue,
                                Quantity = item.Quantity
                            }
                        );
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
                        contractedPrices.Add(
                            new ContractedPrice
                            {
                                IsPercentageValue = item.IsPercentageValue,
                                Sku = item.Sku,
                                DiscountValue = item.DiscountValue,
                                Quantity = item.Quantity
                            }
                        );
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

        public Task UpsertItemAsync<T>(T item)
        {
            return Task.CompletedTask;
        }

        public Task DeleteItemAsync<T>(T item)
        {
            return Task.CompletedTask;
        }
    }
}
