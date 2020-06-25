using Domain.DTO;
using Domain.Entities.StoreModule;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IOrderRepository : ICrudRepository<Order>
    {
        Order GetDetails(Guid orderId);

        bool CheckStatusCompleted_NoDispute_OrDisputeClosed(Guid orderId);

        List<Order> ListByCartId(Guid cartId);

        List<Order> ListByTransactionId(Guid trasactionId);

        List<Order> ListForAdmin();

        List<Order> ListForRentalStartReminderNotification(double periodReminderInHours);

        List<Order> ListForRentalEndReminderNotification(double periodReminderInHours);

        Tuple<IList<OrderListing>, int> ListForPage(int start, int limit, DateTime? startDate = null, DateTime? endDate = null, string sellerId = null, string buyerId = null);

        OrderListing GetForPage(Guid orderId, string sellerId = null);

        List<OrderProductRentalDto> ListProductRentalByDates(Guid productId, DateTime periodStartUTC, DateTime periodEndUTC, bool onlyNotReturned, Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId, Guid? productAvailabilityId);
       
        List<OrderProductRentalDto> ListBlocksForProductInPeriod(Product product, DateTime periodStartUTC, DateTime periodEndUTC, Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId);

        List<Order> ListByUpcomingCompletion(int days);
    }
}