using BestPrice.Models;
using System.Threading.Tasks;

namespace Laerdal.BestPrice.Repository
{
    public interface ICosmosRepository
    {
        Task<ContractType> GetContractTypeAsync(string contractTypeId);
        Task<CustomerPrices> GetCustomerPricesAsync(string contractTypeId);
        Task UpsertItemAsync<T>(T item);
        Task DeleteItemAsync<T>(T item);
    }
}
