using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.StoreModule.Queries
{
    public interface IOrderQueries
    {
        OrderDetailsForBuyerAndSellerDTO GetOrderDetailsForBuyerAndSeller(Guid orderId);
        IReadOnlyList<string> ListGatewayTransactionsPendingPayment();
    }

    public class OrderDetailsForBuyerAndSellerDTO
    {
        public OrderDetailsForBuyerAndSellerDTO(Order order, IShippingMethodAdapter shippingMethodAdapter, Quote relatedQuote)
        {
            Order = order;
            ShippingMethodAdapter = shippingMethodAdapter;
            RelatedQuote = relatedQuote;
        }
        public Order Order { get; }
        public IShippingMethodAdapter ShippingMethodAdapter { get; }
        public Quote RelatedQuote { get; }
    }
}
