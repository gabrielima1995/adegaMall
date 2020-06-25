namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShippingDays : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "DeliveryDate", c => c.DateTime());
            AddColumn("dbo.Order", "DeliveryDate", c => c.DateTime());

            AddColumn("dbo.CartItem", "ShippingDays", c => c.Int());
            Sql("UPDATE dbo.CartItem SET ShippingDays = 0");
            AlterColumn("dbo.CartItem", "ShippingDays", c => c.Int(nullable: false));

            AddColumn("dbo.CartSeller", "SellerShippingDays", c => c.Int());
            Sql("UPDATE dbo.CartSeller SET SellerShippingDays = 0");
            AlterColumn("dbo.CartSeller", "SellerShippingDays", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.CartSeller", "SellerShippingDays");
            DropColumn("dbo.CartItem", "ShippingDays");
            DropColumn("dbo.Order", "DeliveryDate");
            DropColumn("dbo.Cart", "DeliveryDate");
        }
    }
}
