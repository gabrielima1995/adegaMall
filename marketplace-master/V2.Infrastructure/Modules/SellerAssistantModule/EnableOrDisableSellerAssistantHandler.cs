using Dapper;
using Framework;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using V2.Application.Modules.SellerAssistantModule;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.SellerAssistantModule
{
    public class EnableOrDisableSellerAssistantHandler : IRequestHandler<EnableOrDisableSellerAssistant, OperationResult>
    {
        public async Task<OperationResult> Handle(EnableOrDisableSellerAssistant request, CancellationToken cancellationToken)
        {
            using (var conn = MyDbConnection.Create())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    var sql = @"
                        DECLARE @userId VARCHAR(128)
                        SELECT TOP 1 @userId = UserId FROM Person WHERE AssistantToSellerId = @sellerId

                        UPDATE AspNetUsers
                        SET Enabled = @enabled
                        WHERE Id = @userId

                        UPDATE Person
                        SET Person.Enabled = @enabled
                        WHERE Person.AssistantToSellerId = @sellerId";

                    try
                    {
                        await conn.ExecuteAsync(sql, new { sellerId = request.SellerId, enabled = request.Enable }, tran);
                        tran.Commit();
                        return OperationResult.Success;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
