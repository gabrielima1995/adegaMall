namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductAndStoreDefaultShipping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Store", "DefaultShippingAmount", c => c.Decimal(nullable: true, precision: 7, scale: 2));
            AddColumn("dbo.Store", "DefaultShippingDeliveryDays", c => c.Int(nullable: true));
            AddColumn("dbo.Product", "DefaultShippingAmount", c => c.Decimal(nullable: true, precision: 7, scale: 2));
            AddColumn("dbo.Product", "DefaultShippingDeliveryDays", c => c.Int(nullable: true));

            Sql("UPDATE dbo.Store SET DefaultShippingAmount = 0");
            Sql("UPDATE dbo.Store SET DefaultShippingDeliveryDays = 0");
            Sql("UPDATE dbo.Product SET DefaultShippingAmount = 0");
            Sql("UPDATE dbo.Product SET DefaultShippingDeliveryDays = 0");

            AlterColumn("dbo.Store", "DefaultShippingAmount", c => c.Decimal(nullable: false, precision: 7, scale: 2));
            AlterColumn("dbo.Store", "DefaultShippingDeliveryDays", c => c.Int(nullable: false));
            AlterColumn("dbo.Product", "DefaultShippingAmount", c => c.Decimal(nullable: false, precision: 7, scale: 2));
            AlterColumn("dbo.Product", "DefaultShippingDeliveryDays", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "DefaultShippingDeliveryDays");
            DropColumn("dbo.Product", "DefaultShippingAmount");
            DropColumn("dbo.Store", "DefaultShippingDeliveryDays");
            DropColumn("dbo.Store", "DefaultShippingAmount");
        }
    }
}
