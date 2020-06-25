namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductTemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductTemplate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(maxLength: 100, unicode: false),
                        TitleIsEditable = c.Boolean(nullable: false),
                        CoverImageUrl = c.String(maxLength: 1000, unicode: false),
                        CategoryId = c.Int(nullable: false),
                        CategoryIsEditable = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 8000, unicode: false),
                        DescriptionIsEditable = c.Boolean(nullable: false),
                        Specification = c.String(maxLength: 8000, unicode: false),
                        SpecificationIsEditable = c.Boolean(nullable: false),
                        VariantsIsEditable = c.Boolean(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ImagesIsEditable = c.Boolean(nullable: false),
                        Dimensions_Length = c.Decimal(precision: 18, scale: 2),
                        Dimensions_Width = c.Decimal(precision: 18, scale: 2),
                        Dimensions_Height = c.Decimal(precision: 18, scale: 2),
                        Dimensions_Weight = c.Decimal(precision: 18, scale: 2),
                        DimensionsIsEditable = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategory", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ProductTemplateImage",
                c => new
                    {
                        ProductTemplateId = c.Guid(nullable: false),
                        Id = c.Guid(nullable: false),
                        Url = c.String(maxLength: 1000, unicode: false),
                        Position = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.ProductTemplateId, t.Id })
                .ForeignKey("dbo.ProductTemplate", t => t.ProductTemplateId)
                .Index(t => t.ProductTemplateId);
            
            CreateTable(
                "dbo.ProductTemplateProductVariant",
                c => new
                    {
                        ProductTemplate_Id = c.Guid(nullable: false),
                        ProductVariant_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductTemplate_Id, t.ProductVariant_Id })
                .ForeignKey("dbo.ProductTemplate", t => t.ProductTemplate_Id)
                .ForeignKey("dbo.ProductVariant", t => t.ProductVariant_Id)
                .Index(t => t.ProductTemplate_Id)
                .Index(t => t.ProductVariant_Id);
            
            AddColumn("dbo.Product", "ProductTemplateId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductTemplateProductVariant", "ProductVariant_Id", "dbo.ProductVariant");
            DropForeignKey("dbo.ProductTemplateProductVariant", "ProductTemplate_Id", "dbo.ProductTemplate");
            DropForeignKey("dbo.ProductTemplateImage", "ProductTemplateId", "dbo.ProductTemplate");
            DropForeignKey("dbo.ProductTemplate", "CategoryId", "dbo.ProductCategory");
            DropIndex("dbo.ProductTemplateProductVariant", new[] { "ProductVariant_Id" });
            DropIndex("dbo.ProductTemplateProductVariant", new[] { "ProductTemplate_Id" });
            DropIndex("dbo.ProductTemplateImage", new[] { "ProductTemplateId" });
            DropIndex("dbo.ProductTemplate", new[] { "CategoryId" });
            DropColumn("dbo.Product", "ProductTemplateId");
            DropTable("dbo.ProductTemplateProductVariant");
            DropTable("dbo.ProductTemplateImage");
            DropTable("dbo.ProductTemplate");
        }
    }
}
