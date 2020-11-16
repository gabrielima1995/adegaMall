using Dapper;
using Domain.Entities.StoreModule;
using Domain.Modules.ProductTemplateModule;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Data.Entity;
using System.Linq;
using V2.Application.Modules.ProductTemplateModule;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.ProductTemplateModule
{
    public class ProductTemplateRepository : IProductTemplateRepository
    {
        private readonly ScaffoldContext db;
        private readonly SqlDbConnection dbConnection;

        public ProductTemplateRepository(ScaffoldContext context, SqlDbConnection dbConnection)
        {
            db = context;
            this.dbConnection = dbConnection;
        }

        public OperationResult Add(ProductTemplate productTemplate, bool saveChanges = true)
        {
            var variantIds = productTemplate.Variants.SelectList(v => v.Id);
            var dbVariants = db.Set<ProductVariant>().Where(v => variantIds.Any(id => v.Id == id)).ToList();
            productTemplate.Variants.Clear();
            productTemplate.Variants.AddRange(dbVariants);

            var category = db.ProductCategories.Find(productTemplate.CategoryId);
            productTemplate.Category = category;

            db.ProductTemplates.Add(productTemplate);
            if (saveChanges) db.SaveChanges();
            return OperationResult.Success;
        }

        public ProductTemplate Find(Guid id)
        {
            var template = db.ProductTemplates
                .Include(t => t.Category)
                .Include(t => t.Images)
                .Include(t => t.Variants)
                .Include(t => t.Variants.Select(v => v.Type))
                .SingleOrDefault(t => t.Id == id);

            if (template.Images?.Count > 0)
                template.Images = template.Images.OrderBy(img => img.Position).ToList();

            return template;
        }

        public OperationResult Remove(ProductTemplate productTemplate)
        {
            productTemplate.MarkAsDeleted();
            db.Entry(productTemplate).State = EntityState.Modified;

            db.SaveChanges();
            return OperationResult.Success;
        }

        public OperationResult Update(ProductTemplate productTemplate)
        {
            var variantIds = productTemplate.Variants.SelectList(v => v.Id);
            var dbVariants = db.Set<ProductVariant>().Where(v => variantIds.Any(id => v.Id == id)).ToList();
            productTemplate.Variants.Clear();
            productTemplate.Variants.AddRange(dbVariants);

            var category = db.ProductCategories.Find(productTemplate.CategoryId);
            productTemplate.Category = category;

            db.Entry(productTemplate).State = EntityState.Modified;
            productTemplate.MarkAsUpdated();

            db.SaveChanges();
            return OperationResult.Success;
        }

        public OperationResult OverwriteProductPropertiesFromTemplateWhereNotEditable(Guid templateId)
        {
            using (var conn = dbConnection.Connect())
            {
                var sql = @"
                    -- DECLARE @templateId UNIQUEIDENTIFIER SET @templateId = '9F0A5B8B-C44A-4045-B965-376E63CB014F'

                    UPDATE Product SET
                        Title = (CASE WHEN Template.TitleIsEditable = 1 THEN Product.Title ELSE Template.Title END),
                        Category_Id = (CASE WHEN Template.CategoryIsEditable = 1 THEN Product.Category_Id ELSE Template.CategoryId END),
                        [Description] = (CASE WHEN Template.DescriptionIsEditable = 1 THEN Product.[Description] ELSE Template.[Description] END),
                        Specifications = (CASE WHEN Template.SpecificationIsEditable = 1 THEN Product.Specifications ELSE Template.Specification END),
                        [Length] = (CASE WHEN Template.DimensionsIsEditable = 1 THEN Product.[Length] ELSE Template.Dimensions_Length END),
                        Width = (CASE WHEN Template.DimensionsIsEditable = 1 THEN Product.Width ELSE Template.Dimensions_Width END),
                        Height = (CASE WHEN Template.DimensionsIsEditable = 1 THEN Product.Height ELSE Template.Dimensions_Height END),
                        [Weight] = (CASE WHEN Template.DimensionsIsEditable = 1 THEN Product.[Weight] ELSE Template.Dimensions_Weight END)
                    FROM Product
                    INNER JOIN ProductTemplate Template ON Product.ProductTemplateId = Template.Id
                    WHERE Product.ProductTemplateId = @templateId

                    IF ((SELECT TOP 1 ImagesIsEditable FROM ProductTemplate WHERE Id = @templateId) = 0) BEGIN
                        DELETE FROM ProductImage
                        WHERE Product_Id IN (SELECT Id FROM Product WHERE ProductTemplateId = @templateId)

                        INSERT INTO ProductImage (Id, [Url], Position, CreatedAt, Product_Id)
                        SELECT NEWID(), TemplateImage.[Url], TemplateImage.Position, GETUTCDATE(), Product.Id
                        FROM ProductTemplate Template
                        INNER JOIN ProductTemplateImage TemplateImage ON Template.Id = TemplateImage.ProductTemplateId
                        INNER JOIN Product ON Template.Id = Product.ProductTemplateId
                        WHERE Template.Id = @templateId
                    END

                    IF ((SELECT TOP 1 VariantsIsEditable FROM ProductTemplate WHERE Id = @templateId) = 0) BEGIN
                        -- Cadastra as variantes
                        DELETE FROM ProductVariantProduct
                        WHERE Product_Id IN (SELECT Id FROM Product WHERE ProductTemplateId = @templateId)

                        INSERT INTO ProductVariantProduct (ProductVariant_Id, Product_Id)
                        SELECT TemplateVariant.ProductVariant_Id, Product.Id
                        FROM ProductTemplate Template
                        INNER JOIN ProductTemplateProductVariant TemplateVariant ON Template.Id = TemplateVariant.ProductTemplate_Id
                        INNER JOIN Product ON Template.Id = Product.ProductTemplateId
                        WHERE Template.Id = @templateId

                        -- Cadastra as variantes por estoque
                        -- Sem excluir as que dão match com as variantes do template, porque neste caso existe a configuração de estoque do vendedor
                        -- Que não queremos perder
                        DELETE FROM StockProductVariant
                        WHERE Product_Id IN (SELECT Id FROM Product WHERE ProductTemplateId = @templateId)
                        AND Variant_Id NOT IN (SELECT ProductVariant_Id FROM ProductTemplateProductVariant WHERE ProductTemplate_Id = @templateId)

                        INSERT INTO StockProductVariant (CurrentStock, CreatedAt, Variant_Id, Product_Id)
                        SELECT 0, GETUTCDATE(), TemplateVariant.ProductVariant_Id, Product.Id
                        FROM ProductTemplate Template
                        INNER JOIN ProductTemplateProductVariant TemplateVariant ON Template.Id = TemplateVariant.ProductTemplate_Id
                        INNER JOIN Product ON Template.Id = Product.ProductTemplateId
                        WHERE Template.Id = @templateId
                        AND TemplateVariant.ProductVariant_Id NOT IN (SELECT ProductVariant_Id FROM ProductTemplateProductVariant WHERE ProductTemplate_Id = @templateId)
                    END";

                conn.Execute(sql, new { templateId });
                return OperationResult.Success;
            }
        }

        public OperationResult DeleteProductsAssociatedToDeletedTemplate(Guid templateId)
        {
            using (var conn = dbConnection.Connect())
            {
                var sql = @"
                  --DECLARE @templateId UNIQUEIDENTIFIER SET @templateId = 'a1fa0bf7-60b2-46ee-8abe-79b1970deac0'
                    UPDATE Product SET
                        Deleted = 1,
                        DeletedAt = GETUTCDATE()
                    FROM Product
                    INNER JOIN ProductTemplate Template ON Product.ProductTemplateId = Template.Id
                    WHERE Product.ProductTemplateId = @templateId

                     UPDATE ProductTemplate SET
                        IsDeleted = 1,
                        IsEnabled = 0
                    WHERE Id = @templateId";

                conn.Execute(sql, new { templateId });
                return OperationResult.Success;
            }
        }
    }
}
