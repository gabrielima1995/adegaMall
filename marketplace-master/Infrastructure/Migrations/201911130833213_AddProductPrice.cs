namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "BasePrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Product", "BasePriceOriginal", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Product", "ProductPrice", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.Product", "ProductPriceVersion", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "ProductPriceVersion");
            DropColumn("dbo.Product", "ProductPrice");
            DropColumn("dbo.Product", "BasePriceOriginal");
            DropColumn("dbo.Product", "BasePrice");
        }
    }
}
