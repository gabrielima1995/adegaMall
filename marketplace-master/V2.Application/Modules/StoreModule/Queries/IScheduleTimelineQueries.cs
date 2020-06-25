using System;
using System.Collections.Generic;
using Framework;

namespace V2.Application.Modules.StoreModule.Queries
{
    public interface IScheduleTimelineQueries
    {
        List<RentalScheduleTimelineResourceItem> ListRentalScheduleTimelineResources(Guid? productId, string sellerId);

        List<RentalScheduleItem> ListRentalScheduleTimelineItems(Guid? productId, string sellerId,
            DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone,
            bool showLocks = true, bool forceMinStartDate = false, bool shortData = false);
    }
}