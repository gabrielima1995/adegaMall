using Domain.Entities;
using Framework;
using System.Collections.Generic;

namespace Domain.Modules.Ratings
{
    public interface IUserRatingsRepository
    {
        List<UserRating> ListRatingsReceivedByUser(string userId);
        List<UserRating> ListRatingsSentByUser(string userId);
        List<UserRating> ListAll();
        List<UserRating> ListRecentByUser(string userId, int offset, int amount);
        UserRating GetById(string ratingId);
        UserRating GetByOrderId(string orderId, string targetId);
        UserRating GetByQuoteId(string quoteId, string targetId);
        OperationResult Add(UserRating userRating);
        OperationResult Update(UserRating userRating);
        OperationResult Delete(string ratingId);
    }
}
