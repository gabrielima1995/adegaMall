using Dapper;
using Domain.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.StoreModule.Queries
{
    public class StoreProductQueries : IStoreProductQueries
    {
        private readonly SqlDbConnection dbConnection;

        public StoreProductQueries(SqlDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public StoreProductResultsVM ListProducts(string sellerId, string query, int startIndex, int pageSize)
        {
            using (var conn = dbConnection.Connect())
            {
                var sql = @"
                    --DECLARE @sellerId VARCHAR(128) SET @sellerId = 'c34f494a-924e-48b4-862b-077fb1ba8199'
                    --DECLARE @query VARCHAR
                    --DECLARE @startIndex INT SET @startIndex = 0
                    --DECLARE @pageSize INT SET @pageSize = 25

                    DROP TABLE IF EXISTS #tmpStoreProducts
                    SELECT
                        p.Id,
                        Title,
                        ProductPrice,
                        CoverImageUrl,
                        CurrentStock,
                        pc.Name as CategoryName,
                        CustomId
                    INTO #tmpStoreProducts
                    FROM Product p
                    JOIN ProductCategory pc ON p.Category_Id = pc.Id
                    WHERE
                        Store_SellerId = @sellerId AND p.Deleted = 0
                        AND (@query IS NULL OR p.Title LIKE @query OR p.CustomId LIKE @query OR pc.Name LIKE @query)

                    -- // ----------

                    DECLARE @totalCount INT
                    SELECT @totalCount = COUNT(1) FROM Product WHERE Store_SellerId = @sellerId AND Deleted = 0

                    DECLARE @filteredCount INT
                    SELECT @filteredCount = COUNT(1) FROM #tmpStoreProducts

                    -- // ----------

                    SELECT @totalCount TotalCount, @filteredCount FilteredCount

                    SELECT * FROM #tmpStoreProducts
                    ORDER BY Title
                    OFFSET @startIndex ROWS
                    FETCH NEXT @pageSize ROWS ONLY

                    DROP TABLE IF EXISTS #tmpStoreProducts";

                query = string.IsNullOrWhiteSpace(query) ? null : $"%{query.Trim()}%";
                var grid = conn.QueryMultiple(sql, new { sellerId, query, startIndex, pageSize });

                var counters = grid.ReadSingle<CountersAux>();
                var items = grid.Read<StoreProductVMAux>();

                return new StoreProductResultsVM(counters.TotalCount, counters.FilteredCount, items);
            }
        }

        private class CountersAux
        {
            public int TotalCount { get; set; }
            public int FilteredCount { get; set; }
        }

        private class StoreProductVMAux : StoreProductVM
        {
            public string ProductPrice
            {
                set
                {
                    dynamic parsed = JObject.Parse(value);
                    Price = (decimal)(parsed.DisplayPrice.Value ?? 0m);
                    PriceFormatted = (string)parsed.DisplayPrice.FullPriceText;
                    DiscountPriceFormatted = (string)parsed.DisplayPrice.Discount.Formatted;
                }
            }
        }
    }
}
