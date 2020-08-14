using Application.Addresses;
using Application.Config;
using Application.Modules.Products;
using Application.Modules.Products.Queries;
using Application.Modules.Products.Search;
using Application.Modules.Ratings;
using DevTrends.MvcDonutCaching;
using Domain.DTO;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Domain.Services.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using Geocoding;
using Newtonsoft.Json;
using Presentation.Globalization;
using Presentation.Models;
using Presentation.Models.ProductViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;
using V2.Application.Modules;
using V2.Application.Modules.General.CustomFields;
using Address = Domain.Entities.Address;

namespace Presentation.Controllers.Products
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ProductsController : BaseController
    {
        private readonly ITimeZoneProvider userTimeZoneProvider;
        private readonly IGeocoder _geocoder;
        private readonly CartService cartService;
        private readonly IProductAppService productAppService;
        private readonly IProductQueries productQueries;
        private readonly IShippingService shippingService;
        private readonly ProductService productService;
        private readonly SellerService sellerService;
        private readonly RentalService rentalService;
        private readonly IUserRatingAppService ratingAppService;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly ProductSmartSearchAppService searchAppService;
        private readonly IPersonRepository personRepository;
        private readonly IRepository<ProductCategory> productCategoryRepository;
        private readonly IRepository<ProductVariant> productVariantRepository;
        private readonly BuyerService buyerService;

        public ProductsController(
            ITimeZoneProvider userTimeZoneProvider,
            IGeocoder geocoder,
            CartService cartService,
            IProductAppService productAppService,
            IProductQueries productQueries,
            IShippingService shippingService,
            ProductService productService,
            SellerService sellerService,
            RentalService rentalService,
            IUserRatingAppService ratingAppService,
            ICustomFieldAppService customFieldAppService,
            ProductSmartSearchAppService searchAppService,
            IPersonRepository personRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductVariant> productVariantRepository,
            BuyerService buyerService)
        {
            this.userTimeZoneProvider = userTimeZoneProvider;
            _geocoder = geocoder;
            this.cartService = cartService;
            this.productAppService = productAppService;
            this.productQueries = productQueries;
            this.shippingService = shippingService;
            this.productService = productService;
            this.sellerService = sellerService;
            this.rentalService = rentalService;
            this.ratingAppService = ratingAppService;
            this.customFieldAppService = customFieldAppService;
            this.searchAppService = searchAppService;
            this.personRepository = personRepository;
            this.productCategoryRepository = productCategoryRepository;
            this.productVariantRepository = productVariantRepository;
            this.buyerService = buyerService;
        }

        [DonutOutputCache(CacheProfile = "CategoryPage")]
        [Route("categoria/{categorySlug}", Name = "CategoryPageRoute")]
        [Route("categoria/{categorySlug}/{variantTypeSlug}/{variantSlug}", Name = "SubcategoryPageRoute")]
        public ActionResult CategoryPage(SearchRequestViewModel request)
        {
            if (!string.IsNullOrEmpty(request.CategorySlug))
            {
                var category = productCategoryRepository.Get(
                    i => i.Slug == request.CategorySlug);

                if (category != null)
                {
                    ViewBag.Title = $"{category.Name}";
                    ViewBag.Description = $"Encontre as melhores bebidas de {category.Name.ToLower()}";
                }

                if (!string.IsNullOrEmpty(request.VariantSlug))
                {
                    var variant = productVariantRepository.Get(
                        i => i.Slug == request.VariantSlug,
                        x => x.Type);

                    if (variant != null)
                    {
                        ViewBag.Title += $" por {variant.Type.Name.ToLower()} - {variant.Name}";
                        ViewBag.Description += $" por {variant.Type.Name.ToLower()} - {variant.Name}";
                    }
                }
            }

            return Search(request);
        }

        [DonutOutputCache(CacheProfile = "SearchResults")]
        public ActionResult Search(SearchRequestViewModel request)
        {
            if (MaaSConfig.Current.Search.RequireLogin && !Request.IsAuthenticated)
            {
                var url = Url.ActionLocalized("Login", "Account", new { logintosearch = 1 });
                return Redirect(url);
            }

            if (string.IsNullOrWhiteSpace(request.SellerId) && MaaSConfig.Current.Search.RequireCategoryForProductSearch && !request.Category.HasValue)
                return RedirectToAction("Index", "Home", new { err = "1" });

            var hasNoLocation = request.GetLocationOrNull() == null;
            var sortTypes = new Dictionary<int, string>
            {
                { (int)ProductSortType.MostRelevant, Request.GetLocalizedString(ProductSortType.MostRelevant) },
                { (int)ProductSortType.Newer, Request.GetLocalizedString(ProductSortType.Newer) },
                { (int)ProductSortType.LowerPrice, Request.GetLocalizedString(ProductSortType.LowerPrice) },
                { (int)ProductSortType.HigherPrice, Request.GetLocalizedString(ProductSortType.HigherPrice) },
                { (int)ProductSortType.Alphabetical, Request.GetLocalizedString(ProductSortType.Alphabetical) },
                { (int)ProductSortType.Distance, hasNoLocation ? Request.GetLocalizedString("SEARCH_SORT_LOWER_DISTANCE_DISABLED") : Request.GetLocalizedString(ProductSortType.Distance) }
            };

            if (hasNoLocation && request.SortType == ProductSortType.Distance)
                request.SortType = ProductSortType.MostRelevant;

            var disabledValues = hasNoLocation ? new[] { (int)ProductSortType.Distance } : Array.Empty<int>();
            ViewBag.SortType = new SelectList(sortTypes, "Key", "Value", (int)request.SortType, disabledValues);

            var response = searchAppService.Search(request, GetPersonId());

            if (request.SellerId != null)
                response.FilteredBySeller = sellerService.GetById(request.SellerId);

            SetNextResults(request, response);
            return View("Search", response);
        }

        [DonutOutputCache(CacheProfile = "SearchResults")]
        public ActionResult SearchResult(SearchRequestViewModel request)
        {
            var response = searchAppService.SearchOnlyResults(request, GetPersonId());

            SetNextResults(request, response);

            return response.Results.Any() ?
                PartialView("_SearchResult", response) :
                this.NoContent();
        }

        [DonutOutputCache(CacheProfile = "ProductPage")]
        public ActionResult PublicPage(string id, DateTime? start = null, string time = null, Guid? productAvailability = null)
        {
            if (!TryResolveProductId(id, out Guid productId))
                return HttpNotFound();

            var productInfo = productService.GetDetailsForPublicPage(productId);
            if (productInfo == null) return this.NotFound();

            if (productInfo.Product.Store.Seller.RequireModeration.GetValueOrDefault())
                return HttpNotFound();

            var model = new ProductPageVM(productInfo);

            if (MaaSConfig.Current.Product.ShowRelatedProducts)
                model.RelatedProducts = productAppService.GetRelatedProducts(productInfo.Product.Id, new[] { productInfo.Product.Id });

            if (MaaSConfig.Current.CustomFields.Product.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(model);

            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
            {
                model.SellerRatingAverage = ratingAppService.GetAverageRating(productInfo.Product.Store.SellerId, out int count);
                model.SellerRatingsCount = count;
            }

            if (MaaSConfig.Current.CustomFields.CartItem.Enabled)
            {
                model.CartItemCustomFields = new CartItemCustomFields();
                customFieldAppService.ReadCustomFieldsToViewModel(model.CartItemCustomFields);
            }

            //var disabledValues = hasNoLocation ? new[] { (int)ProductSortType.Distance } : Array.Empty<int>();
            var variants = model.GetStockVariants();
            var variantsDictonary = new Dictionary<int, string>();
            List<int> disableValues = new List<int>();
            foreach (var item in variants)
            {
                if (item.HasStock)
                {
                    variantsDictonary.Add(item.Variant.Id, item.Variant.Name);
                }
                else
                {
                    disableValues.Add(item.Variant.Id);
                    variantsDictonary.Add(item.Variant.Id, $"{item.Variant.Name} {Request.GetLocalizedString("CART_PRODUCT_STOCK_OUT")}");
                }
            }
            //SelectList(IEnumerable items, object selectedValue, IEnumerable disabledValues);
            ViewBag.StockVariant = new SelectList(variantsDictonary, "Key", "Value", variantsDictonary, disabledValues: disableValues);

            ViewBag.StartDate = start;
            ViewBag.StartTime = time;
            ViewBag.ProductAvailabilityId = productAvailability;

            return View(model);
        }

        public ActionResult LoadingShippingInfos(string id, string zipCode)
        {
            if (!TryResolveProductId(id, out Guid productId)) return new EmptyResult();

            var productInfo = productService.GetDetailsForPublicPage(productId);
            var sellerAddress = productInfo.Product.Store.Seller.Address;

            Address shippingAddress = null;
            ShippingInfosVM shippingInfos = null;

            #region Get shipping address

            if (string.IsNullOrEmpty(zipCode))
            {
                if (User.Identity.IsAuthenticated &&
                    GetLoginProfile().CurrentPersonType == LoginProfilePersonType.Buyer)
                {
                    var buyer = buyerService.GetWithAddresses(GetPersonId());

                    if (buyer != null) shippingAddress = buyer.Address;
                }
            }
            else
            {
                shippingAddress = new Address
                {
                    ZipCode = zipCode
                };

                _geocoder.UpdateLocation(shippingAddress);
            }

            #endregion Get shipping address

            #region Get shipping infos

            if (shippingAddress != null && !shippingAddress.Location.IsEmpty)
            {
                ViewBag.ZipCode = shippingAddress.ZipCode;

                shippingInfos = new ShippingInfosVM(productInfo.Product, sellerAddress, shippingAddress,
                    shippingService.GetAdapter(ShippingMethod.ActualNoShipping),
                    shippingService.GetAdapter(ShippingMethod.NoShipping),
                    shippingService.GetAdapter(ShippingMethod.OnSitePickup),
                    shippingService.GetAdapter(ShippingMethod.PrivateFleet),
                    shippingService.GetAdapter(ShippingMethod.PostalExpress),
                    shippingService.GetAdapter(ShippingMethod.PostalRegular),
                    shippingService.GetAdapter(ShippingMethod.AsapLog),
                    shippingService.GetAdapter(ShippingMethod.TotalExpress));
            }

            #endregion Get shipping infos

            return PartialView("_ShippingProduct", shippingInfos);
        }

        private bool TryResolveProductId(string input, out Guid productId)
        {
            productId = Guid.Empty;

            if (input == null)
                return false;

            if ((input.Length == 32 || input.Length == 36) && Guid.TryParse(input, out productId))
                return true;

            var nguid = productQueries.GetIdByFriendlyUrl(input);
            if (nguid.HasValue)
            {
                productId = nguid.Value;
                return true;
            }

            return false;
        }

        public PartialViewResult PublicPageScript(Guid productId)
        {
            return PartialView("_PublicPageScript", productId);
        }

        public ActionResult DoubleFreightWarning(Guid productId)
        {
            if (User.Identity.IsAuthenticated && GetLoginProfile().IsBuyer())
            {
                var buyerId = GetPersonId();
                var displayWarning = cartService.ProductWouldCauseDoubleFreight(buyerId, productId);

                if (displayWarning)
                    return PartialView("_DoubleFreightWarningPartial");
            }

            return Content("");
        }

        [HttpGet]
        [ActionName("RentalCalendar")]
        public JsonResult GetCalendar(
            Guid productId,
            int year, int month, int day,
            Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId, Guid? productAvailabilityId,
            int quantity = 1, bool forceSelectPeriod = false, bool showDiscount = true, DateTime? start = null, string time = null)
        {
            string GenerateDateUrl(DateTime date) => Url.Action("RentalCalendar", "Products", new { productId, year = date.Year, month = date.Month, day = date.Day, ignoreOrderProductId, ignoreOrderProductRentalInfoId, quantity, productAvailabilityId, forceSelectPeriod });

            var product = productService.GetById(productId);

            var timeZone = userTimeZoneProvider.GetTimeZone();
            var incomingDate = new DateTime(year, month, day);
            var startDate = incomingDate.AddDays((double)incomingDate.DayOfWeek * -1);

            var rentalCalendarResult = rentalService.GetRentalCalendar(product, quantity,
                startDate, timeZone, GenerateDateUrl,
                ignoreOrderProductId, ignoreOrderProductRentalInfoId, productAvailabilityId);

            ViewBag.MonthOptions = new MonthOptionsBuilder(startDate, GenerateDateUrl).Build();
            ViewBag.Quantity = quantity;
            ViewBag.ForceSelectPeriod = forceSelectPeriod;
            ViewBag.IgnoreOrderProductId = ignoreOrderProductId;
            ViewBag.IgnoreOrderProductRentalInfoId = ignoreOrderProductRentalInfoId;
            ViewBag.ProductAvailabilityId = productAvailabilityId;
            ViewBag.ShowDiscount = showDiscount;
            ViewBag.StartDate = start;
            ViewBag.StartTime = time;

            var html = PartialView("~/Views/Products/_RentalCalendar.cshtml", rentalCalendarResult.Result).RenderToString();
            return Json(new { html }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("RentalGrid")]
        public ActionResult GetRentalGrid(
            Guid productId,
            int year, int month, int day,
            Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId, Guid? productAvailabilityId,
            int quantity = 1, bool forceSelectPeriod = false, bool showDiscount = true, DateTime? start = null, string time = null)
        {
            var product = productService.GetById(productId);

            if (!MaaSConfig.Current.Product.BusinessModel.Rental.Enabled ||
                (product.RentalSettings.RentalPeriodUnit != RentalPeriodUnit.Hour && product.RentalSettings.RentalPeriodUnit != RentalPeriodUnit.Minute) ||
                !MaaSConfig.Current.Product.BusinessModel.Rental.FixedPeriodRental.Enabled ||
                !MaaSConfig.Current.Product.BusinessModel.Rental.GridView.Enabled ||
                !product.RentalSettings.IsFixedRental)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var timeZone = userTimeZoneProvider.GetTimeZone();
            var startDate = new DateTime(year, month, day, 0, 0, 0);

            var rentalGridResult = rentalService.GetRentalGrid(product, quantity,
                startDate, timeZone,
                ignoreOrderProductId, ignoreOrderProductRentalInfoId, productAvailabilityId);

            ViewBag.Quantity = quantity;
            ViewBag.ForceSelectPeriod = forceSelectPeriod;
            ViewBag.IgnoreOrderProductId = ignoreOrderProductId;
            ViewBag.IgnoreOrderProductRentalInfoId = ignoreOrderProductRentalInfoId;
            ViewBag.ProductAvailabilityId = productAvailabilityId;
            ViewBag.ShowDiscount = showDiscount;
            ViewBag.StartDate = start;
            ViewBag.StartTime = time;

            var html = PartialView("~/Views/Products/_RentalGridView.cshtml", rentalGridResult.Result).RenderToString();
            return Json(new { html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateAvailabilityForRental(
            Guid productId,
            int quantity,
            DateTime? startDateTime, DateTime? endDateTime,
            string lang,
            Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId, Guid? productAvailabilityId)
        {
            var product = productService.GetById(productId);

            if (product.RentalSettings.IsFixedRental && startDateTime.HasValue)
            {
                endDateTime = product.RentalSettings.CalculateEndDateTime(startDateTime);
            }

            var hasAllParams =
                productId != Guid.Empty &&
                startDateTime.HasValue &&
                endDateTime.HasValue;

            if (!hasAllParams)
                return Json(new RentalAvailabilityJson(false, Request.GetLocalizedString("RENTAL_ENTER_ALL_PARAMS_MESSAGE")));

            var timeZone = userTimeZoneProvider.GetTimeZone();

            var rentalStartUTC = timeZone.ConvertTimeToUtc(startDateTime.Value);
            var rentalEndUTC = timeZone.ConvertTimeToUtc(endDateTime.Value);

            var validation = rentalService.CheckAvailability(product, quantity,
                rentalStartUTC, rentalEndUTC, timeZone, ignoreOrderProductId, ignoreOrderProductRentalInfoId, productAvailabilityId);

            if (validation.OK)
                return Json(new RentalAvailabilityJson(true, validation.Result.UnitsAvailableMessage()));

            return Json(
                new RentalAvailabilityJson(false, validation.Errors.FirstOrDefault().Value));
        }

        [HttpGet]
        public ActionResult Nearby(
            NearbyProductsRequestViewModel request)
        {
            if (!MaaSConfig.Current.Product.Enabled ||
                !MaaSConfig.Current.NearbyProducts.Enabled)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var response = searchAppService.Nearby(request);

            var html = PartialView("~/Views/Products/_NearbyProducts.cshtml", response).RenderToString();
            return Json(new { html }, JsonRequestBehavior.AllowGet);
        }

        private void SetNextResults(SearchRequestViewModel request, SearchResultViewModel response)
        {
            if (response.Results.Count == request.Limit)
                response.NextResults = Url.ActionLocalized("SearchResult", "Products") + "&" + request.GetNextUrlParams();
        }

        private class RentalAvailabilityJson
        {
            [JsonProperty("isAvailable")]
            public bool IsAvaiable { get; set; }

            [JsonProperty("availabilityMessage")]
            public string AvailabilityMessage { get; set; }

            public RentalAvailabilityJson(bool isAvailable, string message)
            {
                IsAvaiable = isAvailable;
                AvailabilityMessage = message;
            }
        }
    }
}