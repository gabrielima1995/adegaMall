using Dapper;
using Domain.Modules.Ratings;
using Domain.Modules.Ratings.Queries;
using System.Collections.Generic;
using System.Linq;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.Ratings.Queries
{
    public class RatingsQueries : IRatingsQueries
    {
        public IReadOnlyCollection<UserRatingModel> ListForAdmin(int startIndex, int pageSize, out int count, string targetUserId = null)
        {
            using (var db = MyDbConnection.Create())
            {
                var script = SqlScriptHelper.GetScript("RatingQueries_ListForAdmin");
                var grid = db.QueryMultiple(script, new { startIndex, pageSize, targetUserId });

                count = grid.ReadSingle<int>();
                return grid.Read<UserRatingModel>().ToList();
            }
        }

        public IReadOnlyCollection<UserRatingDto> ListUserRatings(IEnumerable<string> userIds)
        {
            if (userIds == null || userIds.Count() == 0)
                return new List<UserRatingDto>();

            using (var db = MyDbConnection.Create())
            {
                var sql = @"
                    --DECLARE @userIds VARCHAR(MAX) SET @userIds = 'fcece307-b857-43b8-a171-ac70d2429cf6,8b8d44ba-caba-4a61-b64a-eee3a1f0346f'

                    SELECT RatingTargetId UserId, AVG(Rating) AverageRating, COUNT(1) ReviewsCount
                    FROM UserRating
                    INNER JOIN STRING_SPLIT(@userIds, ',') ON RatingTargetId = Value
                    WHERE Rating IS NOT NULL
                    GROUP BY RatingTargetId";

                var csv = string.Join(",", userIds);
                var result = db.Query<UserRatingDto>(sql, new { userIds = csv });
                return result.ToList();
            }
        }
    }
}