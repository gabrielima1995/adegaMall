using Application.Config;
using Application.Modules.Products.Variants;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Events.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Price;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using Framework.Helpers;
using Infra.Crosscutting.Globalization;
using Infra.Crosscutting.Globalization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Application.Modules.Products
{
    public class ProductEditorViewModel : IWithProductCategory, IWithCustomFields
    {
        private const int minMultiplierDefault = 0;
        private const int maxMultiplierDefault = 9999;
        private const decimal multiplierPriceDefault = 0m;

        private IList<ProductImage> _images;

        public Guid Id { get; set; }

        public bool IsNew { get; private set; } = true;

        public string SellerId { get; set; }

        public bool DisableProduct { get; set; }

        [DisplayNameLocalized("PRODUCT_MAX_INSTALLMENTS_LABEL")]
        public int? MaxInstallments { get; set; }

        [DisplayNameLocalized("PRODUCT_CUSTOM_ID_LABEL")]
        public string CustomId { get; set; }

        [DisplayNameLocalized("PRODUCT_TITLE_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Title { get; set; }

        public bool TitleIsEditable { get; set; }

        [DisplayNameLocalized("PRODUCT_TYPE_LABEL")]
        public ProductType ProductType { get; set; }

        public bool IsPackageType => ProductType == ProductType.Package;

        public bool ShowProductType =>
            RentalEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.Package.Enabled;

        [DisplayNameLocalized("PRODUCT_DESCRIPTION_LABEL")]
        [RequiredLocalized]
        public string Description { get; set; }

        public bool DescriptionIsEditable { get; set; }

        [DisplayNameLocalized("PRODUCT_SPECIFICATIONS_LABEL")]
        [RequiredIfLocalized("SpecificationsRequired")]
        public string Specifications { get; set; }

        public bool SpecificationsIsEditable { get; set; }

        public bool SpecificationsRequired => MaaSConfig.Current.Product.Specifications.IsRequired;

        [DisplayNameLocalized("PRODUCT_PRICE_LABEL")]
        [RequiredIfLocalized("PriceRequired")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Price { get; set; }

        [DisplayNameLocalized("PRODUCT_DISCOUNT_PRICE_LABEL")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? DiscountPrice { get; set; }

        public bool PriceRequired =>
            MaaSConfig.Current.Product.Price.IsRequired &&
            !ShowByBasePrice;

        [DisplayNameLocalized("PRODUCT_BASEPRICE_LABEL")]
        [RequiredIfLocalized("BasePriceRequired")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? BasePrice { get; set; }

        [DisplayNameLocalized("PRODUCT_DISCOUNT_BASEPRICE_LABEL")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? DiscountBasePrice { get; set; }

        public bool ShowByBasePrice =>
            ShowFixedPeriodRental &&
            MaaSConfig.Current.Product.BusinessModel.Rental.FixedPeriodRental.RegisterByBasePrice;

        public bool BasePriceRequired =>
            MaaSConfig.Current.Product.Price.IsRequired &&
            ShowByBasePrice;

        public bool ShowDiscountPrice =>
            !ProductAvailabilityEnabled ||
            IsAvailabilityAuto;

        [DisplayNameLocalized("PRODUCT_CATEGORIES_SELECT")]
        [RequiredIfLocalized("CategoryRequired")]
        public int? Category { get; set; }

        public bool CategoryIsEditable { get; set; }

        public bool CategoryRequired => MaaSConfig.Current.Product.Category.IsRequired;

        public IList<ProductCategory> Categories { get; set; }

        #region Shipping

        [DisplayNameLocalized("PRODUCT_PRIVATE_FLEET_METHOD_LABEL")]
        public bool PrivateFleetEnabled { get; set; }

        public List<PrivateFleetParameterViewModel> PrivateFleetParams { get; private set; } = new List<PrivateFleetParameterViewModel>();

        [DisplayNameLocalized("PRODUCT_SEDEX_METHOD_LABEL")]
        public bool SedexEnabled { get; set; }

        [DisplayNameLocalized("PRODUCT_PAC_METHOD_LABEL")]
        public bool PACEnabled { get; set; }

        [DisplayNameLocalized("PRODUCT_ON_SITE_PICKUP_METHOD_LABEL")]
        public bool OnSitePickupEnabled { get; set; }

        [DisplayNameLocalized("PRODUCT_ACTUAL_NO_SHIPPING_METHOD_LABEL")]
        public bool ActualNoShippingEnabled { get; set; }

        [DisplayNameLocalized("PRODUCT_NO_SHIPPING_METHOD_LABEL")]
        public bool NoShippingEnabled { get; set; }

        [DisplayNameLocalized("PRODUCT_NO_SHIPPING_URL_LABEL")]
        public string NoShippingUrl { get; set; }

        public ProductEditorShippingType ShippingType { get; set; }

        [DisplayNameLocalized("PRODUCT_DEFAULT_SHIPPING_AMOUNT_LABEL")]
        public decimal? DefaultShippingAmount { get; set; }

        [DisplayNameLocalized("PRODUCT_DEFAULT_SHIPPING_DELIVERY_DAYS_LABEL")]
        public int? DefaultShippingDeliveryDays { get; set; }

        #endregion Shipping

        #region Stock Management

        [DisplayNameLocalized("PRODUCT_STOCK_CURRENT_LABEL")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? CurrentStock { get; set; }

        [DisplayNameLocalized("PRODUCT_STOCK_UPDATED_LABEL")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? UpdatedStock { get; set; }

        [DisplayNameLocalized("PRODUCT_RENTAL_STOCK_LABEL")]
        public decimal? CurrentRentalStock { get; set; }

        [DisplayNameLocalized("PRODUCT_DELETE_WHEN_STOCK_ZERO_LABEL")]
        public bool DeleteWhenStockZero { get; set; }

        public bool StockByVariant { get; set; }

        [DisplayNameLocalized("PRODUCT_STOCK_VARIANT_TYPE_LABEL")]
        [RequiredIfLocalized("StockVariantTypeRequired")]
        public int? StockVariantType { get; set; }

        public bool StockVariantTypeRequired => MaaSConfig.Current.Product.Variants.StockByVariantEnabled && StockByVariant;

        public IList<ProductVariantType> StockVariantTypes { get; set; }

        public IList<ProductStockVariantItemViewModel> StockVariants { get; set; }

        #endregion Stock Management

        public bool DimensionsIsEditable { get; set; }

        [DisplayNameLocalized("PRODUCT_LENGTH_LABEL")]
        [RequiredIfLocalized("LengthRequired")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Length { get; set; }

        public bool LengthRequired => MaaSConfig.Current.Product.Length.IsRequired;

        [DisplayNameLocalized("PRODUCT_WIDTH_LABEL")]
        [RequiredIfLocalized("WidthRequired")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Width { get; set; }

        public bool WidthRequired => MaaSConfig.Current.Product.Width.IsRequired;

        [DisplayNameLocalized("PRODUCT_HEIGHT_LABEL")]
        [RequiredIfLocalized("HeightRequired")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Height { get; set; }

        public bool HeightRequired => MaaSConfig.Current.Product.Height.IsRequired;

        [DisplayNameLocalized("PRODUCT_WEIGHT_LABEL")]
        [RequiredIfLocalized("WeightRequired")]
        [DisplayFormat(DataFormatString = "{0:F3}", ApplyFormatInEditMode = true)]
        public decimal? Weight { get; set; }

        public bool WeightRequired => MaaSConfig.Current.Product.Weight.IsRequired;

        [DisplayNameLocalized("PRODUCT_NEW_IMAGES_LABEL")]
        public IList<ProductImage> Images
        {
            get { return _images ?? (_images = new List<ProductImage>()); }
            set { _images = value; }
        }

        public bool ImagesIsEditable { get; set; }

        public IList<ProductVariantTypeAndItemSelector> Variants { get; set; }

        public bool VariantsIsEditable { get; set; }

        #region Rental

        public bool RentalEnabled =>
            MaaSConfig.Current.Product.BusinessModel.Rental.Enabled;

        public bool GridViewEnabled =>
            RentalEnabled &&
            (ProductRentalPeriodUnit == RentalPeriodUnit.Hour || ProductRentalPeriodUnit == RentalPeriodUnit.Minute) &&
            MaaSConfig.Current.Product.BusinessModel.Rental.GridView.Enabled;

        public bool RentalPeriodModeUnrestricted =>
            RentalEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Unrestricted;

        public bool RentalPeriodModeShifts =>
            RentalEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts;

        public bool RentalFieldsRequired => RentalEnabled;

        [DisplayNameLocalized("PRODUCT_MIN_RENTAL_PERIOD")]
        [RequiredIfLocalized("MinMaxPeriodRequired")]
        public int? MinRentalPeriod { get; set; }

        [DisplayNameLocalized("PRODUCT_MAX_RENTAL_PERIOD")]
        [RequiredIfLocalized("MinMaxPeriodRequired")]
        public int? MaxRentalPeriod { get; set; }

        public bool ShowMinMaxPeriodRental =>
            RentalPeriodModeUnrestricted &&
            !(MaaSConfig.Current.Product.BusinessModel.Rental.FixedPeriodRental.Enabled &&
              MaaSConfig.Current.Product.BusinessModel.Rental.HideMinMaxPeriod);

        public bool MinMaxPeriodRequired =>
            RentalFieldsRequired &&
            ShowMinMaxPeriodRental;

        public bool FixedPeriodRentalDisabled =>
            MaaSConfig.Current.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.Enabled ||
            IsAvailabilityManual;

        [DisplayNameLocalized("PRODUCT_MIN_NOTICE_PERIOD")]
        public int? MinRentalNotice { get; set; }

        [RequiredIfLocalized("FixedRentalRequired")]
        [DisplayNameLocalized("PRODUCT_FIXED_RENTAL_PERIOD")]
        public int? RentalFixedPeriod { get; set; }

        [DisplayNameLocalized("PRODUCT_BLOCK_SCHEDULE")]
        public bool BlockScheduleForProduct { get; set; } = false;

        public bool ShowFixedPeriodRental =>
            RentalPeriodModeUnrestricted &&
            MaaSConfig.Current.Product.BusinessModel.Rental.FixedPeriodRental.Enabled;

        public bool FixedRentalRequired =>
            RentalFieldsRequired &&
            ShowFixedPeriodRental;

        [DisplayNameLocalized("RENTAL_MARGIN_LABEL")]
        public int? RentalMargin { get; set; }

        [RequiredIfLocalized("RentalFieldsRequired")]
        [DisplayNameLocalized("RENTAL_PERIOD_UNIT_LABEL")]
        public RentalPeriodUnit? ProductRentalPeriodUnit { get; set; }

        public RentalPeriodUnit? MinRentalNoticeUnit { get; set; }

        public RentalPeriodUnit? RentalMarginUnit { get; set; }

        [DisplayNameLocalized("PRODUCT_AVAILABILITY_WARNING_LABEL")]
        [MaxLength(499)]
        public string AvailabilityWarning { get; set; }

        [DisplayNameLocalized("PRODUCT_CHARGE_DOUBLE_SHIPPING_LABEL")]
        public bool ChargeDoubleShipping { get; set; }

        [DisplayNameLocalized("MIN_MULTIPLIER_LABEL")]
        public int? MinMultiplier { get; set; }

        [DisplayNameLocalized("MAX_MULTIPLIER_LABEL")]
        public int? MaxMultiplier { get; set; }

        [DisplayNameLocalized("MULTIPLIER_PRICE_LABEL")]
        public decimal? MultiplierPrice { get; set; }

        [DisplayNameLocalized("PRODUCT_PRICE_MULTIPLIER_LOGIC_LABEL")]
        public ProductPriceMultiplierOption MultiplierOption { get; set; }

        public string RentalPeriodLabel
        {
            get
            {
                switch (ProductRentalPeriodUnit)
                {
                    case RentalPeriodUnit.Minute:
                        return "RENTAL_PERIOD_MINUTES_LABEL";
                    case RentalPeriodUnit.Hour:
                        return "RENTAL_PERIOD_HOURS_LABEL";
                    case RentalPeriodUnit.Day:
                        return "RENTAL_PERIOD_DAYS_LABEL";
                    case RentalPeriodUnit.Month:
                        return "RENTAL_PERIOD_MONTHS_LABEL";
                }

                return null;
            }
        }

        [DisplayNameLocalized("PRODUCT_MIN_RENTAL_AMOUNT")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? MinRentalAmount { get; set; }

        public bool ShowMinRentalAmount =>
            RentalEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.MinRentalAmount.Enabled &&
            ProductType == ProductType.Default;

        [EnsureMinimumElementsIfLocalized("RequireRentalShifts", "PRODUCT_RENTAL_SHIFTS_REQUIRED")]
        public List<Guid> RentalShiftIds { get; set; } = new List<Guid>();

        public IReadOnlyCollection<RentalShiftViewModel> RentalShiftOptions { get; set; } = new List<RentalShiftViewModel>();

        public bool RequireRentalShifts => MaaSConfig.Current.Product.BusinessModel.Rental.Enabled && MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts;

        [DisplayNameLocalized("PRODUCT_PACKAGE_CREDITS_LABEL")]
        [RequiredIfLocalized("PackageCreditsRequired")]
        [RangeLocalized(1, 50, "PRODUCT_PACKAGE_CREDITS_ERROR")]
        public int? PackageCredits { get; set; }

        public bool ShowPackageCredits =>
            RentalEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.Package.Enabled &&
            IsPackageType;

        public bool PackageCreditsRequired =>
            RentalFieldsRequired &&
            ShowPackageCredits;

        #endregion Rental

        #region Policies

        public bool CancelOrderPolicyEnabled =>
            RentalEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.Cancel.Enabled;

        public bool ChangePeriodRentalPolicyEnabled =>
            RentalEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.ChangePeriod.Enabled;

        public bool AnyPolicyEnabled =>
            CancelOrderPolicyEnabled || ChangePeriodRentalPolicyEnabled;

        [DisplayNameLocalized("CANCEL_ORDER_POLICY_LABEL")]
        public CancelOrderPolicy CancelOrderPolicy { get; set; }

        [DisplayNameLocalized("CHANGE_PERIOD_RENTAL_POLICY_LABEL")]
        public ChangePeriodRentalPolicy ChangePeriodRentalPolicy { get; set; }

        #endregion Policies

        #region Availability

        public ProductAvailabilityMode AvailabilityMode { get; set; }

        public bool IsAvailabilityAuto => AvailabilityMode == ProductAvailabilityMode.Auto;
        public bool IsAvailabilityManual => AvailabilityMode == ProductAvailabilityMode.Manual;

        public IList<ProductAvailability> Availabilities { get; set; }

        public bool ProductAvailabilityEnabled =>
            GridViewEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.ProductAvailability.Enabled;

        public bool RecurringProductAvailabilityEnabled =>
            ProductAvailabilityEnabled &&
            MaaSConfig.Current.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled;

        public bool ShowNewAvailability =>
            ProductAvailabilityEnabled &&
            (Availabilities.Count == 0 || Availabilities.All(i => !i.IsRecurring));

        public bool ShowNewRecurringAvailability =>
            RecurringProductAvailabilityEnabled &&
            IsPackageType &&
            (Availabilities.Count == 0 || Availabilities.All(i => i.IsRecurring));

        public bool ShowAvailabilities =>
            (ProductAvailabilityEnabled || RecurringProductAvailabilityEnabled) &&
            Availabilities.Any();

        #endregion Availability

        public string PublicPageUrl => $"{MaaSConfig.Current.Environment.AppUrl}product/{FriendlyUrl}";

        public string FriendlyUrl { get; set; }

        #region Custom fields

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Product;
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }

        #endregion Custom fields

        public Guid? TemplateId { get; set; }

        #region Integration to Google Shopping

        [DisplayNameLocalized("Integra com Google Shopping?")]
        public bool MustBeIntegrate { get; set; } = false;

        [DisplayNameLocalized("Último envio: ")]
        public DateTime? LastIntegration { get; set; }

        #endregion

        public ProductEditorViewModel()
        {
            Categories = new List<ProductCategory>();
            Variants = new List<ProductVariantTypeAndItemSelector>();
            StockVariants = new List<ProductStockVariantItemViewModel>();
            Availabilities = new List<ProductAvailability>();

            if (MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
                ProductRentalPeriodUnit = MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodUnit;
        }

        public ProductEditorViewModel(IList<ProductCategory> allCategories)
            : this()
        {
            Categories = allCategories;
            //PrivateFleetEnabled = MaaSConfig.Current.Product.PrivateFleetShipping.Enabled;
            //PrivateFleetParams = MaaSConfig.Current.Product.PrivateFleetShipping.DefaultParams
            //    .Select(i => new PrivateFleetParameterViewModel(i))
            //    .ToList();
        }

        public ProductEditorViewModel(Product product, IList<ProductCategory> allCategories)
            : this(allCategories)
        {
            Id = product.Id;
            IsNew = false;
            CustomId = product.CustomId;
            Title = product.Title;
            ProductType = product.ProductType;
            Description = product.Description;
            Specifications = product.Specifications;
            Category = product.Category?.Id;
            DisableProduct = product.IsDisabled.GetValueOrDefault();
            MaxInstallments = product.MaxInstallments;

            var hasDiscount = product.PriceOriginal.HasValue && product.Price.HasValue;
            Price = hasDiscount ? product.PriceOriginal : product.Price;
            DiscountPrice = hasDiscount ? product.Price : null;
            BasePrice = hasDiscount ? product.BasePriceOriginal : product.BasePrice;
            DiscountBasePrice = hasDiscount ? product.BasePrice : null;

            #region Shipping

            NoShippingEnabled = false;
            NoShippingUrl = null;

            ActualNoShippingEnabled = false;
            OnSitePickupEnabled = false;
            
            SedexEnabled = false;
            PACEnabled = false;
            DefaultShippingAmount = 0;
            DefaultShippingDeliveryDays = 0;

            PrivateFleetEnabled = false;
            PrivateFleetParams.Clear();

            Length = null;
            Width = null;
            Height = null;
            Weight = null;

            if (product.NoShippingEnabled)
            {
                ShippingType = ProductEditorShippingType.Digital;
                NoShippingEnabled = true;
                NoShippingUrl = product.NoShippingUrl;
            }
            else
            {
                ShippingType = ProductEditorShippingType.Physical;

                ActualNoShippingEnabled = product.ActualNoShippingEnabled;
                OnSitePickupEnabled = product.OnSitePickupEnabled;
                
                SedexEnabled = product.SedexEnabled;
                PACEnabled = product.PACEnabled;
                DefaultShippingAmount = product.DefaultShipping.Amount;
                DefaultShippingDeliveryDays = product.DefaultShipping.DeliveryDays;

                PrivateFleetEnabled = product.PrivateFleetEnabled && product.PrivateFleetParams.Any();
                if (PrivateFleetEnabled) PrivateFleetParams = product.PrivateFleetParams.Select(param => new PrivateFleetParameterViewModel(param)).ToList();

                Length = product.Length;
                Width = product.Width;
                Height = product.Height;
                Weight = product.Weight;
            }

            #endregion Shipping

            #region Stock management

            CurrentStock = product.CurrentStock;
            UpdatedStock = product.UpdatedStock;
            DeleteWhenStockZero = product.DeleteWhenStockZero;
            CurrentRentalStock = product.CurrentRentalStock;

            StockByVariant = product.StockByVariant;
            StockVariantType = product.StockVariantType?.Id;

            #endregion Stock management

            #region Rental

            MinRentalPeriod = product.RentalSettings.MinRentalPeriod;
            MaxRentalPeriod = product.RentalSettings.MaxRentalPeriod;
            MinRentalNotice = product.RentalSettings.MinRentalNotice;
            MinRentalNoticeUnit = product.RentalSettings.MinRentalNoticeUnit;
            RentalMargin = product.RentalSettings.MinRentalMargin;
            RentalMarginUnit = product.RentalSettings.RentalMarginUnit ?? RentalPeriodUnit.Hour;
            AvailabilityWarning = product.RentalSettings.AvailabilityWarning;
            ChargeDoubleShipping = product.RentalSettings.ChargeDoubleShipping ?? false;
            ProductRentalPeriodUnit = product.RentalSettings.RentalPeriodUnit ?? MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodUnit;

            MinMultiplier = product.MinMultiplier;
            MaxMultiplier = product.MaxMultiplier;
            MultiplierPrice = product.MultiplierPrice;
            PackageCredits = product.PackageCredits;

            if (product.RentalSettings.IsFixedRental && MaaSConfig.Current.Product.BusinessModel.Rental.FixedPeriodRental.Enabled)
                RentalFixedPeriod = product.RentalSettings.MinRentalPeriod;

            BlockScheduleForProduct = product.RentalSettings.BlockScheduleForProduct;
            MinRentalAmount = product.RentalSettings.MinRentalAmount;

            #endregion Rental

            Images = product.Images ?? new List<ProductImage>(0);
            FriendlyUrl = product.FriendlyUrl;

            if (MaaSConfig.Current.Product.BusinessModel.Rental.EnablePriceMultiplier && MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
            {
                if ((MinMultiplier.HasValue && MinMultiplier != minMultiplierDefault) ||
                    (MaxMultiplier.HasValue && MaxMultiplier != maxMultiplierDefault) ||
                    (MultiplierPrice.HasValue && MultiplierPrice != multiplierPriceDefault))
                {
                    MultiplierOption = ProductPriceMultiplierOption.Enabled;
                }
            }

            #region Policies

            CancelOrderPolicy = product.CancelOrderPolicy;
            ChangePeriodRentalPolicy = product.ChangePeriodRentalPolicy;

            #endregion Policies

            #region Availability

            AvailabilityMode = product.ProductAvailabilityMode;

            Availabilities = product.Availabilities
                .Where(i => !i.Deleted)
                .OrderBy(i => (int)i.DayOfWeek)
                .ThenBy(i => i.StartTime)
                .ToList();

            #endregion Availability

            MustBeIntegrate = product.MustBeIntegrate;
            LastIntegration = product.LastIntegration;
        }

        private void SetCoverImageUrl(Product product)
        {
            product.CoverImageUrl = product.Images?.FirstOrDefault()?.Url;
        }

        private void CalculatePrice(Product target, IList<RentalShiftDto> shifts)
        {
            var price = !ShowByBasePrice ? DiscountPrice ?? Price : DiscountBasePrice ?? BasePrice;
            var originalPrice = !ShowByBasePrice ? Price : BasePrice;

            if (price != null && originalPrice != null)
            {
                if (originalPrice == price)
                    originalPrice = null;

                else if (originalPrice < price)
                {
                    var aux = price;
                    price = originalPrice;
                    originalPrice = aux;
                }
            }

            var productPrice = !ShowByBasePrice
                ? ProductPrice.ByPrice(target, price.GetValueOrDefault(), originalPrice, shifts)
                : ProductPrice.ByBasePrice(target, price.GetValueOrDefault(), originalPrice, shifts);

            target.SetPrice(productPrice);
        }

        public Product Create(Domain.Entities.StoreModule.Store store, ProductCategory category, IList<ProductVariant> variants, ProductVariantType stockVariantType, IList<RentalShiftDto> shifts)
        {
            var entity = new Product();

            entity.CustomId = CustomId;
            entity.Title = Title;
            entity.ProductType = ProductType;
            entity.Description = Description;
            entity.Specifications = Specifications;
            entity.Category = category;
            
            entity.Store = store;
            entity.Store_SellerId = store.Seller?.Id ?? store.SellerId;
            entity.Variants = variants;
            entity.FriendlyUrl = FriendlyUrl;
            entity.IsDisabled = DisableProduct;
            entity.MaxInstallments = MaxInstallments ?? 3;
            entity.ProductTemplateId = TemplateId;

            if (Id != null && Id != Guid.Empty)
                entity.Id = Id;

            entity.MustBeIntegrate = MustBeIntegrate;

            #region Stock management

            entity.StockByVariant = StockByVariant;

            if (StockByVariant)
            {
                entity.StockVariantType = stockVariantType;

                UpdatedStock = 0;

                StockVariants = StockVariants?
                    .Where(i => i.StockVariant.HasValue && i.UpdatedStock.HasValue)
                    .DistinctBy(i => i.StockVariant)
                    .ToList();

                StockVariants?.ForEach(sv =>
                {
                    entity.StockVariants.Add(sv.Create(variants));

                    UpdatedStock += (sv.UpdatedStock ?? sv.CurrentStock);
                });
            }
            else
            {
                entity.StockVariantType = null;

                StockVariants?.Clear();

                entity.StockVariants.Clear();
            }

            entity.CurrentStock = CurrentStock ?? 0;
            entity.UpdatedStock = UpdatedStock ?? entity.CurrentStock;
            entity.DeleteWhenStockZero = DeleteWhenStockZero;

            #endregion Stock management

            MapShippingToEntity(entity);

            #region Rental

            entity.RentalSettings.MinRentalPeriod = MinRentalPeriod;
            entity.RentalSettings.MaxRentalPeriod = MaxRentalPeriod;

            if (RentalFixedPeriod.HasValue)
            {
                entity.RentalSettings.MinRentalPeriod = RentalFixedPeriod.Value;
                entity.RentalSettings.MaxRentalPeriod = RentalFixedPeriod.Value;
            }

            entity.RentalSettings.BlockScheduleForProduct = BlockScheduleForProduct;
            entity.RentalSettings.MinRentalNotice = MinRentalNotice;
            entity.RentalSettings.MinRentalNoticeUnit = MinRentalNoticeUnit;
            entity.RentalSettings.ChargeDoubleShipping = ChargeDoubleShipping;
            entity.RentalSettings.AvailabilityWarning = AvailabilityWarning;
            entity.RentalSettings.RentalPeriodUnit = ProductRentalPeriodUnit;

            entity.RentalSettings.MinRentalMargin = RentalMargin;
            entity.RentalSettings.RentalMarginUnit = RentalMarginUnit;
            entity.RentalSettings.MinRentalAmount = MinRentalAmount;

            entity.MinMultiplier = MinMultiplier;
            entity.MaxMultiplier = MaxMultiplier;
            entity.MultiplierPrice = MultiplierPrice;
            entity.PackageCredits = PackageCredits;

            #endregion Rental

            #region Policies

            entity.CancelOrderPolicy = CancelOrderPolicy;
            entity.ChangePeriodRentalPolicy = ChangePeriodRentalPolicy;

            #endregion Policies

            #region Images

            entity.Images = Images.FixPosition();
            SetCoverImageUrl(entity);

            #endregion Images

            #region Availability

            entity.ProductAvailabilityMode = AvailabilityMode;

            #endregion Availability

            CalculatePrice(entity, shifts);

            return entity;
        }

        public void Update(Product entity, ProductCategory category, IList<ProductVariant> variants, ProductVariantType stockVariantType, IList<RentalShiftDto> shifts)
        {
            entity.CustomId = CustomId;
            entity.Title = Title;
            entity.ProductType = ProductType;
            entity.Description = Description;
            entity.Specifications = Specifications;
            entity.Category = category;
            entity.Length = Length;
            entity.Width = Width;
            entity.Height = Height;
            entity.Weight = Weight;
            entity.FriendlyUrl = FriendlyUrl;
            entity.IsDisabled = DisableProduct;
            entity.MaxInstallments = MaxInstallments ?? 3;

            if (Id != null && Id != Guid.Empty)
                entity.Id = Id;

            entity.MustBeIntegrate = MustBeIntegrate;

            entity.Variants.Clear();
            foreach (var item in variants)
                entity.Variants.Add(item);

            #region Stock management

            var stockModeChanged = entity.StockByVariant != StockByVariant;

            var oldStockVariants = entity.StockVariants.ToArray();

            entity.StockByVariant = StockByVariant;

            entity.StockVariants.Clear();

            if (StockByVariant)
            {
                entity.StockVariantType = stockVariantType;

                UpdatedStock = 0;

                StockVariants = StockVariants?
                    .Where(i => i.StockVariant.HasValue && i.UpdatedStock.HasValue)
                    .DistinctBy(i => i.StockVariant)
                    .ToList();

                StockVariants.ForEach(sv =>
                {
                    entity.StockVariants.Add(sv.Create(variants));

                    UpdatedStock += (sv.UpdatedStock ?? sv.CurrentStock);
                });
            }
            else
            {
                entity.StockVariantType = null;
            }

            entity.CurrentStock = CurrentStock ?? 0;
            entity.UpdatedStock = UpdatedStock ?? entity.CurrentStock;
            entity.DeleteWhenStockZero = DeleteWhenStockZero;

            #region Alteração de estoque

            // mudou o modo de controle de estoque
            if (stockModeChanged)
            {
                // mudou para o modo estoque por caracteristica
                if (StockByVariant)
                {
                    if (entity.CurrentStock > 0M)
                    {
                        // remove o estoque geral
                        entity.AddDomainEvent(new ProductStockUpdatedEvent(entity, null, entity.Title,
                            0, (int)entity.CurrentStock));
                    }

                    // aplica o estoque das caracteristicas
                    foreach (var stockVariant in entity.StockVariants.Where(i => i.UpdatedStock != i.CurrentStock && i.UpdatedStock > 0))
                    {
                        entity.AddDomainEvent(new ProductStockUpdatedEvent(entity, stockVariant.Variant, entity.Title,
                            stockVariant.UpdatedStock, 0));
                    }
                }
                // mudou para o modo estoque geral
                else
                {
                    // remove o estoque das caracteristicas
                    foreach (var stockVariant in oldStockVariants.Where(i => i.CurrentStock > 0))
                    {
                        entity.AddDomainEvent(new ProductStockUpdatedEvent(entity, stockVariant.Variant, entity.Title,
                            0, stockVariant.CurrentStock));
                    }

                    // aplica o estoque geral
                    entity.AddDomainEvent(new ProductStockUpdatedEvent(entity, null, entity.Title,
                        (int)entity.UpdatedStock, 0));
                }
            }
            // não mudou o modo de controle de estoque
            else
            {
                // o modo é estoque por caracteristica
                if (StockByVariant)
                {
                    // caracteristicas removidas
                    var variantsRemoved = oldStockVariants.Where(i => entity.StockVariants.All(j => j.Variant.Id != i.Variant.Id));

                    // remove estoque das caracteristicas removidas
                    foreach (var stockVariant in variantsRemoved.Where(i => i.CurrentStock > 0))
                    {
                        entity.AddDomainEvent(new ProductStockUpdatedEvent(entity, stockVariant.Variant, entity.Title,
                            0, stockVariant.CurrentStock));
                    }

                    // aplicação alteração de estoque das caracteristicas
                    foreach (var stockVariant in entity.StockVariants.Where(i => i.CurrentStock != i.UpdatedStock))
                    {
                        entity.AddDomainEvent(new ProductStockUpdatedEvent(entity, stockVariant.Variant, entity.Title,
                            stockVariant.UpdatedStock, stockVariant.CurrentStock));
                    }
                }
                // o modo é estoque geral
                else
                {
                    // aplica alteração de estoque geral
                    if (entity.CurrentStock != entity.UpdatedStock)
                    {
                        entity.AddDomainEvent(new ProductStockUpdatedEvent(entity, null, entity.Title,
                            (int)entity.UpdatedStock, (int)entity.CurrentStock));
                    }
                }
            }

            #endregion Alteração de estoque

            #endregion Stock management

            MapShippingToEntity(entity);

            #region Rental

            entity.RentalSettings.MinRentalPeriod = MinRentalPeriod;
            entity.RentalSettings.MaxRentalPeriod = MaxRentalPeriod;

            if (RentalFixedPeriod.HasValue && MaaSConfig.Current.Product.BusinessModel.Rental.FixedPeriodRental.Enabled)
            {
                entity.RentalSettings.MinRentalPeriod = RentalFixedPeriod.Value;
                entity.RentalSettings.MaxRentalPeriod = RentalFixedPeriod.Value;
            }

            entity.RentalSettings.BlockScheduleForProduct = BlockScheduleForProduct;
            entity.RentalSettings.MinRentalNotice = MinRentalNotice;
            entity.RentalSettings.MinRentalNoticeUnit = MinRentalNoticeUnit;
            entity.RentalSettings.ChargeDoubleShipping = ChargeDoubleShipping;
            entity.RentalSettings.AvailabilityWarning = AvailabilityWarning;
            entity.RentalSettings.RentalPeriodUnit = ProductRentalPeriodUnit;

            entity.RentalSettings.MinRentalMargin = RentalMargin;
            entity.RentalSettings.RentalMarginUnit = RentalMarginUnit;
            entity.RentalSettings.MinRentalAmount = MinRentalAmount;

            entity.MultiplierPrice = MultiplierPrice;
            entity.MaxMultiplier = MaxMultiplier;
            entity.MinMultiplier = MinMultiplier;
            entity.PackageCredits = PackageCredits;

            #region Price Multiplier

            if (MinMultiplier == null)
                MinMultiplier = minMultiplierDefault;

            if (MaxMultiplier == null)
                MaxMultiplier = maxMultiplierDefault;

            if (MultiplierPrice == null)
                MultiplierPrice = multiplierPriceDefault;

            #endregion Price Multiplier

            #endregion Rental

            #region Policies

            entity.CancelOrderPolicy = CancelOrderPolicy;
            entity.ChangePeriodRentalPolicy = ChangePeriodRentalPolicy;

            #endregion Policies

            #region Images

            Images = Images.FixPosition();

            var imagesToRemove = entity.Images.Where(i => Images.All(j => i.Url != j.Url)).ToList();
            foreach (var image in imagesToRemove)
                entity.Images.Remove(image);

            var imagesToAdd = Images.Where(i => entity.Images.All(j => i.Url != j.Url)).ToList();
            foreach (var image in imagesToAdd)
                entity.Images.Add(image);

            entity.Images.FixPosition(true);
            SetCoverImageUrl(entity);

            #endregion Images

            #region Availability

            entity.ProductAvailabilityMode = AvailabilityMode;

            #endregion Availability

            CalculatePrice(entity, shifts);
        }

        private void MapShippingToEntity(Product entity)
        {
            entity.NoShippingEnabled = false;
            entity.NoShippingUrl = null;
            
            entity.ActualNoShippingEnabled = false;
            entity.OnSitePickupEnabled = false;

            entity.SedexEnabled = false;
            entity.PACEnabled = false;
            entity.DefaultShipping.Amount = 0;
            entity.DefaultShipping.DeliveryDays = 0;

            entity.PrivateFleetEnabled = false;
            entity.PrivateFleetParams.Clear();

            entity.Length = null;
            entity.Width = null;
            entity.Height = null;
            entity.Weight = null;

            if (ShippingType == ProductEditorShippingType.Digital)
            {
                entity.NoShippingEnabled = true;
                entity.NoShippingUrl = NoShippingUrl;
            }
            else if (ShippingType == ProductEditorShippingType.Physical)
            {
                entity.ActualNoShippingEnabled = ActualNoShippingEnabled;
                entity.OnSitePickupEnabled = OnSitePickupEnabled;

                entity.SedexEnabled = SedexEnabled;
                entity.PACEnabled = PACEnabled;
                entity.DefaultShipping.Amount = DefaultShippingAmount.GetValueOrDefault();
                entity.DefaultShipping.DeliveryDays = DefaultShippingDeliveryDays.GetValueOrDefault();

                entity.PrivateFleetEnabled = PrivateFleetEnabled;
                if (PrivateFleetEnabled)
                {
                    PrivateFleetParams = PrivateFleetParams.DistinctBy(i => i.Range).ToList();
                    PrivateFleetParams.ForEach(param => { entity.PrivateFleetParams.Add(param.Create()); });
                }
                else
                {
                    PrivateFleetParams.Clear();
                    entity.PrivateFleetParams.Clear();
                }

                entity.Length = Length;
                entity.Width = Width;
                entity.Height = Height;
                entity.Weight = Weight;
            }
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();
            var config = MaaSConfig.Current;

            if (config.Product.Image.IsRequired && Images.Count == 0 && (!Images.Any() || Images.FirstOrDefault() == null))
                errors.Add(nameof(Images), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_IMAGES_REQUIRED"));

            if (config.Product.ShippingMode == ShippingMode.PerProduct)
            {
                if (!(PrivateFleetEnabled || SedexEnabled || PACEnabled || OnSitePickupEnabled || NoShippingEnabled || ActualNoShippingEnabled))
                    errors.Add("", CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_SHIPPING_REQUIRED"));

                if (config.Product.PrivateFleetShipping.Enabled && PrivateFleetEnabled)
                {
                    if (!PrivateFleetParams.Any())
                        errors.Add(nameof(PrivateFleetParams), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_PRIVATE_FLEET_PARAMS_REQUIRED"));
                    else
                        if (!config.Product.PrivateFleetShipping.AllowFreeShipping && PrivateFleetParams.Any(x => x.Shipping <= 0))
                        errors.Add(nameof(PrivateFleetParams), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_PRIVATE_FLEET_CANNOT_BE_FREE"));
                }

                if (config.Product.NoShipping.Enabled && NoShippingEnabled)
                {
                    if (string.IsNullOrEmpty(NoShippingUrl))
                        errors.Add(nameof(NoShippingUrl), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_NO_SHIPPING_URL_REQUIRED"));

                    else if (!NoShippingUrl.IsUrl())
                        errors.Add(nameof(NoShippingUrl), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_NO_SHIPPING_URL_INVALID"));
                }

                var defaultShippingHasErrors = false;

                if (DefaultShippingAmount < 0 || DefaultShippingDeliveryDays < 0)
                    defaultShippingHasErrors = true;

                if ((config.Product.SedexShipping.Enabled && SedexEnabled) || (config.Product.PACShipping.Enabled && PACEnabled))
                {
                    if (Width == null || Width == 0 ||
                        Height == null || Height == 0 ||
                        Length == null || Length == 0 ||
                        Weight == null || Weight == 0)
                        errors.Add(nameof(Weight), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_DIMENSIONS_REQUIRED"));

                    if (!DefaultShippingAmount.HasValue || !DefaultShippingDeliveryDays.HasValue)
                    {
                        DefaultShippingAmount = 0;
                        DefaultShippingDeliveryDays = 0;
                    }
                }

                if (defaultShippingHasErrors)
                    errors.Add(nameof(DefaultShippingAmount), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_DEFAULT_SHIPPING_ERROR"));
            }

            return errors;
        }

        public string GetRentalPeriodString()
        {
            var result = "";

            switch (ProductRentalPeriodUnit)
            {
                case RentalPeriodUnit.Minute:
                    result = GlobalizationHelper.GetString("RENTAL_TIMESPAN_MINUTE");
                    break;

                case RentalPeriodUnit.Hour:
                    result = GlobalizationHelper.GetString("RENTAL_TIMESPAN_HOUR");
                    break;

                case RentalPeriodUnit.Day:
                    result = GlobalizationHelper.GetString("RENTAL_TIMESPAN_DAY");
                    break;

                case RentalPeriodUnit.Month:
                    result = GlobalizationHelper.GetString("RENTAL_TIMESPAN_MONTH");
                    break;
            }

            return result.ToLower();
        }
    }

    public enum ProductPriceMultiplierOption
    {
        [DescriptionLocalized("PRODUCT_PRICE_MULTIPLIER_OPTION_FIXED")]
        Fixed,

        [DescriptionLocalized("PRODUCT_PRICE_MULTIPLIER_OPTION_ENABLED")]
        Enabled
    }

    public enum ProductEditorShippingType
    {
        [DescriptionLocalized("PRODUCT_SHIPPING_TYPE_PHYSICAL")]
        Physical,

        [DescriptionLocalized("PRODUCT_SHIPPING_TYPE_DIGITAL")]
        Digital
    }
}