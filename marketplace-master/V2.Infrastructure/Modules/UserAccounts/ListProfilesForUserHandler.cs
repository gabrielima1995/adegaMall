using Dapper;
using Framework;
using MediatR;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using V2.Application.Modules.UserAccounts;

namespace V2.Infrastructure.Modules.UserAccounts
{
    public class ListProfilesForUserHandler : IRequestHandler<ListProfilesForUser, ListProfilesForUserResult>
    {
        private readonly ConnectionString connectionString;

        public ListProfilesForUserHandler(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<ListProfilesForUserResult> Handle(ListProfilesForUser request, CancellationToken cancellationToken)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var sql = @"
                    --DECLARE @userId VARCHAR(128) SET @userId = '5d3d5a5e-4961-4d86-8163-b66d34b923e8'

                    SELECT
                        Id 'PersonId', UserId, Avatar,
                        ISNULL(AlternativeName, FullName) 'Name',
                        DocumentNumber,
                        CONVERT(BIT, CASE WHEN Enabled = 1 THEN 0 ELSE 1 END) IsDisabled,
                        Discriminator 'Type'
                    FROM Person
                    WHERE UserId = @userId";

                var profiles = await conn.QueryAsync<ListProfilesForUserResultItem>(sql, new { userId = request.UserId });
                return new ListProfilesForUserResult(profiles);
            }
        }
    }
}
