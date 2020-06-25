using Application.Config;
using Application.Modules.Ratings;
using Domain.Interfaces.Repositories;
using Domain.Modules.Ratings.Queries;
using Domain.Services;
using NewContext.Core;
using NewContext.Queries.Buyers;
using Presentation.Areas.Admin.Views.Buyers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class BuyersController : Controller
    {
        private readonly BuyerService buyerService;
        private readonly IBuyerRepository buyerRepository;
        private readonly IUserRatingAppService ratingAppService;
        private readonly IRatingsQueries ratingsQueries;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly IMessagesDispatcher messages;

        public BuyersController(
            BuyerService buyerService, IBuyerRepository buyerRepository,
            IUserRatingAppService ratingAppService,
            IRatingsQueries ratingsQueries, ICustomFieldAppService customFieldAppService,
            IMessagesDispatcher messages)
        {
            this.buyerService = buyerService;
            this.buyerRepository = buyerRepository;
            this.ratingAppService = ratingAppService;
            this.ratingsQueries = ratingsQueries;
            this.customFieldAppService = customFieldAppService;
            this.messages = messages;
        }

        public async Task<ActionResult> Index(string mode = null)
        {
            ListBuyersForAdmin query;

            if ("moderation".Equals(mode, StringComparison.OrdinalIgnoreCase))
            {
                query = new ListBuyersForAdmin(pendingModeration: true);
            }
            else if ("inactive".Equals(mode, StringComparison.OrdinalIgnoreCase))
            {
                query = new ListBuyersForAdmin(enabled: false, pendingModeration: MaaSConfig.Current.Buyer.Moderation.Enabled ? (bool?)false : null);
            }
            else
            {
                query = new ListBuyersForAdmin(enabled: true, pendingModeration: MaaSConfig.Current.Buyer.Moderation.Enabled ? (bool?)false : null);
            }

            var buyers = await messages.QueryAsync(query);

            return View(new IndexModel(buyers.Select(buyer => BuyerListItemModel.From(buyer))));
        }

        public ActionResult Details(string id)
        {
            if (id == null) return HttpNotFound();

            var buyer = buyerService.GetDetailsForAdmin(id);
            if (buyer == null) return HttpNotFound();

            if (MaaSConfig.Current.Buyer.UserRatings.Enabled)
            {
                ViewData.Add("AverageRating", ratingAppService.GetAverageRating(id, out int count));
                ViewData.Add("RatingsCount", count);

                var ratings = ratingsQueries.ListForAdmin(0, 100, out int num, id);
                ratings = ratings.Where(x => x.Rating.HasValue).ToList();
                ViewData.Add("RatingsList", ratings);
            }

            var model = new BuyerModel { Buyer = buyer };

            if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(model);

            return View(model);
        }

        public ActionResult ApproveModeration(string buyerguid)
        {
            var buyer = buyerRepository.GetById(buyerguid);

            if (buyer == null) return HttpNotFound();

            buyerService.ApproveModeration(buyer);

            return RedirectToAction("Details", new { id = buyerguid });
        }

        public ActionResult ChangeEnabled(string id, bool enabled)
        {
            if (id == null) return HttpNotFound();

            buyerService.SetEnabledStatus(id, enabled);

            return RedirectToAction("Details", "Buyers", new { Id = id });
        }
    }
}