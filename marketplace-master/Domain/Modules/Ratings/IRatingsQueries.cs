using System.Collections.Generic;

namespace Domain.Modules.Ratings.Queries
{
    public interface IRatingsQueries
    {
        IReadOnlyCollection<UserRatingModel> ListForAdmin(int startIndex, int pageSize, out int count, string targetUserId = null);
        IReadOnlyCollection<UserRatingDto> ListUserRatings(IEnumerable<string> userIds);
    }

    public class UserRatingDto
    {
        public string UserId { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }
}