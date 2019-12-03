using Laerdal.BestPrice.Models;
using System.Threading.Tasks;

namespace Laerdal.BestPrice.Repository
{
    public interface ICosmosRepository
    {
        Task<ContractType> GetContractTypeAsync(string contractTypeId);
        Task<CustomerPrices> GetCustomerPricesAsync(string contractTypeId);
        Task<BatchOutput> UpsertContractTypeAsync(ContractType item);
        Task<BatchOutput> DeleteContractTypeAsync(string contractTypeId);
        Task<BatchOutput> UpsertCustomerPricesAsync(CustomerPrices item);
        Task<BatchOutput> DeleteCustomerPricesAsync(string customerNumber);
    }
}
