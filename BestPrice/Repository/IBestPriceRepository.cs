using BestPrice.Models;
using System.Threading.Tasks;

namespace BestPrice.Repository
{
    public interface IBestPriceRepository
    {
        Task<ContractType> GetContractType(string contractTypeId);
        Task<CustomerPrices> GetCustomerPrices(string contractTypeId);
    }
}
