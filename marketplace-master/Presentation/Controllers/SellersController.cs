using Application.Config;
using Application.Modules.Portfolio.Search;
using Application.Modules.Ratings;
using Application.Modules.Ratings.Models;
using Application.Sellers;
using DevTrends.MvcDonutCaching;
using Domain.DTO;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Portfolio;
using Domain.Services;
using Presentation.Models;
using Presentation.Models.PortfolioViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.Search.Products;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Modules.Portfolio;

namespace Presentation.Controllers
{
    public class SellersController : BaseController
    {
        private const string PortfolioItemViewPathFmt = "~/Views/Sellers/PortfolioItem/{0}.cshtml";

        private readonly SellerAppService sellerAppService;
        private readonly SellerService sellerService;
        private readonly ISellerRepository sellerRepository;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly IUserRatingAppService ratingAppService;
        private readonly IPersonRepository personRepository;
        private readonly IPortfolioQueries portfolioQueries;
        private readonly IPortfolioRepository portfolioRepository;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly IScheduleTimelineQueries _scheduleTimelineQueries;
        private readonly IProductSearchService _productSearchService;

        public SellersController(
            SellerAppService sellerAppService,
            SellerService sellerService,
            ISellerRepository sellerRepository,
            ICustomFieldAppService customFieldAppService,
            IUserRatingAppService ratingAppService,
            IPersonRepository personRepository,
            IPortfolioQueries portfolioQueries,
            IPortfolioRepository portfolioRepository,
            ITimeZoneProvider timeZoneProvider,
            IScheduleTimelineQueries scheduleTimelineQueries,
            IProductSearchService productSearchService)
        {
            this.sellerAppService = sellerAppService;
            this.sellerService = sellerService;
            this.sellerRepository = sellerRepository;
            this.customFieldAppService = customFieldAppService;
            this.ratingAppService = ratingAppService;
            this.personRepository = personRepository;
            this.portfolioQueries = portfolioQueries;
            this.portfolioRepository = portfolioRepository;
            _timeZoneProvider = timeZoneProvider;
            _scheduleTimelineQueries = scheduleTimelineQueries;
            _productSearchService = productSearchService;
        }

        [DonutOutputCache(CacheProfile = "PublicProfilePage")]
        public ActionResult PublicProfile(string nickname)
        {
            var seller = sellerService.GetByNickname(nickname) ?? sellerRepository.GetByFriendlyUrl(nickname);
            if (seller == null || !seller.Enabled || seller.RequireModeration.GetValueOrDefault()) return HttpNotFound();

            var profileData = sellerService.GetPublicProfile(seller.Id);

            if (profileData.Seller == null)
            {
                var invisibleSellerVM = new PublicProfileInvisibleVM { Seller = seller };
                return View("PublicProfileInvisible", invisibleSellerVM);
            }

            var viewModel = new PublicProfileVM(profileData);

            if (profileData.Products.Count > 8)
            {
                viewModel.Products = viewModel.Products.Take(8).ToList();
                viewModel.ShowLinkForMoreProducts = true;
                viewModel.LinkForMoreProducts = Url.ActionLocalized("Search", "Products", new { SellerId = profileData.Seller.Id });
            }

            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
            {
                viewModel.AverageRating = ratingAppService.GetAverageRating(seller.Id, out int count);
                viewModel.RatingsCount = count;
                viewModel.LatestRatings = ratingAppService.GetLastRatings(3, seller.Id).Select(x =>
                {
                    var person = personRepository.GetById(x.RatingSenderId);
                    var model = new UserRatingViewModel
                    {
                        CreatedAt = x.CreatedAt,
                        RatingMessage = x.Comment,
                        RatingValue = x.Rating.Value,
                        SenderName = person.Name,
                        SenderAvatar = person.GetAvatarUrl(60, 60),
                    };

                    return model;
                }).ToList();
            }

            if (MaaSConfig.Current.CustomFields.Seller.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(viewModel);

            if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
                foreach (var item in viewModel.Portfolio.Items)
                    customFieldAppService.ReadCustomFieldsToViewModel(item);

            return View(viewModel);
        }

        public ActionResult Search(SearchRequestViewModel request)
        {
            if (MaaSConfig.Current.Search.RequireLogin && !Request.IsAuthenticated)
            {
                var url = Url.ActionLocalized("Login", "Account", new { logintosearch = 1 });
                return Redirect(url);
            }

            var response = sellerAppService.Search(request);
            SetNextResults(request, response);
            return View(response);
        }

        public ActionResult SearchResult(SearchRequestViewModel request)
        {
            var response = sellerAppService.SearchOnlyResults(request);
            SetNextResults(request, response);

            return response.Results.Any() ?
                PartialView("_SearchResult", response) :
                this.NoContent();
        }

        public ActionResult Selector()
        {
            var response = sellerRepository.ListForSelector();
            return Content(response.ToJson(), "application/json");
        }

        protected override IEnumerable<FileUpload> GetFileUploads()
        {
            var fileUploads = new List<FileUpload>();

            foreach (var fileName in Request.Files.AllKeys)
            {
                if (fileName != "quoteAttachment")
                    continue;

                var file = Request.Files[fileName];
                if (file.ContentLength == 0)
                    continue;

                fileUploads.Add(new FileUpload(file.InputStream, file.FileName, file.ContentType));
            }

            return fileUploads;
        }

        private void SetNextResults(SearchRequestViewModel request, SearchResultViewModel response)
        {
            if (response.Results.Count == request.Limit)
                response.NextResults = Url.ActionLocalized("SearchResult", "Sellers") + "&" + request.GetNextUrlParams();
        }

        [DonutOutputCache(CacheProfile = "PortfolioItemPage")]
        [Route("Portfolio/Item/{id}")]
        public ActionResult PortfolioItemPublicProfile(string id)
        {
            if (!TryResolvePortfolioItemId(id, out var portfolioItemId))
                return HttpNotFound();

            var portfolioItem = portfolioRepository.GetItemForPublicPage(portfolioItemId);
            if (portfolioItem == null) return this.NotFound();

            if (portfolioItem.Portfolio.Seller.RequireModeration.GetValueOrDefault())
                return HttpNotFound();

            var relatedItems = new List<PortfolioItem>();

            if (MaaSConfig.Current.Portfolio.ShowRelatedItems)
                relatedItems = portfolioRepository.GetRelatedItems(portfolioItem.Id, Array.Empty<Guid>());

            var model = new PortfolioItemPageVM(portfolioItem, relatedItems);

            if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(model);

            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
            {
                model.SellerRatingAverage = ratingAppService.GetAverageRating(portfolioItem.Portfolio.SellerId, out int count);
                model.SellerRatingsCount = count;
            }

            return View(string.Format(PortfolioItemViewPathFmt, "PublicPage"), model);
        }

        private bool TryResolvePortfolioItemId(string input, out Guid portfolioItemId)
        {
            portfolioItemId = Guid.Empty;

            if (input == null)
                return false;

            if ((input.Length == 32 || input.Length == 36) && Guid.TryParse(input, out portfolioItemId))
                return true;

            var nguid = portfolioQueries.GetIdByFriendlyUrl(input);
            if (nguid.HasValue)
            {
                portfolioItemId = nguid.Value;
                return true;
            }

            return false;
        }

        #region Schedule

        public ActionResult TimelineResources(string sellerId, string lang)
        {
            var resources = _productSearchService.ScheduleTimelineProfile(sellerId);

            return Content(resources.ToJson(), "application/json", Encoding.UTF8);
        }

        public ActionResult TimelineEvents(DateTime start, DateTime end, string sellerId, string lang)
        {
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startUtc = start.Kind == DateTimeKind.Local ? start.ToUniversalTime() : timeZone.ConvertTimeToUtc(start);
            var endUtc = end.Kind == DateTimeKind.Local ? end.ToUniversalTime() : timeZone.ConvertTimeToUtc(end);

            var events = _scheduleTimelineQueries.ListRentalScheduleTimelineItems(
                null, sellerId,
                startUtc, endUtc, timeZone,
                false, true, true);

            return Content(events.ToJson(), "application/json", Encoding.UTF8);
        }

        #endregion Schedule
    }
}