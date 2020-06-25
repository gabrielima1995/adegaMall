using Domain.Entities;
using Domain.Modules.Ratings;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class UserRatingsRepository : IUserRatingsRepository
    {
        private readonly ScaffoldContext db;

        public UserRatingsRepository(ScaffoldContext context)
        {
            db = context;
        }

        public OperationResult Add(UserRating userRating)
        {
            try
            {
                db.UserRatings.Add(userRating);
                db.SaveChanges();
                return OperationResult.Success;
            }
            catch (Exception e)
            {
                return OperationResult.Fail(e.Message);
            }
        }

        public OperationResult Delete(string ratingId)
        {
            var entity = db.UserRatings.Find(new Guid(ratingId));
            db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
            return db.SaveChanges() > 0 ? OperationResult.Success : OperationResult.Fail("Could not delete entity");
        }

        public UserRating GetById(string ratingId)
        {
            return db.UserRatings.Find(new Guid(ratingId));
        }

        public UserRating GetByOrderId(string orderId, string targetId)
        {
            var id = new Guid(orderId);
            return db.UserRatings.FirstOrDefault(x => x.OrderId == id && x.RatingTargetId == targetId);
        }

        public UserRating GetByQuoteId(string quoteId, string targetId)
        {
            var id = new Guid(quoteId);
            return db.UserRatings.FirstOrDefault(x => x.QuoteId == id && x.RatingTargetId == targetId);
        }

        public List<UserRating> ListAll()
        {
            return db.UserRatings.ToList();
        }

        public List<UserRating> ListRatingsReceivedByUser(string userId)
        {
            return db.UserRatings.Where(x => x.RatingTargetId == userId && x.Rating.HasValue).ToList();
        }

        public List<UserRating> ListRatingsSentByUser(string userId)
        {
            return db.UserRatings.Where(x => x.RatingSenderId == userId && x.Rating.HasValue).ToList();
        }

        public List<UserRating> ListRecentByUser(string userId, int offset, int amount)
        {
            try
            {
                return db.UserRatings
                    .Where(x => x.RatingTargetId == userId && !x.Hidden && x.Rating.HasValue && (x.PairedRating == null || x.IsPairCompleted && x.IsCompleted))
                    .OrderByDescending(x => x.CreatedAt).Skip(offset).Take(amount).ToList();
            }
            catch (Exception)
            {
                return new List<UserRating>();
            }
        }

        public OperationResult Update(UserRating userRating)
        {
            var entry = db.Entry(userRating);

            if (entry == null)
                return OperationResult.Fail("Inexistant rating");
            else
            {
                try
                {
                    entry.State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return OperationResult.Success;
                }
                catch (Exception e)
                {
                    return OperationResult.Fail(e.Message);
                }
            }
        }
    }
}
