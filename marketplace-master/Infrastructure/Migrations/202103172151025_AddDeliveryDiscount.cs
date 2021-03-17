namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeliveryDiscount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "ShippingDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Coupon", "DistanceInKm", c => c.Int(nullable: false));
            AddColumn("dbo.Coupon", "MinOrderAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CartSeller", "SellerShippingDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartSeller", "SellerShippingDiscount");
            DropColumn("dbo.Coupon", "MinOrderAmount");
            DropColumn("dbo.Coupon", "DistanceInKm");
            DropColumn("dbo.Cart", "ShippingDiscount");
        }
    }
}
