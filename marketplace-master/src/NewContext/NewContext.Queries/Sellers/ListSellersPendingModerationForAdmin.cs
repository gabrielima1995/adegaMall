using CSharpFunctionalExtensions;
using Dapper;
using Domain.Modules.StoreModule.CompleteProfile;
using NewContext.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Queries.Sellers
{
    public class ListSellersPendingModerationForAdmin : IQuery<IReadOnlyCollection<ListSellersPendingModerationForAdmin.SellerPendingModerationDto>>
    {
        public class SellerPendingModerationDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Avatar { get; set; }
            public string WhatsAppNumber { get; set; }
            public string AlternativeName { get; set; }
            public string Email { get; set; }
            public Maybe<string> CityState { get; set; }
            public bool HasActiveSubscription { get; set; }
            public int OrdersCount { get; set; }
            public decimal OrdersTotalAmount { get; set; }
            public int QuotesCount { get; set; }
            public DateTime RegisteredAt { get; set; }
            public DateTime? ModifiedAt { get; private set; }
            public bool Enabled { get; set; }
            public bool EnableCustomFees { get; set; }
            public bool CustomIsLiable { get; set; }
            public decimal CustomPaymentFeePercent { get; set; }
            public decimal CustomPaymentFeeAmount { get; set; }
            public bool RequireModeration { get; set; }
            public int CategoriesCount { get; set; }
            public int PortfolioItemsCount { get; set; }
            public string SupplierGatewayId { get; set; }
            public string SellerCompleteProfile { get; private set; }
            public SellerCompleteProfile SellerCompleteProfileMapped
            {
                get { return SellerCompleteProfile == null ? null : JsonConvert.DeserializeObject<SellerCompleteProfile>(SellerCompleteProfile); }
                set { SellerCompleteProfile = JsonConvert.SerializeObject(value); }
            }
        }
    }

    public class ListSellersPendingModerationForAdminHandler : IQueryHandler<ListSellersPendingModerationForAdmin, IReadOnlyCollection<ListSellersPendingModerationForAdmin.SellerPendingModerationDto>>
    {
        private readonly QueriesConnectionString connectionString;

        public ListSellersPendingModerationForAdminHandler(QueriesConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IReadOnlyCollection<ListSellersPendingModerationForAdmin.SellerPendingModerationDto>> HandleAsync(ListSellersPendingModerationForAdmin query)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var data = await conn.QueryAsync<ListSellersPendingModerationForAdmin.SellerPendingModerationDto>(sql);
                return data.ToList().AsReadOnly();
            }
        }

        private const string sql = @"
	    SELECT
                Seller.Id,
                Seller.FullName [Name],
                ISNULL(Seller.AlternativeName, Seller.FullName) AlternativeName,
                Seller.Email,
                [Address].CityAndState CityState,
                CONVERT(BIT, CASE WHEN Subscription.[Status] IN (1 /*trialing*/, 2 /*paid*/, 3 /*pending payment*/) THEN 1 ELSE 0 END) HasActiveSubscription,
                COUNT([Order].Id) OrdersCount,
                SUM(([OrderProduct].Quantity * [OrderProduct].Price) + [Order].Shipping + [Order].InstallmentFees - [Order].Discount) OrdersTotalAmount,
                COUNT([Quote].Id) QuotesCount,
                Seller.CreatedAt RegisteredAt,
                Seller.ModifiedAt ModifiedAt,
                Seller.[Enabled],
                CONVERT(BIT, ISNULL(Seller.OverrideGlobalPaymentFees, 0)) [EnableCustomFees],
                CONVERT(BIT, ISNULL(Seller.CustomIsLiable, 0)) CustomIsLiable,
                ISNULL(Seller.CustomPaymentFeePercent, 0) CustomPaymentFeePercent,
                ISNULL(Seller.CustomPaymentFeeAmount, 0) CustomPaymentFeeAmount,
                CONVERT(BIT, ISNULL(Seller.RequireModeration, 0)) RequireModeration,
                COUNT(SubCategory.Id) CategoriesCount,
                COUNT(PortfolioItem.Id) PortfolioItemsCount,
                Seller.SupplierGatewayId,
                Seller.Avatar,
                Seller.WhatsAppNumber,
                Seller.SellerCompleteProfile
            FROM Person Seller
            LEFT JOIN [Address] ON Seller.Address_Id = [Address].Id
            LEFT JOIN [Order] ON Seller.Id = [Order].Seller_Id
            LEFT JOIN OrderProduct ON [Order].Id = OrderProduct.Order_Id
            LEFT JOIN Quote ON Seller.Id = Quote.Seller_Id
            LEFT JOIN Subscription ON Seller.Id = Subscription.SellerId
            LEFT JOIN Portfolio ON Seller.Id = Portfolio.SellerId
            LEFT JOIN PortfolioItem ON Portfolio.SellerId = PortfolioItem.Portfolio_SellerId
            LEFT JOIN SubCategoryPortfolio ON Portfolio.SellerId = SubCategoryPortfolio.Portfolio_SellerId
            LEFT JOIN SubCategory ON SubCategoryPortfolio.SubCategory_Id = SubCategory.Id
            WHERE Seller.Discriminator = 'Seller'
                AND (Seller.RequireModeration IS NULL OR Seller.RequireModeration = 1)
		        AND Seller.Enabled != 0
            GROUP BY
                Seller.Id,
                Seller.FullName,
                ISNULL(Seller.AlternativeName, Seller.FullName),
                Seller.Email,
                [Address].CityAndState,
                CONVERT(BIT, CASE WHEN Subscription.[Status] IN (1 /*trialing*/, 2 /*paid*/, 3 /*pending payment*/) THEN 1 ELSE 0 END),
                Seller.CreatedAt,
                Seller.ModifiedAt, 
                Seller.[Enabled],
                CONVERT(BIT, ISNULL(Seller.OverrideGlobalPaymentFees, 0)),
                CONVERT(BIT, ISNULL(Seller.CustomIsLiable, 0)) ,
                ISNULL(Seller.CustomPaymentFeePercent, 0) ,
                ISNULL(Seller.CustomPaymentFeeAmount, 0) ,
                CONVERT(BIT, ISNULL(Seller.RequireModeration, 0)),
                Seller.SupplierGatewayId,
                Seller.Avatar,
                Seller.WhatsAppNumber,
                Seller.SellerCompleteProfile";
    }
}
