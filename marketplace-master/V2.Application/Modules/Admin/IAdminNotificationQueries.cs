using Domain.Entities;
using System.Collections.Generic;

namespace V2.Application.Modules.Admin
{
    public interface IAdminNotificationQueries
    {
        List<MarketplacePushSubscription> GetDataForNotification(NotificationType notificationType);
    }

    public enum NotificationType
    {
        AllSellers,
        EnabledSellers,
        SellersPendingModeration,

        AllBuyers,
        EnabledBuyers,
        BuyerPendingModeration
    }
}
