using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Events.SellerProfileUpdated;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.Store;
using Domain.Modules.Products.Variants;
using Domain.Sellers;
using Domain.Services.StoreModule;
using Domain.StoreModule.Commands;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using V2.Application.Modules.ProductTemplateModule;
using StoreModule = Domain.Entities.StoreModule;

namespace Application.Modules.Products.Store
{
    public class StoreAppService : IStoreAppService
    {
        private static bool RentalShiftsEnabled => MaaSConfig.Current.Product.BusinessModel.Rental.Enabled && MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts;

        private readonly ProductService_Obsolete _productService;
        private readonly ProductCategoryService _productCategoryService;
        private readonly StoreService_Obsolete _storeService;
        private readonly SellerService_Obsolete _sellerService;
        private readonly ProductVariantService _productVariantService;
        private readonly ProductVariantTypeService _productVariantTypeService;
        private readonly IProductRepository _productRepository;
        private readonly IProductTemplateRepository _productTemplateRepository;
        private readonly IRentalShiftQueries _rentalShiftQueries;
        private readonly SetProductRentalShiftsCommandHandler _setProductRentalShiftsCommandHandler;
        private readonly ISellerRepository _sellerRepository;

        public StoreAppService(
            StoreService_Obsolete storeService,
            ProductService_Obsolete productService,
            ProductCategoryService productCategoryService,
            SellerService_Obsolete sellerService,
            ProductVariantService productVariantService,
            ProductVariantTypeService productVariantTypeService,
            IProductRepository productRepository,
            IProductTemplateRepository productTemplateRepository,
            IRentalShiftQueries rentalShiftQueries,
            SetProductRentalShiftsCommandHandler setProductRentalShiftsCommandHandler,
            ISellerRepository sellerRepository)
        {
            _storeService = storeService;
            _productService = productService;
            _productCategoryService = productCategoryService;
            _sellerService = sellerService;
            _productVariantService = productVariantService;
            _productVariantTypeService = productVariantTypeService;
            _productRepository = productRepository;
            _productTemplateRepository = productTemplateRepository;
            _rentalShiftQueries = rentalShiftQueries;
            _setProductRentalShiftsCommandHandler = setProductRentalShiftsCommandHandler;
            _sellerRepository = sellerRepository;
        }

        private Expression<Func<Product, object>>[] GetIncludes()
        {
            return new Expression<Func<Product, object>>[]
            {
                i => i.Images,
                i => i.Category,
                i => i.PrivateFleetParams,
                i => i.Variants,
                i => i.Variants.Select(j => j.Type),
                i => i.StockVariants,
                i => i.StockVariants.Select(j => j.Variant),
                i => i.Availabilities,
            };
        }

        public ProductEditorViewModel PrepareNewProduct(Guid? templateId)
        {
            var categories = _productCategoryService.ListActive();

            var model = new ProductEditorViewModel(categories)
            {
                CurrentStock = 1,
                UpdatedStock = 1
            };

            var variantTypesList = _productVariantTypeService.SelectorList();
            model.Variants.Add(new Variants.ProductVariantTypeAndItemSelector()
            {
                VariantType = 0,
                Variant = 0,
                VariantTypes = variantTypesList,
                Variants = new List<ProductVariant>(0)
            });

            if (MaaSConfig.Current.Product.Variants.StockByVariantEnabled)
            {
                model.StockVariantTypes = variantTypesList;
                if (model.StockByVariant && model.StockVariantType.HasValue)
                {
                    model.StockVariants.Add(new Variants.ProductStockVariantItemViewModel()
                    {
                        StockVariant = 0,
                        StockVariants = new List<ProductVariant>(0)
                    });
                }
            }

            if (RentalShiftsEnabled)
            {
                model.RentalShiftOptions = _rentalShiftQueries
                    .ListRentalShifts()
                    .Select(shift => RentalShiftViewModel.From(shift))
                    .ToList();
            }

            if (MaaSConfig.Current.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.Enabled)
            {
                model.RentalFixedPeriod = MaaSConfig.Current.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.MaxFixedPeriod;
                model.MinRentalPeriod = MaaSConfig.Current.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.MaxFixedPeriod;
                model.MaxRentalPeriod = MaaSConfig.Current.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.MaxFixedPeriod;
                model.ProductRentalPeriodUnit = MaaSConfig.Current.Product.BusinessModel.Rental.RentalFixedPeriodForAllProducts.FixedPeriodType;
            }
            model.Id = Guid.NewGuid();

            if (templateId.HasValue)
            {
                var template = _productTemplateRepository.Find(templateId.Value);
                model.TemplateId = template.Id;
                model.Title = template.Title;
                model.TitleIsEditable = template.TitleIsEditable;
                model.Category = template.Category.Id;
                model.CategoryIsEditable = template.CategoryIsEditable;
                model.Description = template.Description;
                model.DescriptionIsEditable = template.DescriptionIsEditable;
                model.Specifications = template.Specification;
                model.SpecificationsIsEditable = template.SpecificationIsEditable;

                var imagePosition = 0;
                model.Images = template.Images.SelectList(img => new ProductImage { Url = img.Url, Position = imagePosition++ });
                model.ImagesIsEditable = template.ImagesIsEditable;

                model.Height = template.Dimensions.Height;
                model.Length = template.Dimensions.Length;
                model.Weight = template.Dimensions.Weight;
                model.Width = template.Dimensions.Width;
                model.DimensionsIsEditable = template.DimensionsIsEditable;

                model.VariantsIsEditable = template.VariantsIsEditable;
                if (template.Variants.Count > 0)
                {
                    var variantItemsList = _productVariantService.SelectorList();
                    var currentVariants = template.Variants
                        .OrderBy(x => x.Type.Order).ThenBy(x => x.Type.Name)
                        .ThenBy(x => x.Order).ThenBy(x => x.Name)
                        .ToList();

                    model.Variants.Clear();
                    model.StockVariants.Clear();

                    foreach (var item in currentVariants)
                    {
                        model.Variants.Add(new Variants.ProductVariantTypeAndItemSelector
                        {
                            VariantType = item.Type.Id,
                            Variant = item.Id,
                            VariantTypes = variantTypesList,
                            Variants = variantItemsList.Where(x => x.Type.Id == item.Type.Id).ToList()
                        });

                        if (MaaSConfig.Current.Product.Variants.StockByVariantEnabled)
                        {
                            var stockVariant = StockProductVariant.New(item, 0);
                            model.StockVariants.Add(new Variants.ProductStockVariantItemViewModel(stockVariant));
                        }
                    }
                }

                model = PrepareProduct(model);
            }

            return model;
        }

        public ProductEditorViewModel PrepareProduct(ProductEditorViewModel model)
        {
            model.Categories = _productCategoryService.ListActive();

            #region Popula as variantes para seleção

            var variantTypesList = _productVariantTypeService.SelectorList();
            var variantItemsList = _productVariantService.SelectorList();

            if (model.Variants.Count == 0)
            {
                model.Variants.Add(new Variants.ProductVariantTypeAndItemSelector
                {
                    VariantType = 0,
                    Variant = 0
                });
            }

            foreach (var item in model.Variants)
            {
                item.VariantTypes = variantTypesList;
                item.Variants = variantItemsList.Where(x => x.Type.Id == item.VariantType).ToList();
            }

            #endregion

            if (MaaSConfig.Current.Product.Variants.StockByVariantEnabled)
            {
                model.StockVariantTypes = variantTypesList;
                if (model.StockByVariant && model.StockVariantType.HasValue)
                {
                    if (model.StockVariants.Count == 0)
                        model.StockVariants.Add(new Variants.ProductStockVariantItemViewModel { StockVariant = 0 });

                    var items = variantItemsList.Where(x => x.Type.Id == model.StockVariantType.Value).ToList();

                    foreach (var item in model.StockVariants)
                        item.StockVariants = items;
                }
            }

            if (RentalShiftsEnabled)
            {
                model.RentalShiftOptions = _rentalShiftQueries
                    .ListRentalShifts()
                    .Select(shift => RentalShiftViewModel.From(shift))
                    .ToList();

                foreach (var shift in model.RentalShiftOptions)
                    if (model.RentalShiftIds.Contains(shift.Id))
                        shift.Selected = true;
            }

            return model;
        }

        public ProductEditorViewModel GetProduct(Guid productId, string sellerId)
        {
            var product = _productService.Get(
                i => i.Id == productId && i.Store.SellerId == sellerId && !i.Deleted,
                GetIncludes());

            if (product == null)
                return null;

            var categories = _productCategoryService.ListActive();
            var viewModel = new ProductEditorViewModel(product, categories);

            #region Popula as variantes para seleção

            var currentVariants = product.Variants
                .OrderBy(x => x.Type.Order)
                .ThenBy(x => x.Type.Name)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToList();

            var variantTypesList = _productVariantTypeService.SelectorList();
            var variantItemsList = _productVariantService.SelectorList();

            if (currentVariants.Count == 0)
                currentVariants.Add(new ProductVariant { Type = new ProductVariantType() });

            foreach (var item in currentVariants)
            {
                viewModel.Variants.Add(new Variants.ProductVariantTypeAndItemSelector
                {
                    VariantType = item.Type.Id,
                    Variant = item.Id,
                    VariantTypes = variantTypesList,
                    Variants = variantItemsList.Where(x => x.Type.Id == item.Type.Id).ToList()
                });
            }

            #endregion

            if (MaaSConfig.Current.Product.Variants.StockByVariantEnabled)
            {
                #region Popula as variantes para estoque

                viewModel.StockVariantTypes = variantTypesList;

                if (viewModel.StockByVariant && viewModel.StockVariantType.HasValue)
                {
                    var currentStockVariants = product.StockVariants
                        .OrderBy(x => x.Variant.Order)
                        .ThenBy(x => x.Variant.Name)
                        .ToList();

                    if (currentStockVariants.Count == 0)
                    {
                        currentStockVariants.Add(StockProductVariant.New(new ProductVariant(), 0));
                    }

                    var items = variantItemsList.Where(x => x.Type.Id == viewModel.StockVariantType.Value).ToList();

                    foreach (var item in currentStockVariants)
                    {
                        viewModel.StockVariants.Add(new Variants.ProductStockVariantItemViewModel(item)
                        {
                            StockVariants = items
                        });
                    }
                }

                #endregion
            }

            if (RentalShiftsEnabled)
            {
                var productShifts = _rentalShiftQueries.ListProductShifts(productId);
                viewModel.RentalShiftIds = productShifts.Select(shift => shift.Id).ToList();
            }

            if (product.ProductTemplateId.HasValue)
            {
                var template = _productTemplateRepository.Find(product.ProductTemplateId.Value);
                viewModel.TemplateId = template.Id;
                viewModel.TitleIsEditable = template.TitleIsEditable;
                viewModel.CategoryIsEditable = template.CategoryIsEditable;
                viewModel.DescriptionIsEditable = template.DescriptionIsEditable;
                viewModel.SpecificationsIsEditable = template.SpecificationIsEditable;
                viewModel.ImagesIsEditable = template.ImagesIsEditable;
                viewModel.DimensionsIsEditable = template.DimensionsIsEditable;
                viewModel.VariantsIsEditable = template.VariantsIsEditable;
            }

            return PrepareProduct(viewModel);
        }

        public OperationResult<Product> SaveProduct(ProductEditorViewModel model)
        {
            var config = MaaSConfig.Current;

            var store = _storeService.Get(i => i.SellerId == model.SellerId,
                i => i.Seller,
                i => i.Products);

            var category = _productCategoryService.Get(i => i.Id == model.Category && !i.Deleted);
            var variantIds = model.StockByVariant
                ? model.StockVariants.Where(x => x.StockVariant != null).Select(i => i.StockVariant).Distinct().ToList()
                : model.Variants.Where(x => x.Variant != null).Select(i => i.Variant).Distinct().ToList();
            var variants = _productVariantService.Query().Where(i => variantIds.Contains(i.Id)).ToList();
            var stockVariantType = model.StockByVariant && model.StockVariantType.HasValue
                ? _productVariantTypeService.Get(i => i.Id == model.StockVariantType.Value)
                : null;

            if (model.ShippingType == ProductEditorShippingType.Digital && !String.IsNullOrWhiteSpace(model.NoShippingUrl))
                model.NoShippingEnabled = true;

            var errors = model.Validate();

            if (store == null)
            {
                var seller = _sellerService.Get(i => i.Id == model.SellerId);
                store = new StoreModule.Store { Seller = seller };
            }
            else
            {
                if (model.Id == default(Guid) &&
                    config.Product.ProductsLimit != 0 &&
                    config.Product.ProductsLimit <= store.Products.Count)
                    errors.Add("", CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_LIMIT_MESSAGE"));
            }

            if (errors.IsEmpty)
            {
                if (model.FriendlyUrl == null)
                    SetProductFriendlyUrl(model);

                var shifts = RentalShiftsEnabled
                    ? _rentalShiftQueries.ListRentalShifts().ToList()
                    : new List<RentalShiftDto>();

                var savedProduct = _productService.Save(
                    i => i.Id == model.Id && i.Store.SellerId == model.SellerId && !i.Deleted,
                    () => model.Create(store, category, variants, stockVariantType, shifts),
                    entity =>
                    {
                        model.Update(entity, category, variants, stockVariantType, shifts);
                    },
                    GetIncludes());

                var saveRentalShifts = _setProductRentalShiftsCommandHandler.Handle(
                    new SetProductRentalShiftsCommand(model.Id, model.RentalShiftIds));

                if (saveRentalShifts.IsFailure)
                    return saveRentalShifts.Errors;

                var seller = _sellerRepository.GetById(model.SellerId);
                seller.ModifiedAt = DateTime.UtcNow;
                _sellerRepository.SaveChanges();
                return savedProduct;
            }

            return errors;
        }

        public OperationResult<StoreModule.Store> SaveStore(StoreViewModel model)
        {
            var config = MaaSConfig.Current;

            var store = _storeService.Get(i => i.SellerId == model.SellerId,
                i => i.Seller,
                i => i.SingleFreightParams,
                i => i.Products);

            if (store == null)
            {
                var seller = _sellerService.Get(i => i.Id == model.SellerId);
                store = new StoreModule.Store { Seller = seller };
            }

            return _storeService.Save(
                i => i.SellerId == model.SellerId,
                () => model.Create(),
                entity => model.Update(entity),
                i => i.Seller,
                i => i.Products,
                i => i.Products.Select(j => j.Images),
                i => i.Products.Select(j => j.Category),
                i => i.SingleFreightParams);
        }

        public void DeleteProduct(Guid productId, string sellerId)
        {
            _productService.Update(
                i => i.Id == productId && i.Store.SellerId == sellerId && !i.Deleted,
                entity =>
                {
                    entity.Deleted = true;
                    entity.AddDomainEvent(new SellerProfileUpdated(sellerId));
                },
                i => i.Store);
        }

        private void SetProductFriendlyUrl(ProductEditorViewModel model)
        {
            var url = GenerateFriendlyUrl(model.Title);
            var exists = _productRepository.FriendlyUrlExists(url);

            if (exists)
            {
                var uniqueTitle = Guid.NewGuid().ToString().Substring(0, 6) + " " + model.Title;
                url = GenerateFriendlyUrl(uniqueTitle);
            }

            model.FriendlyUrl = url;
        }

        private string GenerateFriendlyUrl(string productTitle)
        {
            return productTitle.ReplaceDiacritics().Hyphenize();
        }
    }
}