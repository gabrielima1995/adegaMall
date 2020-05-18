using Application.Config;
using Application.Helpers;
using Application.Modules.Products.Search;
using Domain.Categories;
using Domain.Config.General;
using Domain.Modules.Ratings.Queries;
using Domain.Services.StoreModule;
using System.Linq;
using V2.Application.Modules.Search;
using V2.Application.Modules.Search.Sellers;

namespace Application.Home
{
    public class HomeAppService
    {
        private readonly CategoryService categoryService;
        private readonly ProductCategoryService productCategoryService;
        private readonly ISellerSearchService sellerSearchService;
        private readonly IRatingsQueries ratingsQueries;

        public HomeAppService(
            CategoryService categoryService,
            ProductCategoryService productCategoryService,
            ISellerSearchService sellerSearchService,
            IRatingsQueries ratingsQueries)
        {
            this.categoryService = categoryService;
            this.productCategoryService = productCategoryService;
            this.sellerSearchService = sellerSearchService;
            this.ratingsQueries = ratingsQueries;
        }

        public HomeViewModel Prepare()
        {
            var viewModel = new HomeViewModel();
            SetupSearchIfEnabled(viewModel);
            SetBestSellersIfEnabled(viewModel);
            return viewModel;
        }

        private void SetupSearchIfEnabled(HomeViewModel viewModel)
        {
            if (!MaaSConfig.Current.Search.Enabled)
                return;

            if (MaaSConfig.Current.Search.SearchType == SearchType.Seller || MaaSConfig.Current.Search.SearchType == SearchType.Portfolio)
            {
                var categories = categoryService.ListActiveCategories();
                var subCategories = categoryService.ListActiveSubCategories();
                viewModel.SellersSearch = new SellersSearchHomeViewModel(categories, subCategories);
            }

            else if (MaaSConfig.Current.Search.SearchType == SearchType.Product)
            {
                var categories = productCategoryService.ListActive();
                viewModel.ProductsSearch = new ProductsSearchHomeViewModel(categories);
            }
        }

        private void SetBestSellersIfEnabled(HomeViewModel viewModel)
        {
            if (!MaaSConfig.Current.BestSellers.Enabled)
                return;

            var result = sellerSearchService.Search(new CommonSearchRequest
            {
                PageSize = 8,
                FilterBestResultsForHome = true
            });

            var ids = result.Items.Select(item => item.Id);
            var ratings = ratingsQueries.ListUserRatings(ids);
            var bestSellers = result.Items.Select(item =>
            {
                var sellerRating = ratings.SingleOrDefault(rating => rating.UserId == item.Id);
                var averageRating = sellerRating?.AverageRating ?? 0d;
                var reviewsCount = sellerRating?.ReviewsCount ?? 0;

                return new BestSellerDto(
                    item.Id,
                    item.Title,
                    item.Description,
                    ImgHelper.Avatar(item.ImageUrl, 600),
                    $"/p/{item.Handle}",
                    averageRating,
                    reviewsCount);
            });

            viewModel.BestSellers = bestSellers.ToList();
        }
    }
}