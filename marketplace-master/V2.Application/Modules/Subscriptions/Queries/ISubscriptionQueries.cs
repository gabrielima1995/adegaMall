using System.Collections.Generic;
using V2.Application.Modules.Subscriptions.Model;

namespace V2.Application.Modules.Subscriptions.Queries
{
    public interface ISubscriptionQueries
    {
        List<SubscriptionListItemForAdminDto> ListSubscriptions();
    }
}