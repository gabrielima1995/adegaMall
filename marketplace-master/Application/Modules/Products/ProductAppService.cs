using Application.Config;
using Application.Modules.Products.Search;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Services.StoreModule;
using Framework.Geography;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using V2.Application.Cities;

namespace Application.Modules.Products
{
    public class ProductAppService : IProductAppService
    {
        private readonly ProductService_Obsolete productService;
        private readonly ProductCategoryService productCategoryService;
        private readonly ProductSearchStore productSearchStore;
        private readonly IProductRepository productRepository;
        private readonly ICityQueries cityQueries;

        public ProductAppService(
            ProductService_Obsolete productService,
            ProductCategoryService productCategoryService,
            ProductSearchStore productSearchStore,
            IProductRepository productRepository,
            ICityQueries cityQueries)
        {
            this.productService = productService;
            this.productCategoryService = productCategoryService;
            this.productSearchStore = productSearchStore;
            this.productRepository = productRepository;
            this.cityQueries = cityQueries;
        }

        private IList<ProductCategory> GetAllCategories()
        {
            return productCategoryService
                .Query()
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Name)
                .ToList();
        }

        private IList<ProductWithPrice> SearchFilterBy(SearchRequestViewModel request)
        {
            var onSitePickupShippingEnabled = MaaSConfig.Current.Product.OnSitePickupShipping.Enabled;

            StringHelper.Sanitize(request);
            IEnumerable<ProductWithPrice> productsWithPrice = productSearchStore.GetProducts();

            if (MaaSConfig.Current.Seller.Moderation.Enabled)
                productsWithPrice = productsWithPrice.Where(x => x.Product.Store.Seller.RequireModeration != true);

            if (request.SellerId != null)
                productsWithPrice = productsWithPrice.Where(x => x.Product.Store.SellerId == request.SellerId);

            if (request.Product != null)
                productsWithPrice = productsWithPrice.Where(i =>
                    SearchWords(i.Product.Title, request.Product) ||
                    SearchWords(i.Product.Description, request.Product) ||
                    SearchWords(i.Product.Specifications, request.Product) ||
                    SearchWords(i.Product.Store.Seller.AlternativeName, request.Product) ||
                    (i.Product.CustomId != null && SearchWords(i.Product.CustomId, request.Product)));

            if (request.Category != null)
                productsWithPrice = productsWithPrice.Where(i => i.Product.Category != null && i.Product.Category.Id == request.Category);

            if (request.VariantIds.Any())
                productsWithPrice = productsWithPrice.Where(i => request.VariantIds.All(j => i.Product.Variants.Any(k => j == k.Id)));

            productsWithPrice = productsWithPrice.Where(product =>
                // Cidade não especificada e qualquer tipo de retirada
                (request.CityAndState == null && !request.OnlyOnsitePickup)
                // Cidade não especificada mas apenas produtos com retirada no local
                || (request.CityAndState == null && request.OnlyOnsitePickup && (onSitePickupShippingEnabled && product.Product.OnSitePickupEnabled || product.Product.Store.OnSitePickupEnabled))
                // Cidade especificada e qualquer tipo de retirada
                || (product.Product.Store.Seller.Address?.CityAndState == request.CityAndState && !request.OnlyOnsitePickup)
                // Cidade especificada E produtos com retirada no local
                || (product.Product.Store.Seller.Address?.CityAndState == request.CityAndState && request.OnlyOnsitePickup && (onSitePickupShippingEnabled && product.Product.OnSitePickupEnabled || product.Product.Store.OnSitePickupEnabled))
                // Pode entregar na cidade selecionada
                || (!request.OnlyOnsitePickup && CanDeliver(product, request)));

            productsWithPrice = productsWithPrice.Where(product => IsInRange(request, product));
            return productsWithPrice.ToList();
        }

        private bool SearchWords(string source, string query)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace;

            var words = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words.All(word => compareInfo.IndexOf(source, word, options) >= 0);
        }

        private bool IsInRange(SearchRequestViewModel request, ProductWithPrice productWithPrice)
        {
            var bounds = request.GetBoundsOrNull();

            if (productWithPrice.Product.Store.Seller.Address == null)
                return false;

            if (bounds != null && MaaSConfig.Current.Search.SearchFields.Location)
            {
                var dbBounds = bounds.AsDbGeography();

                // Área mínima da boundary deve ser a área do círculo
                var minArea = Math.Pow(MaaSConfig.Current.Search.Map.SearchRange, 2) * Math.PI;
                var location = productWithPrice.Product.Store.Seller.Address.Location;

                // Se a área determinada for grande o suficiente e contiver a localização do produto
                if (location != null && dbBounds.Area > minArea)
                    return dbBounds.Intersects(location);

                // Se a área for muito pequena, considerar um raio ao redor do centro
                else if (location != null)
                {
                    double latitude = (request.BoundsAy.Value + request.BoundsBy.Value) / 2d;
                    double longitude = (request.BoundsAx.Value + request.BoundsBx.Value) / 2d;
                    var pointString = string.Format("POINT({0} {1})", longitude, latitude).Replace(',', '.');
                    var centerPoint = DbGeography.FromText(pointString);

                    var result = location.Distance(centerPoint) < MaaSConfig.Current.Search.Map.SearchRange;
                    return result;
                }

                return false;
            }

            return true;
        }

        private bool CanDeliver(ProductWithPrice productWithPrice, SearchRequestViewModel request)
        {
            if (productWithPrice.Product.SedexEnabled || productWithPrice.Product.PACEnabled)
                return true;

            var shippingPerStore = MaaSConfig.Current.Product.ShippingMode == ShippingMode.PerStore;
            var shippingPerProduct = MaaSConfig.Current.Product.ShippingMode == ShippingMode.PerProduct;
            var privateFleetShippingEnabled = MaaSConfig.Current.Product.PrivateFleetShipping.Enabled;

            if (productWithPrice.Product.PrivateFleetEnabled)
            {
                DbGeography geography = null;
                if (request.CityAndState != null)
                    geography = cityQueries.GetCityDbGeography(request.CityAndState);
                else if (request.GetBoundsOrNull() != null)
                    geography = request.GetBoundsOrNull().AsDbGeography();

                if (shippingPerProduct && productWithPrice.Product.PrivateFleetEnabled)
                    return productWithPrice.Product.PrivateFleetParams.Any(j => j.DeliveryRange >= productWithPrice.Product.Store.Seller.Address.Location.Distance(geography) / 1000);

                else if (shippingPerStore)
                    return productWithPrice.Product.Store.SingleFreightParams.Any(j => j.DeliveryRange >= productWithPrice.Product.Store.Seller.Address.Location.Distance(geography) / 1000);
            }

            return false;
        }

        private void SearchResults(SearchResultViewModel response, SearchRequestViewModel request, out int total)
        {
            var resultPre = SearchFilterBy(request);
            IOrderedEnumerable<ProductWithPrice> resultSorted = null;

            switch (request.SortType)
            {
                case Domain.DTO.ProductSortType.LowerPrice:
                    resultSorted = resultPre.OrderBy(x => x.Product.Price);
                    break;

                case Domain.DTO.ProductSortType.HigherPrice:
                    resultSorted = resultPre.OrderByDescending(x => x.Product.Price);
                    break;

                case Domain.DTO.ProductSortType.Alphabetical:
                    resultSorted = resultPre.OrderBy(x => x.Product.Title);
                    break;

                case Domain.DTO.ProductSortType.Newer:
                    resultSorted = resultPre.OrderByDescending(x => x.Product.CreatedAt);
                    break;

                case Domain.DTO.ProductSortType.MostRelevant:
                default: break;
            }

            List<ProductWithPrice> allProducts = null;

            if (resultSorted == null)
            {
                allProducts = resultPre
                    .OrderByDescending(p => p.Product.Store.Seller.Rating)
                    .ThenByDescending(p => p.Product.Store.Seller.Score)
                    .ThenBy(p => p.Product.Store.Seller.CreatedAt)
                    .ThenByDescending(p => p.Product.UpdatedAt)
                    .ToList();
            }
            else
            {
                allProducts = resultSorted
                    .ThenByDescending(p => p.Product.Store.Seller.Rating)
                    .ThenByDescending(p => p.Product.Store.Seller.Score)
                    .ThenBy(p => p.Product.Store.Seller.CreatedAt)
                    .ThenByDescending(p => p.Product.UpdatedAt)
                    .ToList();
            }

            if (MaaSConfig.Current.Product.Variants.Enabled && request.Page == 1)
                response.Facets = BuildFacetResults(allProducts, request);

            total = allProducts.Count;

            //response.ResultsOld = allProducts
            //    .Skip((request.Page - 1) * request.Limit)
            //    .Take(request.Limit)
            //    .ToList();
        }

        private IList<FacetGroup> BuildFacetResults(IList<ProductWithPrice> productsWithPrice, SearchRequestViewModel request)
        {
            var facetResults = new List<FacetGroup>();

            var variantGroups = productsWithPrice
                .SelectMany(x => x.Product.Variants)
                .OrderBy(x => x.Type.Order)
                .ThenBy(x => x.Type.Name)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.Name)
                .GroupBy(x => x.Type.Id)
                .ToList();

            foreach (var variantGroup in variantGroups)
            {
                var facetGroup = new FacetGroup(variantGroup.First().Type.Name);
                facetResults.Add(facetGroup);

                foreach (var variant in variantGroup)
                {
                    var facetItem = facetGroup.Items.SingleOrDefault(x => x.VariantId == variant.Id);

                    if (facetItem == null)
                    {
                        facetItem = new FacetItem
                        {
                            Name = variant.Type.Name,
                            Value = variant.Name,
                            VariantId = variant.Id,
                            Selected = request.VariantIds.Contains(variant.Id)
                        };

                        facetGroup.Items.Add(facetItem);
                    }

                    facetItem.Quantity += 1;
                }
            }

            return facetResults;
        }

        public SearchViewModel Search(SearchRequestViewModel request, string buyerId)
        {
            var response = new SearchViewModel(request);

            SearchResults(response, request, out int total);
            response.Total = total;
            response.Categories = GetAllCategories();

            return response;
        }

        public SearchResultViewModel SearchOnlyResults(SearchRequestViewModel request, string buyerId)
        {
            var response = new SearchResultViewModel();
            SearchResults(response, request, out _);
            return response;
        }

        public IList<Product> GetRelatedProducts(Guid productId, IEnumerable<Guid> sellerProductsIds)
        {
            var config = MaaSConfig.Current;
            var product = productRepository.GetById(productId);

            var queryable = productService.Query(
                x => x.Images,
                x => x.Store,
                x => x.Store.Seller,
                x => x.Variants);

            if (product.Category != null)
                queryable = queryable
                    .Where(x => x.Category.Id == product.Category.Id);

            return queryable
                .Where(x =>
                    x.Id != product.Id &&
                    x.Store.Seller.Enabled && x.Store.Seller.RequireModeration != true &&
                    x.IsBillingProduct != true &&
                    (x.CurrentStock > 0 || x.NoShippingEnabled) &&
                    x.Deleted == false &&
                    (!config.Subscription.IsRequired ||
                        (x.Store.Seller.Subscription != null &&
                        (x.Store.Seller.Subscription.Status == SubscriptionStatus.Trialing ||
                            x.Store.Seller.Subscription.Status == SubscriptionStatus.Paid ||
                            x.Store.Seller.Subscription.Status == SubscriptionStatus.PendingPayment))) &&
                    !sellerProductsIds.Contains(x.Id))
                .Take(24)
                .ToList()
                .Where(x => x.Store.Seller.SellerCompleteProfileMapped.IsComplete)
                .Take(8)
                .ToList();
        }
    }
}