namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeProductVariant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategory", "Slug", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.ProductVariant", "Slug", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.ProductVariantType", "Slug", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductVariantType", "Slug");
            DropColumn("dbo.ProductVariant", "Slug");
            DropColumn("dbo.ProductCategory", "Slug");
        }
    }
}
