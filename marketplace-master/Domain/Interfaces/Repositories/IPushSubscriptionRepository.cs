using Domain.Entities;
using Framework;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IPushSubscriptionRepository
    {
        MarketplacePushSubscription GetByToken(string token, string userId);

        IEnumerable<MarketplacePushSubscription> ListByUserId(string userId);

        IEnumerable<MarketplacePushSubscription> ListByRoles(string role);

        IEnumerable<MarketplacePushSubscription> ListByToken(string token);

        int CountByRole(string role);

        OperationResult AddSubscription(MarketplacePushSubscription subscription);

        OperationResult RemoveSubscription(MarketplacePushSubscription subscription);

        OperationResult RemoveSubscription(string token);
    }
}