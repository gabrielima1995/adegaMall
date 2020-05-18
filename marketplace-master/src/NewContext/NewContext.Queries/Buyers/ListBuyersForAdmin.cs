using CSharpFunctionalExtensions;
using Dapper;
using NewContext.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Queries.Buyers
{
    public class ListBuyersForAdmin : IQuery<IReadOnlyCollection<ListBuyersForAdmin.BuyerDto>>
    {
        public ListBuyersForAdmin(bool? enabled = null, bool? pendingModeration = null)
        {
            Enabled = enabled;
            PendingModeration = pendingModeration;
        }

        public bool? Enabled { get; }
        public bool? PendingModeration { get; }

        public class BuyerDto
        {
            public string Id { get; protected set; }
            public string Name { get; protected set; }
            public string Email { get; protected set; }
            public Maybe<string> CityAndState { get; protected set; }
            public int OrdersCount { get; protected set; }
            public decimal OrdersTotalAmount { get; protected set; }
            public int QuotesCount { get; protected set; }
            public DateTime RegisteredAt { get; protected set; }
            public bool Enabled { get; protected set; }
            public bool RequireModeration { get; protected set; }
        }
    }

    public class ListBuyersForAdminHandler : IQueryHandler<ListBuyersForAdmin, IReadOnlyCollection<ListBuyersForAdmin.BuyerDto>>
    {
        private readonly QueriesConnectionString connectionString;

        public ListBuyersForAdminHandler(QueriesConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IReadOnlyCollection<ListBuyersForAdmin.BuyerDto>> HandleAsync(ListBuyersForAdmin query)
        {
            var queryParameters = new
            {
                enabled = query.Enabled,
                pendingModeration = query.PendingModeration
            };

            using (var conn = new SqlConnection(connectionString.Value))
            {
                var data = await conn.QueryAsync<ListBuyersForAdmin.BuyerDto>(sql, queryParameters);
                return data.ToList().AsReadOnly();
            }
        }

        private const string sql = @"
            SELECT
                Buyer.Id,
                Buyer.FullName [Name],
                Buyer.Email,
                [Address].CityAndState,
                COUNT([Order].Id) OrdersCount,
                SUM(([OrderProduct].Quantity * [OrderProduct].Price) + [Order].Shipping + [Order].InstallmentFees - [Order].Discount) OrdersTotalAmount,
                COUNT([Quote].Id) QuotesCount,
                Buyer.CreatedAt RegisteredAt,
                Buyer.[Enabled],
                CONVERT(BIT, ISNULL(Buyer.RequireModeration, 0)) RequireModeration
            FROM Person Buyer
            LEFT JOIN [Address] ON Buyer.Address_Id = [Address].Id
            LEFT JOIN [Order] ON Buyer.Id = [Order].Buyer_Id
            LEFT JOIN OrderProduct ON [Order].Id = OrderProduct.Order_Id
            LEFT JOIN Quote ON Buyer.Id = Quote.Buyer_Id
            WHERE Buyer.Discriminator = 'Buyer'
                AND (@enabled IS NULL OR Buyer.[Enabled] = @enabled)
                AND (@pendingModeration IS NULL OR 
                    (@pendingModeration = 1 AND Buyer.RequireModeration = 1) OR 
                    (@pendingModeration = 0 AND (Buyer.RequireModeration = 0 OR Buyer.RequireModeration IS NULL)))
            GROUP BY
                Buyer.Id,
                Buyer.FullName,
                Buyer.Email,
                [Address].CityAndState,
                Buyer.CreatedAt,
                Buyer.[Enabled],
                CONVERT(BIT, ISNULL(Buyer.RequireModeration, 0))";
    }
}