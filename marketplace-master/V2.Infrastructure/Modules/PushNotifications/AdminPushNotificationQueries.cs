using Dapper;
using Domain.Entities;
using System.Collections.Generic;
using V2.Application.Modules.Admin;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.PushNotifications
{
    public class AdminPushNotificationQueries : IAdminNotificationQueries
    {
        private readonly SqlDbConnection dbConnection;

        public AdminPushNotificationQueries(SqlDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public List<MarketplacePushSubscription> GetDataForNotification(NotificationType notificationType)
        {
            string query = "";
            switch (notificationType)
            {
                case NotificationType.AllSellers:
                    query = @";WITH ctePushTokens AS (
                                  SELECT UserId, Token, ROW_NUMBER() OVER(PARTITION BY UserId ORDER BY CreatedAt DESC) RowNumber
                                  FROM MarketplacePushSubscription
                                  WHERE UserRole = 'Seller')
                              SELECT UserId, Token FROM ctePushTokens WHERE RowNumber = 1";
                    break;
                case NotificationType.EnabledSellers:
                    query = @";WITH ctePushTokens AS (
                                  SELECT UserId, Token, ROW_NUMBER() OVER(PARTITION BY UserId ORDER BY p.CreatedAt DESC) RowNumber
                                  FROM MarketplacePushSubscription p
                                  JOIN Person u on p.UserId = u.Id
                                  WHERE UserRole = 'Seller' and u.Enabled = 1)
                              SELECT UserId, Token FROM ctePushTokens WHERE RowNumber = 1";
                    break;
                case NotificationType.SellersPendingModeration:
                    query = @";WITH ctePushTokens AS (
                                  SELECT UserId, Token, ROW_NUMBER() OVER(PARTITION BY UserId ORDER BY p.CreatedAt DESC) RowNumber
                                  FROM MarketplacePushSubscription p
                                  JOIN Person u on p.UserId = u.Id
                                  WHERE UserRole = 'Seller'and (u.RequireModeration is null or u.RequireModeration = 1))
                              SELECT UserId, Token FROM ctePushTokens WHERE RowNumber = 1";
                    break;
                case NotificationType.AllBuyers:
                    query = @";WITH ctePushTokens AS (
                                  SELECT UserId, Token, ROW_NUMBER() OVER(PARTITION BY UserId ORDER BY p.CreatedAt DESC) RowNumber
                                  FROM MarketplacePushSubscription p
                                  WHERE UserRole = 'Buyer')
                              SELECT UserId, Token FROM ctePushTokens WHERE RowNumber = 1";
                    break;
                case NotificationType.EnabledBuyers:
                    query = @";WITH ctePushTokens AS (
                                  SELECT UserId, Token, ROW_NUMBER() OVER(PARTITION BY UserId ORDER BY p.CreatedAt DESC) RowNumber
                                  FROM MarketplacePushSubscription p
                                  JOIN Person u on p.UserId = u.Id
                                  WHERE UserRole = 'Buyer' and u.Enabled = 1)
                              SELECT UserId, Token FROM ctePushTokens WHERE RowNumber = 1";
                    break;
                case NotificationType.BuyerPendingModeration:
                    query = @";WITH ctePushTokens AS (
                                  SELECT UserId, Token, ROW_NUMBER() OVER(PARTITION BY UserId ORDER BY p.CreatedAt DESC) RowNumber
                                  FROM MarketplacePushSubscription p
                                  JOIN Person u on p.UserId = u.Id
                                  WHERE UserRole = 'Buyer' and (u.RequireModeration is null or u.RequireModeration = 1))
                              SELECT UserId, Token FROM ctePushTokens WHERE RowNumber = 1";
                    break;
            }

            using (var conn = dbConnection.Connect())
            {
                var pushSubscriptions = conn.Query<MarketplacePushSubscription>(query);
                return pushSubscriptions.AsList();
            }
        }
    }
}
