using Domain.DTO;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.StoreModule.Interfaces
{
    public interface IRentalLockQueries
    {
        IList<LockProductRentalDto> ListLocksProductRental(Guid productId, string sellerId, DateTime periodStartUTC, DateTime periodEndUTC, MarketplaceTimeZone timeZone, bool onlyFullyUnavailable = true);
    }
}