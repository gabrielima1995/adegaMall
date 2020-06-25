using Application.Config;
using Application.Modules.Ratings;
using Domain.Interfaces.Repositories;
using Domain.Modules.Ratings;
using Presentation.Models;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class UserRatingController : BaseController
    {
        private readonly IPersonRepository personRepository;
        private readonly IUserRatingAppService ratingAppService;
        private readonly IUserRatingsRepository ratingRepository;

        public UserRatingController(
            IUserRatingAppService ratingAppService,
            IUserRatingsRepository ratingRepository,
            IPersonRepository personRepository)
        {
            this.ratingAppService = ratingAppService;
            this.ratingRepository = ratingRepository;
            this.personRepository = personRepository;
        }

        public ActionResult Overview(string sellerId, int page = 0)
        {
            var model = ratingAppService.GetSellerRatingsInfo(sellerId, page);

            if (model == null)
                return HttpNotFound();

            if (model.LatestRatingsList.Count == 0)
                ViewBag.DoNotIndex = true;

            return View("Overview", model);
        }

        [AuthorizeBuyerOrSeller]
        public ActionResult SendRating(string ratingId)
        {
            if (ratingId == null)
                return HttpNotFound();

            var rating = ratingRepository.GetById(ratingId);

            if (rating == null || rating.Rating.HasValue)
                return HttpNotFound();

            if (GetPersonId() != rating.RatingSenderId)
                return HttpNotFound();

            var target = personRepository.GetById(rating.RatingTargetId);

            if (target == null || rating.IsCompleted)
                return HttpNotFound();

            return View("SendRating", new CompleteRatingViewModel()
            {
                Rating = new CompleteRatingModel()
                {
                    RatingId = ratingId,
                },
                RatingOrderId = rating.OrderId.ToString(),
                RatingQuoteId = rating.QuoteId.ToString(),
                TargetAvatar = Application.Sellers.SellerHelper.GetAvatarUrl(target.Avatar, 300, 300),
                TargetName = target.AlternativeName ?? target.FullName,
                ExpirationDate = rating.CreatedAt.AddDays(MaaSConfig.Current.Ratings.MaxWaitDays),
            });
        }

        [HttpPost]
        public ActionResult SendRating(CompleteRatingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ratingAppService.CompleteRating(viewModel.Rating.RatingId, viewModel.Rating.Comment, viewModel.Rating.RatingValue.GetValueOrDefault());
                return View("RatingReceived");
            }
            return SendRating(viewModel.Rating.RatingId);
        }
    }
}