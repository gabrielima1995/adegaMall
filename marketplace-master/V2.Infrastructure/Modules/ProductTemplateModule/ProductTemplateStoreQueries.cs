using Dapper;
using Domain.Modules.StoreModule.Price;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using V2.Application.Modules.ProductTemplateModule;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.ProductTemplateModule
{
    public class ProductTemplateStoreQueries : IProductTemplateStoreQueries
    {
        private readonly SqlDbConnection dbConnection;

        public ProductTemplateStoreQueries(SqlDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public ReadOnlyCollection<StoreTemplateItemDto> ListStoreTemplates(string sellerId, int startIndex, int pageSize)
        {
            using (var conn = dbConnection.Connect())
            {
                var sql = @"
                    -- DECLARE @sellerId VARCHAR(128)
                    -- DECLARE @startIndex INT SET @startIndex = 0
                    -- DECLARE @pageSize INT SET @pageSize = 100

                    SELECT
	                    Template.Id TemplateId,
	                    CASE
		                    WHEN Template.TitleIsEditable = 1 THEN ISNULL(Product.Title, Template.Title)
		                    ELSE Template.Title END Title,
	                    CASE
		                    WHEN Template.CategoryIsEditable = 1 THEN ISNULL(ProductCategory.Name, TemplateCategory.Name)
		                    ELSE TemplateCategory.Name END Category,
	                    CASE
		                    WHEN Template.DescriptionIsEditable = 1 THEN ISNULL(Product.[Description], Template.[Description])
		                    ELSE Template.[Description] END [Description],
	                    CASE
		                    WHEN Template.ImagesIsEditable = 1 THEN ISNULL(Product.CoverImageUrl, Template.CoverImageUrl)
		                    ELSE Template.CoverImageUrl END CoverImageUrl,
	                    Product.Id SellerProductId,
	                    Product.ProductPrice PriceJson,
	                    Product.CurrentStock
                    FROM ProductTemplate Template
                    INNER JOIN ProductCategory TemplateCategory ON Template.CategoryId = TemplateCategory.Id
                    LEFT JOIN Product ON Template.Id = Product.ProductTemplateId AND Product.Store_SellerId = @sellerId AND Product.Deleted = 0
                    LEFT JOIN ProductCategory ON Product.Category_Id = ProductCategory.Id
                    WHERE Template.IsDeleted = 0 AND Template.IsEnabled = 1
                    ORDER BY Template.Title
                    OFFSET @startIndex ROWS
                    FETCH NEXT @pageSize ROWS ONLY";

                var items = conn.Query<StoreTemplateItemDtoAux>(sql, new { sellerId, startIndex, pageSize });
                return items.Select(item =>
                {
                    item.DeserializePriceJson();
                    return item as StoreTemplateItemDto;

                }).ToList().AsReadOnly();
            }
        }

        private class StoreTemplateItemDtoAux : StoreTemplateItemDto
        {
            public string PriceJson { get; set; }
            public void DeserializePriceJson()
            {
                if (PriceJson != null)
                    Price = JsonConvert.DeserializeObject<ProductPrice>(PriceJson);
            }
        }
    }
}
