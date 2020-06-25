using Application.Modules.Ratings;
using Domain.Interfaces.Repositories;
using Domain.Modules.Ratings;
using Domain.Modules.Ratings.Queries;
using Presentation.Areas.Admin.Models;
using Presentation.Areas.Admin.Views.Ratings;
using System;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class RatingsController : Controller
    {
        private readonly IPersonRepository personRepository;
        private readonly IUserRatingsRepository ratingsRepository;
        private readonly IUserRatingAppService ratingAppService;
        private readonly IRatingsQueries ratingsQueries;

        public RatingsController(
            IPersonRepository personRepository,
            IUserRatingsRepository ratingsRepository,
            IRatingsQueries ratingsQueries,
            IUserRatingAppService ratingAppService)
        {
            this.personRepository = personRepository;
            this.ratingsRepository = ratingsRepository;
            this.ratingsQueries = ratingsQueries;
            this.ratingAppService = ratingAppService;
        }

        public ActionResult Index(string mode = null)
        {
            var viewMode = "hidden".Equals(mode, StringComparison.OrdinalIgnoreCase)
                ? IndexViewModel.ViewMode.Hidden
                : IndexViewModel.ViewMode.Visible;
            var ratings = ratingsQueries.ListForAdmin(0, 1000, out _);
            return View("Index", new IndexViewModel(ratings, viewMode));
        }

        public ActionResult Details(string id)
        {
            if (id == null)
                return HttpNotFound();

            var rating = ratingsRepository.GetById(id);

            var sender = personRepository.GetById(rating.RatingSenderId);
            var senderAverage = ratingAppService.GetAverageRating(rating.RatingSenderId, out int sCount);

            var target = personRepository.GetById(rating.RatingTargetId);
            var targetAverage = ratingAppService.GetAverageRating(rating.RatingTargetId, out int tCount);

            var model = new UserRatingDetailsViewModel()
            {
                Rating = rating,
                Sender = sender,
                SenderAverageRating = senderAverage,
                SenderRatingsCount = sCount,
                Target = target,
                TargetAverageRating = targetAverage,
                TargetRatingsCount = tCount,
            };

            return View("Details", model);
        }

        public ActionResult ModerateRating(string id, bool hidden)
        {
            var rating = ratingsRepository.GetById(id);
            rating.Hidden = hidden;
            ratingsRepository.Update(rating);
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        [HttpPost]
        public ActionResult DeleteRating(string deleteId)
        {
            ratingsRepository.Delete(deleteId);
            return RedirectToAction("Index");
        }
    }
}