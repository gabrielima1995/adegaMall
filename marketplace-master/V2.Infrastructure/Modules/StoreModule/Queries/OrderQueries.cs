using Dapper;
using Domain.Entities.PortfolioModule;
using Infrastructure.Data.Context;
using Infrastructure.Shipping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.StoreModule.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private readonly ScaffoldContext context;
        private readonly ShippingService shippingService;

        public OrderQueries(ScaffoldContext context, ShippingService shippingService)
        {
            this.context = context;
            this.shippingService = shippingService;
        }

        public OrderDetailsForBuyerAndSellerDTO GetOrderDetailsForBuyerAndSeller(Guid orderId)
        {
            var order = context.Orders
                .Include(x => x.Buyer)
                .Include(x => x.Histories)
                .Include(x => x.Histories.Select(h => h.Person))
                .Include(x => x.Products)
                .Include(x => x.Products.Select(p => p.Product))
                .Include(x => x.Products.Select(j => j.RentalsInfo))
                .Include(x => x.Reviews)
                .Include(x => x.Seller)
                .Include(x => x.Seller.Address)
                .Include(x => x.ShippingAddress)
                .Include(x => x.Transaction)
                .FirstOrDefault(x => x.Id == orderId);

            if (order == null)
                return null;

            Quote quote = null;
            order.Histories = order.Histories.OrderBy(x => x.OccurredOn).ToList();
            order.Products = order.Products.OrderBy(x => x.Item).ToList();

            if (order.Products.Any(prod => prod.Product.IsBillingProduct.GetValueOrDefault()))
            {
                var quoteId = context.QuoteBillings.Where(billing => billing.OrderId == orderId).Select(billing => billing.QuoteId).FirstOrDefault();
                if (quoteId != Guid.Empty)
                    quote = context.Quotes.Find(quoteId);
            }

            var adapter = shippingService.GetAdapter(order.ShippingMethod);
            return new OrderDetailsForBuyerAndSellerDTO(order, adapter, quote);
        }

        public IReadOnlyList<string> ListGatewayTransactionsPendingPayment()
        {
            var sql = @"
                SELECT OrderTransaction.TransactionId
                FROM [Order]
                INNER JOIN OrderTransaction ON [Order].Transaction_Id = OrderTransaction.Id
                WHERE [Order].[Status] in (6,0) /* 0 = aguardando pagamento, 6 = pedido processado */";

            using (var conn = MyDbConnection.Create())
            {
                var transactionIds = conn.Query<string>(sql);
                return transactionIds.ToList();
            }
        }
    }
}
