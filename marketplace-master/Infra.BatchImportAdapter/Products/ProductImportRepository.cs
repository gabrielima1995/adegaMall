using Application.Modules.BatchImport.Products;
using Dapper;
using Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.BatchImportAdapter.Products
{
    public class ProductImportRepository : IProductImportRepository
    {
        private const string defaultProductImageName = "product-img-placeholder.jpg";
        private readonly ConnectionString connectionString;

        public ProductImportRepository(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task InsertProductImageRecordsAsync(IEnumerable<KeyValuePair<Guid, string>> productIdsAndImageUrls)
        {
            var imagesForInsert = ConvertProductIdsAndImageUrlsForInsert(productIdsAndImageUrls);

            using (var conn = new SqlConnection(connectionString.Value))
            {
                await conn.ExecuteAsync(
                    "INSERT INTO dbo.ProductImage (Id, Url, Position, CreatedAt, Product_Id) VALUES (NEWID(), @Url, @Position, GETUTCDATE(), @ProductId)",
                    imagesForInsert);
            }
        }

        public async Task FixProductCoverImageChangingItFromDefaultAsync(IEnumerable<KeyValuePair<Guid, string>> productIdsAndImageUrls)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                await conn.ExecuteAsync(
                    $"DELETE FROM dbo.ProductImage WHERE Product_Id = @Key AND Url LIKE '%{defaultProductImageName}'",
                    productIdsAndImageUrls, transaction);

                await conn.ExecuteAsync(
                    $"UPDATE dbo.Product SET CoverImageUrl = (SELECT TOP 1 Url FROM dbo.ProductImage WHERE dbo.Product.Id = dbo.ProductImage.Product_Id ORDER BY dbo.ProductImage.Position) WHERE Id = @Key",
                    productIdsAndImageUrls, transaction);

                transaction.Commit();
            }
        }

        private IEnumerable<ProductImageForInsert> ConvertProductIdsAndImageUrlsForInsert(IEnumerable<KeyValuePair<Guid, string>> productIdsAndImageUrls)
        {
            // Trick to get a proper Position int for all images everytime.
            var refDate = new DateTime(2020, 1, 1);
            var timeDiff = DateTime.UtcNow - refDate;
            var imagePositionBase = Convert.ToInt32(Math.Round(timeDiff.TotalMinutes));

            return productIdsAndImageUrls.Select(entry => new ProductImageForInsert
            {
                ProductId = entry.Key,
                Url = entry.Value,
                Position = imagePositionBase++
            });
        }

        private class ProductImageForInsert
        {
            public Guid ProductId { get; set; }
            public string Url { get; set; }
            public int Position { get; set; }
        }
    }
}
