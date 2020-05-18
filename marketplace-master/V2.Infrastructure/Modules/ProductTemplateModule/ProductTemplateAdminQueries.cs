using Dapper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using V2.Application.Modules.ProductTemplateModule;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.ProductTemplateModule
{
    public class ProductTemplateAdminQueries : IProductTemplateAdminQueries
    {
        private readonly SqlDbConnection dbConnection;

        public ProductTemplateAdminQueries(SqlDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public ReadOnlyCollection<ProductTemplateAdminListItemDto> ListProductTemplateItems()
        {
            using (var conn = dbConnection.Connect())
            {
                var sql = @"
                    DECLARE @int INT SET @int = 0
                    DECLARE @decimal DECIMAL(18, 2) SET @decimal = 0

                    DROP TABLE IF EXISTS #tmpTemplates

                    SELECT
	                    Template.Id,
	                    Template.Title,
	                    Category.Name Category,
	                    Template.CoverImageUrl,
	                    Template.IsEnabled,
	                    Template.Dimensions_Length DimensionsLength,
	                    Template.Dimensions_Width DimensionsWidth,
	                    Template.Dimensions_Height DimensionsHeight,
	                    Template.Dimensions_Weight DimensionsWeight,
	                    @int ProductsCount,
	                    @decimal GrossRevenue
                    INTO #tmpTemplates
                    FROM ProductTemplate Template
                    INNER JOIN ProductCategory Category ON Template.CategoryId = Category.Id
                    WHERE Template.IsDeleted = 0
                    ORDER BY Template.Title

                    UPDATE #tmpTemplates
                    SET ProductsCount = aux.ProductsCount
                    FROM #tmpTemplates
                    CROSS APPLY (
	                    SELECT COUNT(1) ProductsCount FROM Product
	                    WHERE ProductTemplateId = #tmpTemplates.Id AND Deleted = 0 ) aux

                    UPDATE #tmpTemplates
                    SET GrossRevenue = aux.GrossRevenue
                    FROM #tmpTemplates
                    CROSS APPLY (
	                    SELECT ISNULL(SUM(OrderProduct.Price * OrderProduct.Quantity), @decimal) GrossRevenue
	                    FROM Product
	                    INNER JOIN OrderProduct ON Product.Id = OrderProduct.Product_Id
	                    INNER JOIN [Order] ON [Order].Status IN (1, 2, 3, 4, 14)
	                    WHERE Product.ProductTemplateId = #tmpTemplates.Id) aux

                    SELECT *
                    FROM #tmpTemplates

                    SELECT
	                    ProductTemplateProductVariant.ProductTemplate_Id TemplateId,
	                    VariantType.Name + ': ' + Variant.Name Variant
                    FROM ProductVariantType VariantType
                    INNER JOIN ProductVariant Variant ON VariantType.Id = Variant.[Type_Id]
                    INNER JOIN ProductTemplateProductVariant ON Variant.Id = ProductTemplateProductVariant.ProductVariant_Id
                    INNER JOIN ProductTemplate Template ON ProductTemplateProductVariant.ProductTemplate_Id = Template.Id AND Template.IsDeleted = 0
                    ORDER BY VariantType.Name, Variant.Name

                    DROP TABLE IF EXISTS #tmpProductTemplateStats";

                var grid = conn.QueryMultiple(sql);
                var templates = grid.Read<ProductTemplateAdminListItemDto>().ToList();
                var variants = grid.Read<TemplateVariantAux>().ToList();

                foreach (var template in templates)
                    template.Variants = variants.Where(v => v.TemplateId == template.Id).Select(v => v.Variant).ToList().AsReadOnly();

                return templates.AsReadOnly();
            }
        }

        private class TemplateVariantAux
        {
            public Guid TemplateId { get; set; }
            public string Variant { get; set; }
        }
    }
}
