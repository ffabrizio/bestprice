using Laerdal.BestPrice.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Laerdal.BestPrice.Repository
{
    public static class AppCache
    {
        private static ObservableCollection<ContractType> ContractTypes { get; set; } = new ObservableCollection<ContractType>();
        private static ObservableCollection<CustomerPrices> CustomerPrices { get; set; } = new ObservableCollection<CustomerPrices>();

        public static ContractType GetContractType(string contractTypeId)
        {
            return ContractTypes.FirstOrDefault(_ => _.ContractTypeId == contractTypeId);
        }

        public static void SetContractType(ContractType contractType)
        {
            DeleteContractType(contractType.ContractTypeId);
            ContractTypes.Add(contractType);
        }

        public static void DeleteContractType(string contractTypeId)
        {
            var existingContractType = GetContractType(contractTypeId);
            if (existingContractType != null)
            {
                ContractTypes.Remove(existingContractType);
            }
        }

        public static CustomerPrices GetCustomerPrices(string customerNumber)
        {
            return CustomerPrices.FirstOrDefault(_ => _.CustomerNumber == customerNumber);
        }

        public static void SetCustomerPrices(CustomerPrices customerPrices)
        {
            DeleteCustomerPrices(customerPrices.CustomerNumber);
            CustomerPrices.Add(customerPrices);
        }

        public static void DeleteCustomerPrices(string customerNumber)
        {
            var existingCustomerPrices = GetCustomerPrices(customerNumber);
            if (existingCustomerPrices != null)
            {
                CustomerPrices.Remove(existingCustomerPrices);
            }
        }

    }
}
