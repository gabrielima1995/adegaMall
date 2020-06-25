using Dapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using V2.Application.Modules.SellerAssistantModule;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.SellerAssistantModule
{
    public class GetSellerAssistantHandler : IRequestHandler<GetSellerAssistant, GetSellerAssistantResult>
    {
        public async Task<GetSellerAssistantResult> Handle(GetSellerAssistant request, CancellationToken cancellationToken)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT
                        Person.UserId,
                        Person.Id PersonId,
                        Person.Enabled IsEnabled,
                        AspNetUsers.Email
                    FROM AspNetUsers
                    JOIN Person ON AspNetUsers.Id = Person.UserId AND Person.AssistantToSellerId = @sellerId

                    SELECT AspNetUserClaims.ClaimType
                    FROM AspNetUserClaims
                    JOIN AspNetUsers ON AspNetUserClaims.UserId = AspNetUsers.Id
                    JOIN Person ON AspNetUsers.Id = Person.UserId AND Person.AssistantToSellerId = @sellerId
                    WHERE AspNetUserClaims.ClaimValue = @sellerId";

                var grid = await conn.QueryMultipleAsync(sql, new { sellerId = request.SellerId });
                var dto = await grid.ReadFirstOrDefaultAsync<GetSellerAssistantResult>();

                if (dto != null)
                {
                    var permissions = await grid.ReadAsync<string>();
                    dto.Permissions = permissions.ToList().AsReadOnly();
                }

                return dto;
            }
        }
    }
}
