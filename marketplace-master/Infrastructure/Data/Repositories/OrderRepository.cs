using Dapper;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Entities;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class OrderRepository : EfCrudRepository<Order>, IOrderRepository
    {
        public OrderRepository(ScaffoldContext context) : base(context)
        {
        }

        public Order GetDetails(Guid orderId)
        {
            var result = CacheHelper.Get(Context.Orders
                .Include(x => x.Coupon)
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
                .Where(x => x.Id == orderId));

            if (result != null)
            {
                result.Histories = result.Histories.OrderBy(x => x.OccurredOn).ToList();
                result.Products = result.Products.OrderBy(x => x.Item).ToList();
            }

            return result;
        }

        public List<Order> ListByCartId(Guid cartId)
        {
            return CacheHelper.List(Context.Orders
                .Include(x => x.Coupon)
                .Include(x => x.Transaction)
                .Include(x => x.Buyer)
                .Include(x => x.Seller)
                .Include(x => x.Products)
                .Include(x => x.ShippingAddress)
                .Where(x => x.CartId == cartId || x.Id == cartId));
        }

        public List<Order> ListByTransactionId(Guid transactionId)
        {
            return CacheHelper.List(Context.Orders.Where(x => x.Transaction.Id == transactionId));
        }

        public List<Order> ListForAdmin()
        {
            var cache = new CacheDefinition("__OrderRepository_ListForAdmin", new TimeSpan(0, 0, 25));

            return CacheHelper.List(Context.Orders
                .Include(x => x.Coupon)
                .Include(x => x.Buyer)
                .Include(x => x.Seller)
                .Include(x => x.Products)
                .Include(x => x.ShippingAddress)
                .OrderByDescending(x => x.CreatedAt), cache);
        }

        public List<Order> ListForRentalStartReminderNotification(double periodReminderInHours)
        {
            var aboutToStart = DateTime.UtcNow.AddHours(periodReminderInHours);

            return Context.Set<Order>()
                .Include(x => x.Buyer)
                .Include(x => x.Seller)
                .Include(x => x.Seller.Address)
                .Include(x => x.Products)
                .Include(x => x.Products.Select(j => j.RentalsInfo))
                .Where(x => x.BusinessModel == ProductBusinessModel.Rental)
                .Where(x => x.RentalInfo.StartReminderSent == null || x.RentalInfo.StartReminderSent == false)
                .Where(x =>
                    x.Status == OrderStatus.CompletedPayment ||
                    x.Status == OrderStatus.AwaitingPickup ||
                    x.Status == OrderStatus.AwaitingDelivery ||
                    x.Status == OrderStatus.DeliveryOnTheWay)
                .Where(x => x.Products.Any(j => j.RentalInfo.StartDate <= aboutToStart))
                .ToList();
        }

        public List<Order> ListForRentalEndReminderNotification(double periodReminderInHours)
        {
            var aboutToEnd = DateTime.UtcNow.AddHours(periodReminderInHours);

            return Context.Set<Order>()
                .Include(x => x.Buyer)
                .Include(x => x.Seller)
                .Include(x => x.Seller.Address)
                .Include(x => x.Products)
                .Where(x => x.BusinessModel == ProductBusinessModel.Rental)
                .Where(x => x.RentalInfo.EndReminderSent == null || x.RentalInfo.EndReminderSent == false)
                .Where(x => x.Status == OrderStatus.Delivered)
                .Where(x => x.Products.Any(j => j.RentalInfo.EndDate <= aboutToEnd))
                .ToList();
        }

        private IQueryable<OrderListing> QueryForPage(DateTime? startDate = null, DateTime? endDate = null, string sellerId = null, string buyerId = null)
        {
            return Context.Set<Order>()
                .Include(x => x.Coupon)
                .Include(x => x.Transaction)
                .Include(x => x.Buyer)
                .Include(x => x.Seller)
                .Include(x => x.Seller.Address)
                .Include(x => x.ShippingAddress)
                .Include(x => x.Histories)
                .Include(x => x.Products)
                .Include(x => x.Products.Select(p => p.Product))
                .Where(x => startDate == null || x.CreatedAt >= startDate)
                .Where(x => endDate == null || x.CreatedAt <= endDate)
                .Where(x => sellerId == null || x.Seller.Id == sellerId)
                .Where(x => buyerId == null || x.Buyer.Id == buyerId)
                .Select(x => new OrderListing
                {
                    Id = x.Id,
                    OrderGroupId = x.CartId,
                    CreatedAt = x.CreatedAt,
                    ShippingMethod = x.ShippingMethod,
                    Shipping = x.Shipping,
                    Discount = x.Discount,
                    Status = x.Status,
                    Payment = new OrderPaymentListing
                    {
                        Method = x.PaymentMethod,
                        Installments = x.Transaction == null ? 0 : x.Transaction.Installments,
                        IsPaid = x.IsPaid
                    },
                    Buyer = new OrderBuyerListing
                    {
                        Id = x.Buyer.Id,
                        FullName = x.Buyer.FullName,
                        Email = x.Buyer.Email,
                        CpfCnpj = x.Buyer.DocumentNumber,
                        PhoneNumber = x.Buyer.PhoneNumber
                    },
                    Seller = new OrderSellerListing
                    {
                        Id = x.Seller.Id,
                        FullName = x.Seller.FullName,
                        AlternativeName = x.Seller.AlternativeName,
                        Email = x.Seller.Email,
                        CpfCnpj = x.Seller.DocumentNumber,
                        PhoneNumber = x.Seller.PhoneNumber
                    },
                    SellerAddress = new AddressListing
                    {
                        Street = x.Seller.Address.Street,
                        Number = x.Seller.Address.Number,
                        AdditionalInfo = x.Seller.Address.AdditionalInfo,
                        Neighborhood = x.Seller.Address.Neighborhood,
                        ZipCode = x.Seller.Address.ZipCode,
                        CityState = x.Seller.Address.CityAndState,
                        Location = x.Seller.Address.Location
                    },
                    ShippingAddress = new AddressListing
                    {
                        Street = x.ShippingAddress.Street,
                        Number = x.ShippingAddress.Number,
                        AdditionalInfo = x.ShippingAddress.AdditionalInfo,
                        Neighborhood = x.ShippingAddress.Neighborhood,
                        ZipCode = x.ShippingAddress.ZipCode,
                        CityState = x.ShippingAddress.CityAndState,
                        Location = x.ShippingAddress.Location
                    },
                    History = x.Histories.Select(h => new OrderHistoryListing
                    {
                        CreatedAt = h.CreatedAt,
                        Status = h.Status
                    }).OrderBy(h => h.CreatedAt),
                    Items = x.Products.Select(p => new OrderItemListing
                    {
                        Id = p.Id,
                        CustomId = p.Product.CustomId,
                        Name = p.FullItemName,
                        UnitPrice = p.Price,
                        Quantity = p.Quantity,
                        ShippingPrice = p.Shipping
                    })
                });
        }

        public Tuple<IList<OrderListing>, int> ListForPage(int start, int limit, DateTime? startDate = null, DateTime? endDate = null, string sellerId = null, string buyerId = null)
        {
            var query = QueryForPage(startDate, endDate, sellerId, buyerId);

            var data = query
                .OrderByDescending(i => i.CreatedAt)
                .Skip(start)
                .Take(limit)
                .ToList();

            var total = query
                .Count();

            return new Tuple<IList<OrderListing>, int>(data, total);
        }

        public OrderListing GetForPage(Guid orderId, string sellerId = null)
        {
            return QueryForPage(sellerId: sellerId).FirstOrDefault(x => x.Id == orderId);
        }

        public List<OrderProductRentalDto> ListProductRentalByDates(Guid productId, DateTime periodStartUTC, DateTime periodEndUTC, bool onlyNotReturned, Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId, Guid? productAvailabilityId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT DISTINCT
                        O.Id AS OrderId,
                        CASE WHEN OP.[ProductType] = @defaultType THEN OP.RentalInfo_StartDate ELSE RI.StartDate END AS RentalStartDateTime,
                        CASE WHEN OP.[ProductType] = @defaultType THEN OP.RentalInfo_EndDate ELSE RI.EndDate END AS RentalEndDateTime,
                        CASE WHEN OP.[ProductType] = @defaultType THEN OP.Quantity ELSE 1 END AS Quantity,
                        CASE WHEN OP.[ProductType] = @defaultType THEN OP.RentalInfo_ProductAvailability_Id ELSE RI.ProductAvailability_Id END AS ProductAvailabilityId,
                        RI.[Id] AS OrderProductRentalInfoId
                    FROM [dbo].[OrderProduct] OP
                    INNER JOIN [dbo].[Order] O ON O.Id = OP.Order_Id
                    LEFT JOIN [dbo].[OrderProductRentalsInfo] RI ON RI.OrderProduct_Id = OP.Id
                    WHERE O.BusinessModel = @rental
                        -- ignora o agendamento do produto quando o produto for do tipo Padrão
                        AND (@ignoreOrderProductId IS NULL OR OP.[ProductType] <> @defaultType OR OP.[Id] <> @ignoreOrderProductId)
                        -- ignora um determinado agendamento do produto quando o produto for do tipo Pacote de créditos
                        AND (@ignoreOrderProductRentalInfoId IS NULL OR OP.[ProductType] <> @packageType OR RI.[Id] <> @ignoreOrderProductRentalInfoId)
                        AND O.[Status] <> @canceled
                        AND O.[Status] <> @rejected
                        AND (@onlyNotReturned = 0 OR O.[Status] <> @returned)
                        AND OP.Product_Id = @productId
                        -- obtem o agendamento do produto quando o produto for do tipo Padrão
                        AND ((OP.[ProductType] = @defaultType AND 
                                (OP.RentalInfo_StartDate BETWEEN @start AND @end
                                    OR OP.RentalInfo_EndDate BETWEEN @start AND @end
                                    OR (OP.RentalInfo_StartDate <= @start AND OP.RentalInfo_EndDate >= @end)) AND
                                (@productAvailabilityId IS NULL OR OP.RentalInfo_ProductAvailability_Id = @productAvailabilityId)) OR
                        -- obtem os agendamentos do produto quando o produto for do tipo Pacote de créditos
                             (OP.[ProductType] = @packageType AND 
                                (RI.StartDate BETWEEN @start AND @end
                                    OR RI.EndDate BETWEEN @start AND @end
                                    OR (RI.StartDate <= @start AND RI.EndDate >= @end)) AND
                                (@productAvailabilityId IS NULL OR RI.ProductAvailability_Id = @productAvailabilityId)))";
                var query = conn.Query<OrderProductRentalDto>(sql, new
                {
                    rental = ProductBusinessModel.Rental,
                    canceled = OrderStatus.Canceled,
                    rejected = OrderStatus.Rejected,
                    returned = OrderStatus.Returned,
                    defaultType = ProductType.Default,
                    packageType = ProductType.Package,
                    onlyNotReturned,
                    productId,
                    ignoreOrderProductId,
                    ignoreOrderProductRentalInfoId,
                    productAvailabilityId,
                    start = periodStartUTC,
                    end = periodEndUTC
                });

                return query.ToList();
            }
        }

        public List<OrderProductRentalDto> ListBlocksForProductInPeriod(Product product, DateTime periodStartUTC, DateTime periodEndUTC, Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT DISTINCT
                        O.Id AS OrderId,
                        CASE WHEN OP.[ProductType] = @defaultType THEN OP.RentalInfo_StartDate ELSE RI.StartDate END AS RentalStartDateTime,
                        CASE WHEN OP.[ProductType] = @defaultType THEN OP.RentalInfo_EndDate ELSE RI.EndDate END AS RentalEndDateTime,
                        OP.Quantity,
                        P.RentalSettings_BlockScheduleForProduct AS BlockScheduleForProduct
                    FROM [dbo].[OrderProduct] op 
                    JOIN Product p on op.Product_Id =p.Id
                    INNER JOIN [dbo].[Order] O ON O.Id = OP.Order_Id
                    LEFT JOIN [dbo].[OrderProductRentalsInfo] RI ON RI.OrderProduct_Id = OP.Id
                    WHERE p.Store_SellerId = @sellerId                        
                        AND O.BusinessModel = @rental
                        -- ignora o agendamento do produto quando o produto for do tipo Padrão
                        AND (@ignoreOrderProductId IS NULL OR OP.[ProductType] <> @defaultType OR OP.[Id] <> @ignoreOrderProductId)
                        -- ignora um determinado agendamento do produto quando o produto for do tipo Pacote de créditos
                        AND (@ignoreOrderProductRentalInfoId IS NULL OR OP.[ProductType] <> @packageType OR RI.[Id] <> @ignoreOrderProductRentalInfoId)
                        AND O.[Status] <> @canceled
                        AND O.[Status] <> @rejected
                        -- obtem o agendamento do produto quando o produto for do tipo Padrão
                        AND ((OP.[ProductType] = @defaultType AND 
                                (OP.RentalInfo_StartDate BETWEEN @start AND @end
                                    OR OP.RentalInfo_EndDate BETWEEN @start AND @end
                                    OR (OP.RentalInfo_StartDate <= @start AND OP.RentalInfo_EndDate >= @end))) OR
                        -- obtem os agendamentos do produto quando o produto for do tipo Pacote de créditos
                             (OP.[ProductType] = @packageType AND 
                                (RI.StartDate BETWEEN @start AND @end
                                    OR RI.EndDate BETWEEN @start AND @end
                                    OR (RI.StartDate <= @start AND RI.EndDate >= @end))))";

                var query = conn.Query<OrderProductRentalDto>(sql, new
                {
                    rental = ProductBusinessModel.Rental,
                    canceled = OrderStatus.Canceled,
                    rejected = OrderStatus.Rejected,
                    returned = OrderStatus.Returned,
                    defaultType = ProductType.Default,
                    packageType = ProductType.Package,
                    product.Id,
                    ignoreOrderProductId,
                    ignoreOrderProductRentalInfoId,
                    start = periodStartUTC,
                    end = periodEndUTC,
                    sellerId = product.Store_SellerId
                });

                return query.ToList();
            }
        }

        public List<Order> ListByUpcomingCompletion(int days)
        {
            var fromDate = DateTime.Now.AddDays(days);
            var toDate = fromDate.AddDays(1);

            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT [Order].*, Buyer.*, Seller.*
                    FROM [Order]
                    INNER JOIN Person Buyer ON [Order].Buyer_Id = Buyer.Id
                    INNER JOIN Person Seller ON [Order].Seller_Id = Seller.Id
                    LEFT JOIN Dispute ON [Order].Id = Dispute.OrderId
                    WHERE [Order].AutoCompletionDate IS NOT NULL
                        AND Dispute.Id IS NULL /*não pode ter disputa em aberto*/
                        AND [Order].AutoCompletionDate >= @fromDate
                        AND [Order].AutoCompletionDate <= @toDate
                        AND [Order].Status IN (1, 2, 3, 4, 7)";

                var query = conn.Query<Order, Buyer, Seller, Order>(sql,
                    (order, buyer, seller) =>
                    {
                        order.Buyer = buyer;
                        order.Seller = seller;
                        return order;
                    },
                    new { fromDate, toDate }).ToList();

                return query;
            }
        }

        public bool CheckStatusCompleted_NoDispute_OrDisputeClosed(Guid orderId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    --DECLARE @orderId UNIQUEIDENTIFIER SET @orderId = 'c43a19bc-391f-47d6-92f3-cd8123dc0230'
                    SELECT CONVERT(BIT, CASE
                        WHEN [Order].[Status] = 14 /*completed*/
                            AND (Dispute.Id IS NULL OR Dispute.[Status] IN (4 /*ClosedBySeller*/, 5 /*ClosedByBuyer*/, 6 /*ClosedByAdmin*/, 7 /*ClosedForInactivity*/)) THEN 1
                        ELSE 0 END) OrderCompleted_NoDispute_OrDisputeClosed
                    FROM [Order]
                    LEFT JOIN Dispute ON [Order].Id = Dispute.OrderId
                    WHERE [Order].Id = @orderId";

                return conn.ExecuteScalar<bool>(sql, new { orderId });
            }
        }

    }
}