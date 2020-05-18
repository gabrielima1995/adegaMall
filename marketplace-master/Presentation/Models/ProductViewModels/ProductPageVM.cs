using Application.Config;
using Application.Modules.Products;
using Domain.DTO;
using Domain.Entities.StoreModule;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Models
{
    public class ProductPageVM : ProductPageViewModel
    {
        public ProductPaymentOptions PaymentOptions { get; set; }
        public bool IsOutOfStock => Product.CurrentStock <= 0;
        public double SellerRatingAverage { get; set; }
        public int SellerRatingsCount { get; set; }
        public bool ShowAnyShipping => !Product.NoShippingEnabled || 
            !Product.ActualNoShippingEnabled || 
            !Product.PrivateFleetEnabled || 
            !Product.OnSitePickupEnabled ||
            !Product.SedexEnabled ||
            !Product.PACEnabled;

        public List<ProductVariantType> VariantTypes
        {
            get
            {
                return Product.Variants
                    .Select(x => x.Type).Distinct()
                    .OrderBy(x => x.Order).ThenBy(x => x.Name)
                    .ToList();
            }
        }

        public List<ProductVariant> GetVariants(ProductVariantType type)
        {
            return Product.Variants
                .Where(x => x.Type.Id == type.Id)
                .OrderBy(x => x.Order).ThenBy(x => x.Name)
                .ToList();
        }

        private List<StockProductVariant> stockVariants;

        public IReadOnlyList<StockProductVariant> GetStockVariants()
        {
            if (stockVariants == null)
                stockVariants = Product.StockVariants
                    .OrderBy(x => x.Variant.Order).ThenBy(x => x.Variant.Name)
                    .ToList();

            return stockVariants;
        }

        public bool NeedDropDownListForVariants => GetStockVariants().Count() >= MaaSConfig.Current.Product.Variants.enableForNItemsOrHigher;

        public bool VariantIsChecked(StockProductVariant variant)
        {
            var checkedVariant = GetStockVariants().FirstOrDefault(v => v.HasStock);

            return checkedVariant?.Id == variant.Id;
        }

        public ProductPageVM(ProductDetailsForPublicPage productDetails)
            : base(productDetails.Product)
        {
            PaymentOptions = productDetails.PaymentOptions;
        }
    }
}