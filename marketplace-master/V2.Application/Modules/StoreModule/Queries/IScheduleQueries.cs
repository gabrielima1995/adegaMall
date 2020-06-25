using Framework;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.StoreModule.Queries
{
    public interface IScheduleQueries
    {
        IList<ProductWithScheduleDto> ListProductsWithRentalsAndLocks(string sellerId);

        IList<RentalScheduleItem> ListRentalScheduleItems(Guid? productId, string sellerId, DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone, bool onlyRentals = false, Guid? productAvailabilityId = null);

        IList<RentalScheduleItem> ListLockRentalItems(Guid productId, string sellerId, DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone, bool onlyFullyUnavailable = true, bool onlyByProduct = false);

        IList<RentalScheduleItem> ListRentalScheduleBuyerItems(Guid? productId, string buyerId, DateTime periodStartUtc, DateTime periodEndUtc, MarketplaceTimeZone timeZone);
    }
}