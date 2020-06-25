using Application.Config;
using Application.Helpers;
using Application.Modules.Ratings;
using Domain.Categories;
using Domain.DTO;
using Domain.Services;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.Search;
using SearchRequestType = V2.Application.Modules.Search.SearchRequestType;

namespace Presentation.Controllers
{
    public class SearchController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly SearchService _searchService;
        private readonly SearchAppService _searchAppService;
        private readonly IUserRatingAppService _ratingAppService;

        public SearchController(
            CategoryService categoryService,
            SearchService searchService,
            SearchAppService searchAppService,
            IUserRatingAppService ratingAppService)
        {
            _categoryService = categoryService;
            _searchService = searchService;
            _searchAppService = searchAppService;
            _ratingAppService = ratingAppService;
        }

        private ActionResult SearchByType(string searchtype)
        {
            if (MaaSConfig.Current.Search.RequireLogin && !Request.IsAuthenticated)
            {
                var url = Url.ActionLocalized("Login", "Account", new { logintosearch = 1 });
                return Redirect(url);
            }
            var searchResult = SearchInternal(includeFilters: true, searchType: searchtype);
            return View("Index", searchResult);
        }

        public ActionResult Index()
        {
            if (MaaSConfig.Current.Search.RequireLogin && !Request.IsAuthenticated)
            {
                var url = Url.ActionLocalized("Login", "Account", new { logintosearch = 1 });
                return Redirect(url);
            }
            var searchResult = SearchInternal(includeFilters: true, searchType: MaaSConfig.Current.Search.ResultType.ToString());
            return View(searchResult);
        }

        [ActionName("Products")]
        public ActionResult SearchProducts() => SearchByType("product");

        [ActionName("Sellers")]
        public ActionResult SearchSellers() => SearchByType("seller");

        [ActionName("Portfolios")]
        public ActionResult SearchPortfolios() => SearchByType("portfolio");

        public ActionResult ListPage(string searchtype)
        {
            var searchResult = SearchInternal(includeFilters: false, searchType: searchtype);
            return PartialView("_ResultList", searchResult);
        }

        private string BuildNextPageUrl(SearchResultViewMode viewMode, int currentPage, int totalItems, Domain.DTO.SearchRequestType searchtype)
        {
            var itemsShown = currentPage * GetPageSize(viewMode);
            var hasNextPage = itemsShown < totalItems;
            if (!hasNextPage) return null;

            var qryParams = new List<string> { "page=" + (currentPage + 1) };
            if (Request?.QueryString != null)
            {
                foreach (var key in Request?.QueryString?.AllKeys)
                {
                    if ("page".Equals(key, StringComparison.OrdinalIgnoreCase))
                        continue;

                    qryParams.Add($"{key}={Request.QueryString[key]}");
                }
            }

            var baseUrl = Url.ActionLocalized("ListPage", "Search");
            qryParams.Add($"searchtype={searchtype.ToString().ToLower()}");
            var url = baseUrl + "&" + string.Join("&", qryParams);
            
            return url;
        }

        private int GetPageSize(SearchResultViewMode mode)
        {
            return mode == SearchResultViewMode.Map ? 100 : MaaSConfig.Current.Search.List.PageSize;
        }

        private SearchResultVM SearchInternal(bool includeFilters, string searchType = null)
        {
            var requestVM = new SearchRequestVM(Request.QueryString);
            if (searchType == null)
            {
                searchType = requestVM.ActionUrl;
            }

            if (Enum.TryParse(searchType, true, out Domain.DTO.SearchRequestType type))
                requestVM.RequestType = type;

            return type == Domain.DTO.SearchRequestType.Portfolio ?
                SearchInternal_OldSearch(requestVM, includeFilters, requestVM.RequestType) :
                SearchInternal_SmartSearch(requestVM, includeFilters, requestVM.RequestType);
        }

        private SearchResultVM SearchInternal_OldSearch(SearchRequestVM requestVM, bool includeFilters, Domain.DTO.SearchRequestType searchType)
        {
            var config = MaaSConfig.Current;

            #region Ensure requested ViewMode is enabled

            if (requestVM.ViewMode == SearchResultViewMode.List && !config.Search.List.Enabled)
                requestVM.ViewMode = SearchResultViewMode.Map;

            if (requestVM.ViewMode == SearchResultViewMode.Map && !config.Search.Map.Enabled)
                requestVM.ViewMode = SearchResultViewMode.List;

            #endregion Ensure requested ViewMode is enabled

            var requestDto = requestVM.ToSearchRequestDto();
            var resultDto = _searchService.Search(requestDto);
            var resultVM = new SearchResultVM
            {
                NextPageUrl = BuildNextPageUrl(requestVM.ViewMode, requestVM.Page, resultDto.Result.TotalItems, searchType),
                Request = requestVM,
                ViewMode = requestVM.ViewMode
            };

            if (!resultDto.OK)
            {
                ModelState.AddErrors(resultDto.Errors);
            }
            else
            {
                bool portfolio = MaaSConfig.Current.Search.ResultType == Domain.Config.General.ResultType.Portfolio;

                var showOwnPage = MaaSConfig.Current.Portfolio.ShowOwnPage;

                string GetDetailsUrl(SearchResultItem item)
                {
                    if (portfolio && showOwnPage)
                        return Url.ActionLocalized("PortfolioItemPublicProfile", "Sellers", new { id = item.FriendlyUrl });

                    return Url.ActionLocalized("PublicProfile", "Sellers", new { nickname = item.Handle }) + (portfolio ? $"#portfolio-modal-{item.Id}" : "");
                }

                resultVM.TotalItems = resultDto.Result.TotalItems;
                resultVM.Items = resultDto.Result.Items.Select(x => new SearchResultItemVM
                {
                    Id = x.Id,
                    DetailsUrl = GetDetailsUrl(x),
                    ImageUrl = ImgHelper.Avatar(x.ImageUrl, 300),
                    Title = x.Title,
                    Subtitle = requestDto.Type == Domain.DTO.SearchRequestType.Portfolio ? x.Subtitle : null,
                    Description = x.Description,
                    Price = x.Price,
                    Distance = x.Distance,
                    LocationLat = x.LocationLat,
                    LocationLng = x.LocationLng,
                }).ToList();

                if (MaaSConfig.Current.Seller.UserRatings.Enabled && MaaSConfig.Current.Search.ResultType == Domain.Config.General.ResultType.Seller)
                {
                    foreach (var resultSeller in resultVM.Items)
                    {
                        resultSeller.AverageRating = _ratingAppService.GetAverageRating(resultSeller.Id, out int count);
                        resultSeller.RatingsCount = count;
                    }
                }
            }

            if (includeFilters)
            {
                resultVM.Categories = _categoryService.ListActiveCategories();
                resultVM.SubCategories = _categoryService.ListActiveSubCategories();
            }

            return resultVM;
        }

        private SearchResultVM SearchInternal_SmartSearch(SearchRequestVM requestVM, bool includeFilters, Domain.DTO.SearchRequestType searchType)
        {
            var config = MaaSConfig.Current;

            #region Ensure requested ViewMode is enabled

            if (requestVM.ViewMode == SearchResultViewMode.List && !config.Search.List.Enabled)
                requestVM.ViewMode = SearchResultViewMode.Map;

            if (requestVM.ViewMode == SearchResultViewMode.Map && !config.Search.Map.Enabled)
                requestVM.ViewMode = SearchResultViewMode.List;

            #endregion Ensure requested ViewMode is enabled

            var commonRequest = requestVM.ToCommonSearchRequest();
            var commonResult = _searchAppService.Search(commonRequest);

            var resultVM = new SearchResultVM
            {
                NextPageUrl = BuildNextPageUrl(requestVM.ViewMode, requestVM.Page, commonResult.TotalCount, searchType),
                Request = requestVM,
                ViewMode = requestVM.ViewMode
            };

            var isPortfolio = MaaSConfig.Current.Search.ResultType == Domain.Config.General.ResultType.Portfolio;

            resultVM.TotalItems = commonResult.TotalCount;
            resultVM.Items = commonResult.Items.Select(x => new SearchResultItemVM
            {
                Id = x.Id,
                DetailsUrl = Url.ActionLocalized("PublicProfile", "Sellers", new { nickname = x.Handle }) + (isPortfolio ? $"#portfolio-modal-{x.Id}" : ""),
                ImageUrl = ImgHelper.Avatar(x.ImageUrl, 300),
                Title = x.Title,
                Subtitle = commonRequest.Type == SearchRequestType.Portfolio ? x.Subtitle : null,
                Description = x.Description,
                Price = x.Price,
                Distance = x.Distance,
                LocationLat = x.LocationLat,
                LocationLng = x.LocationLng,
            }).ToList();

            if (MaaSConfig.Current.Seller.UserRatings.Enabled && MaaSConfig.Current.Search.ResultType == Domain.Config.General.ResultType.Seller)
            {
                foreach (var resultSeller in resultVM.Items)
                {
                    resultSeller.AverageRating = _ratingAppService.GetAverageRating(resultSeller.Id, out int count);
                    resultSeller.RatingsCount = count;
                }
            }

            if (includeFilters)
            {
                resultVM.Categories = _categoryService.ListActiveCategories();
                resultVM.SubCategories = _categoryService.ListActiveSubCategories();
            }

            return resultVM;
        }
    }
}