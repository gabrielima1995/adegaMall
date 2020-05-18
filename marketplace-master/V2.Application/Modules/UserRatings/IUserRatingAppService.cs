using Application.Modules.Ratings.Models;
using Domain.Entities;
using Framework;
using System;
using System.Collections.Generic;

namespace Application.Modules.Ratings
{
    public interface IUserRatingAppService
    {
        OperationResult<Guid> NewPendingRating(string senderId, string ratedId, string targetRole, string orderId = null, string quoteId = null);

        OperationResult CompleteRating(string ratingId, string comment, int rating);

        double GetAverageRating(string userId, out int ratingsCount);

        List<UserRating> GetLastRatings(int amount, string userId);

        OperationResult HideRating(string ratingId);

        OperationResult AssignRatingPairs(string ratingId1, string ratingId2);

        UserRatingsOverviewModel GetSellerRatingsInfo(string sellerId, int page);
    }
}
