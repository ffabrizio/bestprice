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

        public async Task UpsertContractTypeAsync(ContractType item)
        {
            await _container.Scripts.ExecuteStoredProcedureAsync<object>("bulk-delete",
                partitionKey: new PartitionKey(item.ContractTypeId),
                parameters: new dynamic[] { $"SELECT * FROM c WHERE c.pk = '{item.ContractTypeId}'" });

            foreach (var rule in item.ContractRules)
            {
                var updatedItem = new ContractRuleEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    PartitionKey = item.ContractTypeId,
                    AttributeName = rule.AttributeName,
                    AttributeValue = rule.AttributeValue,
                    DiscountValue = rule.DiscountValue,
                    Quantity = rule.Quantity
                };

                await _container.CreateItemAsync(updatedItem);
            }

            AppCache.DeleteContractType(item.ContractTypeId);
        }

        public async Task DeleteContractTypeAsync(string contractTypeId)
        {
            await _container.Scripts.ExecuteStoredProcedureAsync<object>("bulk-delete",
                partitionKey: new PartitionKey(contractTypeId),
                parameters: new dynamic[] { $"SELECT * FROM c WHERE c.pk = '{contractTypeId}'" });

            AppCache.DeleteContractType(contractTypeId);
        }

        public async Task UpsertCustomerPricesAsync(CustomerPrices item)
        {
            await _container.Scripts.ExecuteStoredProcedureAsync<object>("bulk-delete",
                partitionKey: new PartitionKey(item.CustomerNumber),
                parameters: new dynamic[] { $"SELECT * FROM c WHERE c.pk = '{item.CustomerNumber}'" });

            foreach (var price in item.ContractedPrices)
            {
                var updatedItem = new ContractedPriceEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    PartitionKey = item.CustomerNumber,
                    IsPercentageValue = price.IsPercentageValue,
                    Sku = price.Sku,
                    DiscountValue = price.DiscountValue,
                    Quantity = price.Quantity
                };

                await _container.CreateItemAsync(updatedItem);
            }

            AppCache.DeleteCustomerPrices(item.CustomerNumber);
        }

        public async Task DeleteCustomerPricesAsync(string customerNumber)
        {
            await _container.Scripts.ExecuteStoredProcedureAsync<object>("bulk-delete",
                partitionKey: new PartitionKey(customerNumber),
                parameters: new dynamic[] { $"SELECT * FROM c WHERE c.pk = '{customerNumber}'" });

            AppCache.DeleteCustomerPrices(customerNumber);
        }
    }
}
