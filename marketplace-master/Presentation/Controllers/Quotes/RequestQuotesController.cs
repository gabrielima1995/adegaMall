using Application.Config;
using Domain.Categories;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.Portfolio;
using Domain.Services;
using Domain.Validation.Specifications;
using Framework;
using Framework.Geography;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Presentation.Globalization;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using V2.Application.Common;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.QuoteRequests;
using V2.Application.Modules.QuoteRequests.AppCommands;
using V2.Application.Modules.Search.Sellers;
using V2.Application.Modules.UserAccounts;

namespace Presentation.Controllers.Quotes
{
    [RoutePrefix("quotes")]
    public class RequestQuotesController : BaseController
    {
        private readonly MaaSConfig Config = MaaSConfig.Current;
        private const string ViewPathFormat = "~/Views/Quotes/{0}.cshtml";

        private readonly IBuyerRepository buyerRepository;
        private readonly BuyerService buyerService;
        private readonly CategoryService categoryService;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly QuoteAppService quoteAppService;
        private readonly ISellerSearchService searchService;
        private readonly SellerService sellerService;
        private readonly PortfolioItemService portfolioItemService;
        private readonly ApplicationSignInManager signInManager;
        private readonly IMediator mediator;

        public RequestQuotesController(
            IBuyerRepository buyerRepository,
            BuyerService buyerService,
            CategoryService categoryService,
            ICustomFieldAppService customFieldAppService,
            QuoteAppService quoteAppService,
            ISellerSearchService searchService,
            SellerService sellerService,
            PortfolioItemService portfolioItemService,
            ApplicationSignInManager signInManager,
            IMediator mediator)
        {
            this.buyerRepository = buyerRepository;
            this.buyerService = buyerService;
            this.categoryService = categoryService;
            this.customFieldAppService = customFieldAppService;
            this.quoteAppService = quoteAppService;
            this.searchService = searchService;
            this.sellerService = sellerService;
            this.portfolioItemService = portfolioItemService;
            this.signInManager = signInManager;
            this.mediator = mediator;
        }

        private bool CheckIfCanRequestQuote()
        {
            if (!User.Identity.IsAuthenticated) return true;

            var loginProfile = User.Identity.GetLoginProfile();

            var isSatisfied = new CompleteBuyerSpecification().CanRequestQuote(loginProfile);

            if (!isSatisfied)
                this.SetErrorMessage(Request.GetLocalizedString("BUYER_REQUEST_QUOTE_DISABLED"));

            return isSatisfied;
        }

        [Route("request")]
        [HttpGet]
        public ActionResult Get(string sellerId = null, Guid? portfolioItemId = null)
        {
            if (!(Config.Quote.Enabled && (Config.Quote.GeneralQuote.Enabled || Config.Quote.SpecificQuote.Enabled)))
                return this.NotFound();

            CheckIfCanRequestQuote();

            var model = GetQuoteViewModelWithBuyerData(sellerId, portfolioItemId);

            if (model.SellerId != null && !sellerService.HasValidSubscription(model.SellerId))
                return Redirect(Url.ActionLocalized("Index", "Home"));

            SetViewBagForQuoteRequest(model);

            return View(string.Format(ViewPathFormat, "Request"), model);
        }

        [Route("requestfromsearch")]
        [HttpGet]
        public ActionResult GetFromSearch(string searchQueryString)
        {
            if (!(Config.Quote.Enabled && Config.Quote.GeneralQuote.Enabled && Config.Quote.RequestQuoteToSearchedSellers))
                return HttpNotFound();

            CheckIfCanRequestQuote();

            var model = GetQuoteViewModelWithBuyerData(null, null);

            PopulateFromSearchQueryString(model);
            SetViewBagForQuoteRequest(model);

            return View(string.Format(ViewPathFormat, "Request"), model);
        }

        [Route("request")]
        [HttpPost]
        public ActionResult Post(QuoteVM model)
        {
            var profile = GetLoginProfile();
            model.IsLoggedInAs(profile.IsSellerOrSellerAssistant(), profile.IsBuyer());

            var canRequest = CheckIfCanRequestQuote();

            if (canRequest && ModelState.IsValid)
            {
                var command = CreateRequestQuoteAppCommand(model);
                var commandResult = quoteAppService.RegisterQuoteRequest(command);

                if (commandResult.OK)
                {
                    HttpContext.Items["QuoteSellerName"] = model.SellerName ?? "";
                    return RedirectToAction("RequestConfirmed", "Quotes");
                }

                ModelState.AddErrors(commandResult);
            }

            PopulateFromSearchQueryString(model);
            SetViewBagForQuoteRequest(model);
            return View(string.Format(ViewPathFormat, "Request"), model);
        }

        [Route("requestconfirmed")]
        [HttpGet]
        public ActionResult RequestConfirmed()
        {
            ViewBag.SellerName = HttpContext.Items["QuoteSellerName"] ?? "";
            return View(string.Format(ViewPathFormat, "RequestConfirmed"));
        }

        [Route("registerBuyerLead")]
        [HttpPost]
        public async Task<JsonResult> RegisterUser(UserRegistrationData data)
        {
            var entity = buyerRepository.GetByEmail(data.Email);
            if (entity == null)
            {
                var userAndProfile = BuyerUserAndProfile.Create(UserOrPersonId.Generate(), data.Email, data.Name,
                    requireModeration: MaaSConfig.Current.Buyer.Moderation.Enabled);

                var registration = await mediator.Send(new RegisterBuyerUserAndProfile(userAndProfile, data.Password));

                if (registration.IsFailure)
                    return Json(new { success = false, errors = registration.Errors });

                entity = userAndProfile.Profile;
                await signInManager.SignInAsync(userAndProfile.User, false, false);
            }
            else
            {
                var passwordSignInResult = await HttpContext.GetOwinContext().Get<ApplicationSignInManager>()
                    .PasswordSignInAsync(data.Email, data.Password, false, false);

                if (passwordSignInResult == SignInStatus.Failure)
                    return Json(new { success = false, errors = new ErrorDictionary("", Request.GetLocalizedString("LOGIN_FAILURE_MESSAGE")) });
            }

            return Json(new { success = true, id = entity.Id });
        }

        private QuoteVM GetQuoteViewModelWithBuyerData(string sellerId, Guid? portfolioItemId)
        {
            var model = new QuoteVM();

            if (User.Identity.IsAuthenticated && GetLoginProfile().IsBuyer())
            {
                var profile = GetLoginProfile();
                model.WithBuyer(profile);
            }

            if (portfolioItemId.HasValue)
            {
                var portfolioItem = portfolioItemService.Get(
                    i => i.Id == portfolioItemId,
                    i => i.Portfolio);

                model.WithPortfolioItem(portfolioItem);
                sellerId = portfolioItem.Portfolio.SellerId;
                model.Title = Request.GetLocalizedString("QUOTE_REQUEST_PORTFOLIO_TITLE_LABEL") + " " + portfolioItem.Title;
            }

            if (!string.IsNullOrEmpty(sellerId))
            {
                var seller = sellerService.GetById(sellerId);
                model.WithSeller(seller);
            }

            return model;
        }

        private RequestQuoteAppCommand CreateRequestQuoteAppCommand(QuoteVM model)
        {
            GeoBounds bounds = null;
            GeoCoordinate coords = null;

            if (model.ShowGeneralQuoteCity)
            {
                if (double.TryParse(model.BoundsAY, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double ay) &&
                    double.TryParse(model.BoundsAX, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double ax) &&
                    double.TryParse(model.BoundsBY, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double by) &&
                    double.TryParse(model.BoundsBX, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double bx))
                    bounds = new GeoBounds(new GeoCoordinate(ay, ax), new GeoCoordinate(by, bx));
                if (double.TryParse(model.Lat, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(model.Lng, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double lng))
                    coords = new GeoCoordinate(lat, lng);
            }

            List<string> sellerGuids = new List<string>();

            if (model.IsSearchQuoteRequest)
            {
                var queryString = model.SearchQueryString.FromBase64() ?? "";
                var queryStringCollection = HttpUtility.ParseQueryString(queryString);
                var searchRequest = new SearchRequestVM(queryStringCollection).ToCommonSearchRequest();
                searchRequest.PageSize = 100;
                searchRequest.StartIndex = 0;
                sellerGuids = searchService.Search(searchRequest).Items.Select(x => x.Id).ToList();
            }

            return new RequestQuoteAppCommand
            {
                Buyer = new TransientBuyer(model.BuyerName, model.BuyerEmail)
                {
                    Password = model.NewPassword,
                },
                Address = new TransientAddress
                {
                    Street = model.AddressStreet,
                    Number = model.AddressNumber,
                    Complement = null,
                    Neighborhood = model.AddressNeighborhood,
                    CityAndState = model.CityAndState,
                    ZipCode = model.AddressZipCode
                },
                SellerId = model.SellerId,
                Title = model.Title,
                Description = model.Description,
                Phone = model.Phone,
                Date = model.Date,
                Preference = model.Preference,
                Amount = model.Amount,
                CategoryId = model.CategoryId,
                SubCategoryId = model.SubCategoryId,
                Attachments = GetFileUploads(),
                LocationBounds = bounds,
                LocationCoords = coords,
                SearchSellerIds = sellerGuids,
                IsGeneralQuoteRequest = model.IsGeneralQuoteRequest,
                IsShortQuote = model.ShortQuoteEnabled,
                PortfolioItemId = model.PortfolioItemId,
                Quantity = model.Quantity ?? 0,
                Id = model.Id,
                CustomFields = model.CustomFields
            };
        }

        private void PopulateFromSearchQueryString(QuoteVM model)
        {
            var qs = Request["searchQueryString"];
            if (string.IsNullOrEmpty(qs)) return;

            model.SearchQueryString = qs ?? "query=".ToBase64();

            var queryString = model.SearchQueryString.FromBase64() ?? "query=";
            var queryStringCollection = HttpUtility.ParseQueryString(queryString);
            var searchRequest = new SearchRequestVM(queryStringCollection).ToCommonSearchRequest();
            searchRequest.PageSize = 3;
            searchRequest.StartIndex = 0;

            if (int.TryParse(queryStringCollection["Category"], out int categoryId))
                model.CategoryId = categoryId;

            if (int.TryParse(queryStringCollection["SubCategory"], out int subcategoryId))
                model.SubCategoryId = subcategoryId;

            var result = searchService.Search(searchRequest);
            model.SearchSellerCount = result.TotalCount;
            model.SearchSellerAvatars = result.Items.Select(x => x.ImageUrl).ToList();
        }

        private void SetViewBagForQuoteRequest(QuoteVM quote)
        {
            if (quote == null)
                quote = new QuoteVM();

            customFieldAppService.AddEmptyCustomFields(quote);

            var categories = quote.SellerId == null
                ? categoryService.ListActiveCategories()
                : categoryService.ListCategoriesServicedBySeller(quote.SellerId);

            var subCategories = new List<SubCategory>();
            if (quote?.CategoryId != null)
            {
                var category = categories.SingleOrDefault(cat => cat.Id == quote.CategoryId.Value);
                subCategories = category?.SubCategories ?? subCategories;
            }

            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", quote?.CategoryId);
            ViewBag.SubCategoryId = new SelectList(subCategories, "Id", "Name", quote?.SubCategoryId);
        }
    }

    public class UserRegistrationData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}