using Application.Modules.BatchImport;
using Dapper;
using Framework;
using Infra.BatchImportAdapter.Internals;

namespace Infra.BatchImportAdapter.Sellers
{
    public class SellerAvatarUpdater : ISellerAvatarUpdater
    {
        private const string sql = @"UPDATE Person SET Avatar = @avatarUrl WHERE Id = @sellerId";
        private readonly ConnectionString connectionString;

        public SellerAvatarUpdater(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public void UpdateSellerAvatar(string sellerId, string avatarUrl)
        {
            using (var conn = DbConnectionHelper.GetConnection(connectionString.Value))
                conn.Execute(sql, new { sellerId, avatarUrl });
        }
    }
}
