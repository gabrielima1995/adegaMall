using Application.Config;
using Application.Modules.Ratings;
using Domain.Modules.Ratings;
using System;
using System.Linq;

namespace V2.Application.Modules.UserRatings
{
    public class CheckExpiredRatings
    {
        private readonly IUserRatingAppService _ratingAppService;
        private readonly IUserRatingsRepository _ratingsRepository;

        public CheckExpiredRatings(IUserRatingAppService ratingAppService,
                                   IUserRatingsRepository ratingsRepository)
        {
            _ratingAppService = ratingAppService;
            _ratingsRepository = ratingsRepository;
        }

        public void RunJob()
        {
            var incompleteRatings = _ratingsRepository.ListAll().Where(x => !x.IsCompleted).ToList();

            foreach (var rating in incompleteRatings)
            {
                DateTime expirationDate = rating.CreatedAt.AddDays(MaaSConfig.Current.Ratings.MaxWaitDays);
                if (expirationDate < DateTime.UtcNow)
                {
                    rating.IsCompleted = true;
                    _ratingsRepository.Update(rating);
                }
            }
        }
    }
}
