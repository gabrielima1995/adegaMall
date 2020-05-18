using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace V2.Infrastructure.Modules.PushNotifications
{
    public class PushSubscriptionRepository : IPushSubscriptionRepository
    {
        private readonly ScaffoldContext _dbContext;

        public PushSubscriptionRepository(ScaffoldContext context)
        {
            _dbContext = context;
        }

        public OperationResult AddSubscription(MarketplacePushSubscription subscription)
        {
            try
            {
                //Remove previous subscriptions for different users with same token:
                var previousSubs = ListByToken(subscription.Token).Where(x => x.UserId != subscription.UserId);
                foreach (var sub in previousSubs)
                {
                    RemoveSubscription(sub);
                }

                _dbContext.PushSubscriptions.Add(subscription);
                _dbContext.SaveChanges();
            }
            catch
            {
                return OperationResult.Fail("Error adding subscription");
            }

            return OperationResult.Success;
        }

        public int CountByRole(string role)
        {
            return _dbContext.PushSubscriptions.DistinctBy(x => x.UserId).Count(x => x.UserRole == role);
        }

        public MarketplacePushSubscription GetByToken(string token, string userId)
        {
            return _dbContext.PushSubscriptions.FirstOrDefault(x => x.Token == token && x.UserId == userId);
        }

        public IEnumerable<MarketplacePushSubscription> ListByRoles(string role)
        {
            return _dbContext.PushSubscriptions.Where(x => x.UserRole == role).DistinctBy(x => x.Token).ToList();
        }

        public IEnumerable<MarketplacePushSubscription> ListByToken(string token)
        {
            return _dbContext.PushSubscriptions.Where(x => x.Token == token).ToList();
        }

        public IEnumerable<MarketplacePushSubscription> ListByUserId(string userId)
        {
            return _dbContext.PushSubscriptions.Where(x => x.UserId == userId).DistinctBy(x => x.Token).ToList();
        }

        public OperationResult RemoveSubscription(string token)
        {
            try
            {
                var entity = _dbContext.PushSubscriptions.First(x => x.Token == token);
                var entry = _dbContext.Entry(entity);
                entry.State = EntityState.Deleted;
                _dbContext.SaveChanges();
            }
            catch
            {
                return OperationResult.Fail("Error removing subscription");
            }

            return OperationResult.Success;
        }

        public OperationResult RemoveSubscription(MarketplacePushSubscription subscription)
        {
            try
            {
                var entry = _dbContext.Entry(subscription);
                entry.State = EntityState.Deleted;
                _dbContext.SaveChanges();
            }
            catch
            {
                return OperationResult.Fail("Error removing subscription");
            }

            return OperationResult.Success;
        }
    }
}