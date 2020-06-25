using Application.Config;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Price;
using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Products
{
    public class ProductPageViewModel : IProductDetailsToSelectPeriodViewModel, IWithCustomFields
    {
        public bool IsFixedRental => Product.RentalSettings.IsFixedRental;

        public Product Product { get; set; }

        public ProductPrice PriceModel => Product.ProductPriceMapped;

        public RentalSettings RentalSettings => Product.RentalSettings;

        public RentalPeriodUnit? RentalPeriodUnit { get; set; }

        public IList<ProductAvailability> RecurringAvailabilities { get; private set; }

        public RentalPeriodUnit? MinRentalNoticeUnit { get; set; }

        public string RentalPeriodLabel => RentalSettings?.GetMinRentalPeriodUnitLabel(false);

        public string MinRentalNoticeLabel => RentalSettings?.GetMinRentalNoticeUnitLabel(false);

        public bool IsPackageProduct { get; }

        public IList<Product> SellerProducts { get; set; }

        public IList<Product> RelatedProducts { get; set; }

        public int? Multiplier { get; set; }

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Product;

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }

        public Dictionary<string, string> CustomFields { get; set; }

        public CartItemCustomFields CartItemCustomFields { get; set; } = new CartItemCustomFields();

        public Guid Id { get => Product.Id; set { } }

        public bool RentalEnabled =>
            MaaSConfig.Current.Product.BusinessModel.Rental.Enabled;

        public bool ProductAvailabilityEnabled =>
            RentalEnabled &&
            Product.IsAvailabilityManual &&
            MaaSConfig.Current.Product.BusinessModel.Rental.ProductAvailability.Enabled;

        public bool RecurringProductAvailabilityEnabled =>
            ProductAvailabilityEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled;

        public bool ShowAddToCart => IsPackageProduct &&
                                     !(RecurringProductAvailabilityEnabled &&
                                       RecurringAvailabilities != null && RecurringAvailabilities.Count > 0);

        public ProductPageViewModel(Product product)
        {
            Product = product;

            IsPackageProduct = product.IsPackageType;

            SellerProducts = new List<Product>();
            RelatedProducts = new List<Product>();

            RentalPeriodUnit = product.RentalSettings?.RentalPeriodUnit;

            MinRentalNoticeUnit = product.RentalSettings?.MinRentalNoticeUnit
                ?? MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodUnit;

            if (RecurringProductAvailabilityEnabled)
            {
                RecurringAvailabilities = product.Availabilities
                    .Where(i => i.IsRecurring && !i.Deleted)
                    .OrderBy(i => (int)i.DayOfWeek)
                    .ThenBy(i => i.StartTime)
                    .ToList();
            }
        }
    }
    public class CartItemCustomFields : IWithCustomFields
    {
        public CartItemCustomFields() { }
        public CustomFieldEntityType EntityType => CustomFieldEntityType.CartItem;
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public Guid Id { get; set; }
    }
}