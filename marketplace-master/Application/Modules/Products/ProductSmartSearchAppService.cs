using Application.Config;
using Application.Modules.Products.Search;
using Domain.Entities.StoreModule;
using Domain.Services.StoreModule;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.Search.Products;

namespace Application.Modules.Products
{
    public class ProductSmartSearchAppService
    {
        private readonly ProductCategoryService productCategoryService;
        private readonly IProductSearchService productSearchService;
        private readonly IRepository<ProductVariant> productVariantRepository;

        public ProductSmartSearchAppService(
            ProductCategoryService productCategoryService,
            IProductSearchService productSearchService,
            IRepository<ProductVariant> productVariantRepository)
        {
            this.productCategoryService = productCategoryService;
            this.productSearchService = productSearchService;
            this.productVariantRepository = productVariantRepository;
        }

        #region Search

        private IList<ProductCategory> GetAllCategories()
        {
            return productCategoryService
                .Query()
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Name)
                .ToList();
        }

        private IList<FacetGroup> BuildFacetResults(ProductSearchResult searchResult, SearchRequestViewModel request)
        {
            var facetResults = new List<FacetGroup>();
            var facets = searchResult.Facets;

            var variantIds = facets
                .Select(i => int.Parse(i.Label))
                .ToArray();

            if (!variantIds.Any()) return facetResults;

            var variants = productVariantRepository
                .Query(i => i.Type)
                .Where(i => variantIds.Contains(i.Id))
                .ToList();

            var variantGroups = variants
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
                            Selected = request.VariantIds.Contains(variant.Id),
                            Quantity = facets.FirstOrDefault(i => i.Label == variant.Id.ToString())?.Count ?? 0
                        };

                        facetGroup.Items.Add(facetItem);
                    }
                }
            }

            return facetResults;
        }

        private void SearchResults(SearchResultViewModel response, SearchRequestViewModel request, out int total)
        {
            StringHelper.Sanitize(request);

            var products = productSearchService.Search(request.ToSearchRequest());

            if (MaaSConfig.Current.Product.Variants.Enabled && request.Page == 1)
                response.Facets = BuildFacetResults(products, request);

            total = products.TotalCount;
            response.Results = products.Items;
        }

        public SearchViewModel Search(SearchRequestViewModel request, string buyerId)
        {
            var response = new SearchViewModel(request);

            SearchResults(response, request, out var total);
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

        #endregion Search

        #region Nearby

        private void SearchResults(NearbyProductsResultViewModel response, NearbyProductsRequestViewModel request, out int total)
        {
            StringHelper.Sanitize(request);

            var products = productSearchService.Nearby(request.ToNearbyRequest());

            total = products.TotalCount;
            response.Results = products.Items;
        }

        public NearbyProductsViewModel Nearby(NearbyProductsRequestViewModel request)
        {
            var response = new NearbyProductsViewModel(request);

            SearchResults(response, request, out var total);
            response.Total = total;

            return response;
        }

        #endregion Nearby
    }
}