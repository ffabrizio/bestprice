using BestPrice.Models;

namespace BestPrice.Repository
{
    public interface IBestPriceRepository
    {
        ContractType GetContractType(string contractTypeId);
        CustomerPrices GetCustomerPrices(string contractTypeId);
    }
}
