using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Framework.Geography;
using Newtonsoft.Json;
using Presentation.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using V2.Application.Modules.Search;

namespace Presentation.Models
{
    public enum SearchResultViewMode
    {
        List = 0,
        Map
    }

    public class SearchRequestVM
    {
        public double? BoundsEndLat { get; set; }
        public double? BoundsEndLng { get; set; }
        public double? BoundsOriginLat { get; set; }
        public double? BoundsOriginLng { get; set; }
        public int? CategoryId { get; set; }
        public string LocationAddress { get; set; }
        public double? LocationLat { get; set; }
        public double? LocationLng { get; set; }
        public string LocationName { get; set; }
        public bool OnlyOnSitePickup { get; set; }
        public int Page { get; set; } = 1;
        public string Query { get; set; }
        public Domain.DTO.SearchRequestType RequestType { get; set; } = Domain.DTO.SearchRequestType.Seller;
        public Domain.DTO.SearchSorting Sorting { get; set; }
        public int? SubCategoryId { get; set; }
        public SearchResultViewMode ViewMode { get; set; } = SearchResultViewMode.Map;
        public string ActionUrl
        {
            get
            {
                var urlRequested = HttpContext.Current.Request.Url.LocalPath.ToLower();
                if(urlRequested.Contains("sellers"))
                {
                    return "Sellers";
                }
                else if (urlRequested.Contains("portfolios"))
                {
                        return "Portfolios";
                }
                else
                {
                    return MaaSConfig.Current.Search.ResultType.ToString() + "s";
                }
            }
        }

        public SearchRequestVM()
        {
        }

        public SearchRequestVM(NameValueCollection queryString)
        {
            Query = queryString["query"];

            if (queryString["category"] != null)
            {
                int categoryId;
                if (int.TryParse(queryString["category"], out categoryId))
                    CategoryId = categoryId;
            }

            if (queryString["subcategory"] != null)
            {
                int subCategoryId;
                if (int.TryParse(queryString["subcategory"], out subCategoryId))
                    SubCategoryId = subCategoryId;
            }

            if (queryString["onlyOnSitePickup"] != null)
            {
                var onSitePickup = false;
                if (bool.TryParse(queryString["onlyOnSitePickup"], out onSitePickup))
                    OnlyOnSitePickup = onSitePickup;
            }

            LocationName = queryString["location"];
            LocationAddress = queryString["address"];

            LocationLat = ParseCoordinateValue(queryString["lat"]);
            LocationLng = ParseCoordinateValue(queryString["lng"]);

            BoundsOriginLat = ParseCoordinateValue(queryString["boundsay"]);
            BoundsOriginLng = ParseCoordinateValue(queryString["boundsax"]);
            BoundsEndLat = ParseCoordinateValue(queryString["boundsby"]);
            BoundsEndLng = ParseCoordinateValue(queryString["boundsbx"]);

            if (queryString["mode"] != null)
            {
                SearchResultViewMode viewMode;
                if (Enum.TryParse(queryString["mode"], out viewMode))
                    ViewMode = viewMode;
            }

            if (queryString["page"] != null)
            {
                int page;
                int.TryParse(queryString["page"], out page);
                Page = Math.Max(page, 1);
            }

            if (queryString["sort"] != null)
            {
                Domain.DTO.SearchSorting sorting;
                if (Enum.TryParse(queryString["sort"], true, out sorting))
                    Sorting = sorting;
            }

            switch (MaaSConfig.Current.Search.ResultType)
            {
                default:
                case Domain.Config.General.ResultType.Seller:
                    RequestType = Domain.DTO.SearchRequestType.Seller;
                    break;

                case Domain.Config.General.ResultType.Product:
                    RequestType = Domain.DTO.SearchRequestType.Product;
                    break;

                case Domain.Config.General.ResultType.Portfolio:
                    RequestType = Domain.DTO.SearchRequestType.Portfolio;
                    break;
            }
        }

        public CommonSearchRequest ToCommonSearchRequest()
        {
            var common = new CommonSearchRequest
            {
                Query = Query,
                CategoryId = CategoryId,
                SubCategoryId = SubCategoryId,
                LocationRange = MaaSConfig.Current.Search.Map.SearchRange,
                PageSize = ViewMode == SearchResultViewMode.Map ? 100 : MaaSConfig.Current.Search.List.PageSize,
                Sorting = (V2.Application.Modules.Search.SearchSorting)Enum.Parse(typeof(V2.Application.Modules.Search.SearchSorting),
                    Sorting.ToString(), false),
                Type = (V2.Application.Modules.Search.SearchRequestType)Enum.Parse(typeof(V2.Application.Modules.Search.SearchRequestType),
                    RequestType.ToString(), false)
            };

            common.StartIndex = common.PageSize * (Page - 1);

            common.Bounds = BoundsOriginLat.HasValue && BoundsOriginLng.HasValue && BoundsEndLng.HasValue && BoundsEndLng.HasValue ?
                new GeoBounds(new GeoCoordinate(BoundsOriginLat.Value, BoundsOriginLng.Value), new GeoCoordinate(BoundsEndLat.Value, BoundsEndLng.Value)) : null;

            common.Location = LocationLat.HasValue && LocationLng.HasValue ?
                new GeoCoordinate(LocationLat.Value, LocationLng.Value) : common.Bounds?.GetCenter();

            if (common.Location == null && common.Sorting == V2.Application.Modules.Search.SearchSorting.LowerDistance)
                common.Sorting = V2.Application.Modules.Search.SearchSorting.MostRelevant;

            return common;
        }

        public SearchRequest ToSearchRequestDto()
        {
            var dto = new SearchRequest
            {
                Query = Query,
                CategoryId = CategoryId,
                SubCategoryId = SubCategoryId,
                LocationRange = MaaSConfig.Current.Search.Map.SearchRange,
                Page = Page,
                PageSize = ViewMode == SearchResultViewMode.Map ? 100 : MaaSConfig.Current.Search.List.PageSize,
                Sorting = Sorting,
                Type = RequestType
            };

            dto.Bounds = BoundsOriginLat.HasValue && BoundsOriginLng.HasValue && BoundsEndLng.HasValue && BoundsEndLng.HasValue
                ? new GeoBounds(
                    new GeoCoordinate(BoundsOriginLat.Value, BoundsOriginLng.Value),
                    new GeoCoordinate(BoundsEndLat.Value, BoundsEndLng.Value)) : null;

            dto.Location = LocationLat.HasValue && LocationLng.HasValue
                ? new GeoCoordinate(LocationLat.Value, LocationLng.Value) : null;

            return dto;
        }

        private double? ParseCoordinateValue(string input)
        {
            double parsed;

            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed)
                ? parsed : default(double?);
        }
    }

    public class SearchResultItemVM
    {
        private string _description;

        public double? AverageRating { get; set; }

        public string Description
        {
            get => _description?.Truncate(MaaSConfig.Current.Search.DescriptionMaxLength, "");
            set => _description = value;
        }

        public string DetailsUrl { get; set; }
        public double? Distance { get; set; }

        public string FriendlyDistance
        {
            get
            {
                if (Distance == null) return null;
                if (Distance.Value <= 1000) return $"{(int)Distance.Value} m";
                return $"{(Distance.Value / 1000).ToString("N1")} km";
            }
        }

        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public double? LocationLat { get; set; }
        public double? LocationLng { get; set; }
        public MapLinksVM MapLinks => new MapLinksVM(Title, null, LocationLat, LocationLng);
        public string NextPageUrl { get; set; }
        public decimal? Price { get; set; }
        public int? RatingsCount { get; set; }
        public string Subtitle { get; set; }
        public string Title { get; set; }
        public bool ShowDescription => MaaSConfig.Current.Search.ShowDescription;
    }

    public class SearchResultVM : IWithCategory, IWithSubCategory
    {
        public IList<Category> Categories { get; set; }

        // Helpers
        public bool HasMultipleViewModes
        {
            get
            {
                var config = MaaSConfig.Current;
                return config.Search.Map.Enabled && config.Search.List.Enabled;
            }
        }

        public List<SearchResultItemVM> Items { get; set; }
        public string ItemsJson => JsonUtils.EscapeJsonString(JsonConvert.SerializeObject(Items));
        public string Lang { get; set; }
        public string NextPageUrl { get; set; }
        public SearchRequestVM Request { get; set; }

        public SelectList SortOptions
        {
            get
            {
                var list = new List<Domain.DTO.SearchSorting>
                {
                    Domain.DTO.SearchSorting.MostRelevant,
                    Domain.DTO.SearchSorting.Alphabetical,
                    Domain.DTO.SearchSorting.Newer
                };

                if (Request.RequestType == Domain.DTO.SearchRequestType.Product)
                {
                    list.Add(Domain.DTO.SearchSorting.LowerPrice);
                    list.Add(Domain.DTO.SearchSorting.HigherPrice);
                }

                if (Request.RequestType == Domain.DTO.SearchRequestType.Seller)
                {
                    if (MaaSConfig.Current.Search.SearchFields.Location)
                        list.Add(Domain.DTO.SearchSorting.LowerDistance);
                }

                var options = list.Select(x => new
                {
                    Key = x.ToString(),
                    Value = x.GetDescriptionLocalized()
                });

                return new SelectList(options, "Key", "Value", Request.Sorting);
            }
        }

        public IList<SubCategory> SubCategories { get; set; }
        public int TotalItems { get; set; }
        public SearchResultViewMode ViewMode { get; set; }

        // Interface members.
        public int? Category => Request.CategoryId;

        public int? SubCategory => Request.SubCategoryId;
    }
}