using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.DTO;
using V2.Application.Modules.Sellers;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.SellerTransfer
{
    public class SellerScheduledTransfer : ISellerScheduledTransferQueries
    {
        private readonly SqlDbConnection dbConnection;

        public SellerScheduledTransfer(SqlDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public List<SellerScheduledTransferViewModel> GetScheduledTransferForAdmin(string sellerId)
        {
            using (var conn = dbConnection.Connect())
            {
                var sql = @"
                    -- DECLARE @sellerId VARCHAR(128) SET @sellerId = '025ee302-f935-44cd-8168-c3e9695392f7'

                    SELECT 
                        ScheduledTransfer.Id as Id,
                        OrderId, 
                        BuyerId,
                        TransferAmount, 
                        DueDate,
                        Anticipated, 
                        TransferAuthorized, 
                        Transferred, 
                        Canceled, 
                        GatewayPayableId, 
                        Installments, 
                        CurrentInstallment, 
                        ScheduledTransfer.CreatedAt AS CreatedAt, 
                        Buyer.FullName as BuyerFullName 
                    FROM ScheduledTransfer LEFT JOIN Person as Buyer ON 
                    ScheduledTransfer.BuyerId = Buyer.Id 
                    WHERE SellerId = @sellerId";
                var grid = conn.QueryMultiple(sql, new { sellerId });
                var sellerScheduledTransferViewModel = grid.Read<SellerScheduledTransferViewModel>().ToList();
                return sellerScheduledTransferViewModel;
            }
        }
    }
}
