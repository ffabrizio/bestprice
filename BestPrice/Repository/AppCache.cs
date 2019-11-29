using BestPrice.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace BestPrice.Repository
{
    public static class AppCache
    {
        private static ObservableCollection<ContractType> ContractTypes { get; set; } = new ObservableCollection<ContractType>();
        private static ObservableCollection<CustomerPrices> CustomerPrices { get; set; } = new ObservableCollection<CustomerPrices>();
        private static ObservableCollection<string> NullKeys { get; set; } = new ObservableCollection<string>();

        public static void AddNullKey(string key)
        {
            NullKeys.Add(key);
        }
        public static bool IsNull(string key)
        {
            return NullKeys.Contains(key);
        }

        public static ContractType GetContractType(string contractTypeId)
        {
            return ContractTypes.FirstOrDefault(_ => _.ContractTypeId == contractTypeId);
        }

        public static void SetContractType(ContractType contractType)
        {
            var existingContractType = GetContractType(contractType.ContractTypeId);
            if (existingContractType != null)
            {
                ContractTypes.Remove(existingContractType);
            }

            ContractTypes.Add(contractType);
        }

        public static CustomerPrices GetCustomerPrices(string customerNumber)
        {
            return CustomerPrices.FirstOrDefault(_ => _.CustomerNumber == customerNumber);
        }

        public static void SetCustomerPrices(CustomerPrices customerPrices)
        { 
            var existingCustomerPrices = GetCustomerPrices(customerPrices.CustomerNumber);
            if (existingCustomerPrices != null)
            {
                CustomerPrices.Remove(existingCustomerPrices);
            }

            CustomerPrices.Add(customerPrices);
        }

        
    }
}
