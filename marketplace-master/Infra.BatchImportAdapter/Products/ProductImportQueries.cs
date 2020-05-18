using Application.Modules.BatchImport.Products;
using Dapper;
using Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

namespace Infra.BatchImportAdapter.Products
{
    public class ProductImportQueries : IProductImportQueries
    {
        private readonly ConnectionString connectionString;

        public ProductImportQueries(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public ReadOnlyCollection<KeyValuePair<string, Guid>> ListProductIdsByCustomIds(IEnumerable<string> customIds)
        {
            var sql = @"
                ;WITH cte AS(
	                SELECT CustomIds.Value AS CustomId, Product.Id AS ProductId, ROW_NUMBER() OVER (PARTITION BY CustomIds.Value ORDER BY CustomIds.Value) AS RowNumber
	                FROM Product
	                CROSS APPLY STRING_SPLIT(@customIds, ',') AS CustomIds
	                WHERE Deleted = 0
	                AND CustomIds.Value = Product.CustomId)

                SELECT CustomId, ProductId
                FROM cte
                WHERE RowNumber = 1";

            var sqlCustomIds = string.Join(",", customIds);

            using (var conn = new SqlConnection(connectionString.Value))
            {
                var ids = conn.Query<ProductIdForCustomId>(sql, new { customIds = sqlCustomIds });
                return ids.Select(id => id.AsKeyValuePair()).ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<KeyValuePair<string, Guid>> ListProductIdsByCustomIds(IEnumerable<CustomIdAndSellerEmail> customIdsAndSellerEmails)
        {
            var fullItems = customIdsAndSellerEmails.Where(item => item.CustomId != null && item.SellerEmail != null);

            if (fullItems.Count() == 0)
                return new ReadOnlyCollection<KeyValuePair<string, Guid>>(
                    new List<KeyValuePair<string, Guid>>());

            var sql = @"
                SELECT Product.CustomId, Product.Id AS ProductId
                FROM Product
                INNER JOIN Person AS Sellers ON Product.Store_SellerId = Sellers.Id
                CROSS APPLY STRING_SPLIT(@customIdsAndEmails, ',') AS CustomIdsAndEmails
                WHERE Deleted = 0
                AND CustomIdsAndEmails.Value = (Product.CustomId + '+' + Sellers.Email)";

            var sqlCustomIds = string.Join(",", fullItems.Select(item => $"{item.CustomId}+{item.SellerEmail}"));

            using (var conn = new SqlConnection(connectionString.Value))
            {
                var ids = conn.Query<ProductIdForCustomId>(sql, new { customIdsAndEmails = sqlCustomIds });
                return ids.Select(id => id.AsKeyValuePair()).ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<KeyValuePair<string, string>> ListSellerIdsBySellerEmails(IEnumerable<string> sellerEmails)
        {
            if (sellerEmails.Count() == 0)
                return new ReadOnlyCollection<KeyValuePair<string, string>>(
                    new List<KeyValuePair<string, string>>());

            var sql = @"
                SELECT Email AS SellerEmail, Id AS SellerId
                FROM Person
                CROSS APPLY STRING_SPLIT(@emails, ',') AS SellerEmails
                WHERE Discriminator = 'Seller'
                AND SellerEmails.Value = Email";

            var sqlEmails = string.Join(",", sellerEmails);

            using (var conn = new SqlConnection(connectionString.Value))
            {
                var ids = conn.Query<SellerIdForSellerEmail>(sql, new { emails = sqlEmails });
                return ids.Select(id => id.AsKeyValuePair()).ToList().AsReadOnly();
            }
        }

        private class ProductIdForCustomId
        {
            public string CustomId { get; set; }
            public Guid ProductId { get; set; }
            public KeyValuePair<string, Guid> AsKeyValuePair() => new KeyValuePair<string, Guid>(CustomId, ProductId);
        }

        private class SellerIdForSellerEmail
        {
            public string SellerEmail { get; set; }
            public string SellerId { get; set; }
            public KeyValuePair<string, string> AsKeyValuePair() => new KeyValuePair<string, string>(SellerEmail, SellerId);
        }
    }
}
