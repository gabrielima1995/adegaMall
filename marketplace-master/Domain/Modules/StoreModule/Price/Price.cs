using Domain.Entities.StoreModule;
using System;

namespace Domain.Modules.StoreModule.Price
{
    /// <summary>
    /// Preço da unidade do produto.
    /// </summary>
    public class Price
    {
        public decimal Value { get; set; }

        public Price Original { get; set; }

        public bool HasOriginal => Original != null;

        public static Price New(decimal value, decimal? original)
        {
            var price = new Price
            {
                Value = value
            };

            if (original.HasValue)
            {
                price.Original = New(original.Value, null);
            }

            return price;
        }

        public static Price Calculate(Product product, BasePrice basePrice)
        {
            var price = new Price
            {
                Value = ProductPriceSpecs.AllowsCalculateBasePrice(product)
                    ? Math.Round(basePrice.Value / product.RentalSettings.MinRentalPeriod.Value, 5)
                    : basePrice.Value
            };

            if (basePrice.HasOriginal)
            {
                price.Original = Calculate(product, basePrice.Original);
            }

            return price;
        }
    }
}