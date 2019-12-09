using Laerdal.BestPrice.Models;
using Laerdal.BestPrice.Repository;
using System;

namespace Laerdal.BestPrice.Extensions
{
    public static class EntityFactoryExtensions
    {
        public static ContractedPriceEntity ToEntity(this ContractedPrice model, string pk, string id = null)
        {
            return new ContractedPriceEntity
            {
                Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString(),
                PartitionKey = pk,
                IsPercentageValue = model.IsPercentageValue,
                Sku = model.Sku,
                DiscountValue = model.DiscountValue,
                Quantity = model.Quantity,
                ValidFrom = model.ValidFrom > DateTime.MinValue ? model.ValidFrom : DateTime.UtcNow,
                ValidTo = model.ValidTo > DateTime.MinValue ? model.ValidTo : DateTime.UtcNow.AddYears(1)
            };
        }

        public static ContractedPrice ToModel(this ContractedPriceEntity entity)
        {
            return new ContractedPrice
            {
                IsPercentageValue = entity.IsPercentageValue,
                Sku = entity.Sku,
                DiscountValue = entity.DiscountValue,
                Quantity = entity.Quantity,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo
            };
        }

        public static ContractRuleEntity ToEntity(this ContractRule model, string pk, string id = null)
        {
            return new ContractRuleEntity
            {
                Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString(),
                PartitionKey = pk,
                AttributeName = model.AttributeName,
                AttributeValue = model.AttributeValue,
                DiscountValue = model.DiscountValue,
                Quantity = model.Quantity,
                ValidFrom = model.ValidFrom > DateTime.MinValue ? model.ValidFrom : DateTime.UtcNow,
                ValidTo = model.ValidTo > DateTime.MinValue ? model.ValidTo : DateTime.UtcNow.AddYears(1)
            };
        }

        public static ContractRule ToModel(this ContractRuleEntity entity)
        {
            return new ContractRule
            {
                AttributeName = entity.AttributeName,
                AttributeValue = entity.AttributeValue,
                DiscountValue = entity.DiscountValue,
                Quantity = entity.Quantity,
                ValidFrom = entity.ValidFrom,
                ValidTo = entity.ValidTo
            };
        }
    }
}
