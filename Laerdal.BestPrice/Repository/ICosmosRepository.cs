using Laerdal.BestPrice.Models;
using System.Threading.Tasks;

namespace Laerdal.BestPrice.Repository
{
    public interface ICosmosRepository
    {
        Task<ContractType> GetContractTypeAsync(string contractTypeId);
        Task<CustomerPrices> GetCustomerPricesAsync(string contractTypeId);
        Task UpsertContractTypeAsync(ContractType item);
        Task DeleteContractTypeAsync(string contractTypeId);
        Task UpsertCustomerPricesAsync(CustomerPrices item);
        Task DeleteCustomerPricesAsync(string customerNumber);
    }
}
