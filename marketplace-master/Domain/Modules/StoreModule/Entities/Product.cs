using Application.Config;
using Domain.DataAnnotations;
using Domain.Events.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Interfaces;
using Domain.Modules.StoreModule.Price;
using Domain.StoreModule.Entities;
using Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace Domain.Entities.StoreModule
{
    public class Product : SoftDeletableEntity<Guid>, IWishlistItem
    {
        private IList<ProductImage> _images = new List<ProductImage>();
        private IList<ProductVariant> _variants = new List<ProductVariant>();
        private IList<PrivateFleetParameter> _privateFleetParams = new List<PrivateFleetParameter>();
        private bool? _isDisabled;
        private int? _maxInstallments;

        [ForeignKey("Category")]
        public int? Category_Id { get; set; }

        [Required]
        [ForeignKey("Store")]
        public string Store_SellerId { get; set; }

        [StringLength(100)]
        public string CustomId { get; set; }

        public string Title { get; set; }

        public ProductType ProductType { get; set; }

        public bool IsPackageType => ProductType == ProductType.Package;

        [Required]
        [StringLength(1000)]
        public string FriendlyUrl { get; set; }

        [Text]
        public string Description { get; set; }

        [Text]
        public string Specifications { get; set; }

        public bool? IsDisabled
        {
            get { return _isDisabled.GetValueOrDefault() && MaaSConfig.Current.Product.CanPauseProducts; }
            set { _isDisabled = value; }
        }

        public bool? IsBillingProduct { get; set; }

        public decimal? Price { get; private set; }

        public decimal? BasePrice { get; private set; }

        public decimal? PriceOriginal { get; private set; }

        public decimal? BasePriceOriginal { get; private set; }

        public string ProductPrice { get; private set; }

        [NotMapped]
        public ProductPrice ProductPriceMapped
        {
            get { return ProductPrice == null ? null : JsonConvert.DeserializeObject<ProductPrice>(ProductPrice); }
            set { ProductPrice = JsonConvert.SerializeObject(value); }
        }

        public string ProductPriceVersion { get; set; }

        public string CoverImageUrl { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }

        public decimal CurrentStock { get; set; }

        /// <summary>
        /// Quantidade de itens em locação
        /// </summary>
        public decimal CurrentRentalStock { get; private set; }

        [NotMapped]
        public decimal UpdatedStock { get; set; }

        public bool StockByVariant { get; set; }

        public ProductVariantType StockVariantType { get; set; }

        public IList<StockProductVariant> StockVariants { get; set; } = new List<StockProductVariant>();

        public bool DeleteWhenStockZero { get; set; }

        public bool PrivateFleetEnabled { get; set; }

        public bool SedexEnabled { get; set; }

        public bool PACEnabled { get; set; }

        public DefaultShipping DefaultShipping { get; private set; } = new DefaultShipping();

        public bool OnSitePickupEnabled { get; set; }

        public bool NoShippingEnabled { get; set; }

        public string NoShippingUrl { get; set; }

        public bool ActualNoShippingEnabled { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public RentalSettings RentalSettings { get; set; } = new RentalSettings();

        public int? MinMultiplier { get; set; }

        public int? MaxMultiplier { get; set; }

        public decimal? MultiplierPrice { get; set; }

        /// <summary>
        /// Total de unidades para produtos do tipo Pacote de créditos.
        /// </summary>
        public int? PackageCredits { get; set; }

        public ProductCategory Category { get; set; }

        public Store Store { get; set; }

        public Guid? ProductTemplateId { get; set; }

        public int? MaxInstallments
        {
            get
            {
                return MaaSConfig.Current.Product.EnableInstallmentsPerProduct ?
                    (_maxInstallments ?? MaaSConfig.Current.Payment.PaymentGateway.Installments.MaxInstallments)
                    : MaaSConfig.Current.Payment.PaymentGateway.Installments.MaxInstallments;
            }
            set { _maxInstallments = value; }
        }

        #region Policies

        public CancelOrderPolicy CancelOrderPolicy { get; set; }

        public bool IsDisallowCancelOrder => CancelOrderPolicy == CancelOrderPolicy.Disallow;
        public bool IsAllowInAdvanceCancelOrder => CancelOrderPolicy == CancelOrderPolicy.AllowInAdvance;
        public bool IsAllowCancelOrder => CancelOrderPolicy == CancelOrderPolicy.Allow;

        public ChangePeriodRentalPolicy ChangePeriodRentalPolicy { get; set; }

        public bool IsDisallowChangePeriodRental => ChangePeriodRentalPolicy == ChangePeriodRentalPolicy.Disallow;
        public bool IsAllowInAdvanceChangePeriodRental => ChangePeriodRentalPolicy == ChangePeriodRentalPolicy.AllowInAdvance;
        public bool IsAllowChangePeriodRental => ChangePeriodRentalPolicy == ChangePeriodRentalPolicy.Allow;

        #endregion Policies

        public IList<ProductImage> Images
        {
            get { return _images = _images.OrderBy(i => i.Position).ToList(); }
            set { _images = value ?? new List<ProductImage>(); }
        }

        public IList<ProductVariant> Variants
        {
            get { return _variants = _variants.OrderBy(i => i.Name).ToList(); }
            set { _variants = value ?? new List<ProductVariant>(); }
        }

        public IList<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

        public IList<RentalTransaction> RentalTransactions { get; set; } = new List<RentalTransaction>();

        public IList<PrivateFleetParameter> PrivateFleetParams
        {
            get { return _privateFleetParams = _privateFleetParams.OrderBy(i => i.DeliveryRange).ToList(); }
            set { _privateFleetParams = value ?? new List<PrivateFleetParameter>(); }
        }

        #region Availability

        public ProductAvailabilityMode ProductAvailabilityMode { get; set; }

        public bool IsAvailabilityManual => ProductAvailabilityMode == ProductAvailabilityMode.Manual;

        public IList<ProductAvailability> Availabilities { get; set; } = new List<ProductAvailability>();

        #endregion Availability

        public Product()
        {
            Id = Guid.NewGuid();
            Images = new List<ProductImage>();
            StockTransactions = new List<StockTransaction>();
            RentalTransactions = new List<RentalTransaction>();
            Variants = new List<ProductVariant>();
            StockVariants = new List<StockProductVariant>();
            PrivateFleetParams = new List<PrivateFleetParameter>();
            CreatedAt = UpdatedAt = DateTime.UtcNow;
        }

        public bool IsOutOfStock => CurrentStock <= 0M;

        public void SetPrice(decimal price)
        {
            Price = BasePrice = price;
            PriceOriginal = BasePriceOriginal = null;
        }

        public void SetPrice(ProductPrice priceModel)
        {
            ProductPriceMapped = priceModel;

            Price = priceModel.Price.Value;
            PriceOriginal = priceModel.Price.Original?.Value;

            BasePrice = priceModel.BasePrice.Value;
            BasePriceOriginal = priceModel.BasePrice.Original?.Value;

            ProductPriceVersion = priceModel.Version;

            AddDomainEvent(new ProductPriceChangedEvent(Id, Store_SellerId));
        }

        public OperationResult CheckStock(int quantity, int? variant)
        {
            if (Deleted)
                return OperationResult.Fail("", CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_DELETED_ERROR_MESSAGE"));

            if (MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
                return OperationResult.Success;

            if (!StockByVariant)
            {
                if (quantity > CurrentStock)
                    return OperationResult.Fail("",
                        CultureInfo.CurrentCulture.GetStringLocalized("CART_PRODUCT_OUT_OF_STOCK"));
            }
            else
            {
                var stockVariant = StockVariants.FirstOrDefault(i => i.Variant.Id == variant);

                if (stockVariant == null || quantity > stockVariant.CurrentStock)
                    return OperationResult.Fail("",
                        CultureInfo.CurrentCulture.GetStringLocalized("CART_PRODUCT_OUT_OF_STOCK"));
            }

            return OperationResult.Success;
        }

        public Product Delivered(OrderProduct orderProduct)
        {
            if (orderProduct.Order.IsRental)
                CurrentRentalStock += orderProduct.Quantity;

            return this;
        }

        public Product Returned(OrderProduct orderProduct)
        {
            if (orderProduct.Order.IsRental)
                CurrentRentalStock -= orderProduct.Quantity;

            return this;
        }

        public decimal? GetAdditionalPrice(int multiplierValue)
        {
            if (MultiplierPrice.HasValue)
                return MultiplierPrice.Value * multiplierValue;

            return 0;
        }

        public bool SupportsShippingMethod(ShippingMethod method)
        {
            return GetShippingMethods().Contains(method);
        }

        public IReadOnlyList<ShippingMethod> GetShippingMethods()
        {
            var methods = new List<ShippingMethod>();

            if (NoShippingEnabled)
                methods.Add(ShippingMethod.NoShipping);
            if (ActualNoShippingEnabled)
                methods.Add(ShippingMethod.ActualNoShipping);
            if (PrivateFleetEnabled)
                methods.Add(ShippingMethod.PrivateFleet);
            if (OnSitePickupEnabled)
                methods.Add(ShippingMethod.OnSitePickup);
            if (PACEnabled)
                methods.Add(ShippingMethod.PostalRegular);
            if (SedexEnabled)
                methods.Add(ShippingMethod.PostalExpress);

            return methods;
        }

        public override string ToString()
        {
            return $"{Title} {{{Id}}}";
        }
    }
}