using Domain.Entities.StoreModule;

namespace Domain.Modules.StoreModule.Price
{
    /// <summary>
    /// Preço base do produto.
    /// </summary>
    public class BasePrice
    {
        public decimal Value { get; set; }

        public BasePrice Original { get; set; }

        public bool HasOriginal => Original != null;

        public static BasePrice New(decimal value, decimal? original)
        {
            var basePrice = new BasePrice
            {
                Value = value
            };

            if (original.HasValue)
            {
                basePrice.Original = New(original.Value, null);
            }

            return basePrice;
        }

        public static BasePrice Calculate(Product product, Price price)
        {
            var basePrice = new BasePrice
            {
                Value = ProductPriceSpecs.AllowsCalculateBasePrice(product)
                    ? price.Value * product.RentalSettings.MinRentalPeriod.Value
                    : price.Value
            };

            if (price.HasOriginal)
            {
                basePrice.Original = Calculate(product, price.Original);
            }

            return basePrice;
        }
    }
}