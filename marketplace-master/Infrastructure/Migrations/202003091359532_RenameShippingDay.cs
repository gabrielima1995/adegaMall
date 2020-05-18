namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameShippingDay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartItem", "DeliveryDays", c => c.Int());
            Sql("UPDATE dbo.CartItem SET DeliveryDays = ISNULL(ShippingDays, 0)");
            AlterColumn("dbo.CartItem", "DeliveryDays", c => c.Int(nullable: false));

            AddColumn("dbo.CartSeller", "SellerDeliveryDays", c => c.Int());
            Sql("UPDATE dbo.CartSeller SET SellerDeliveryDays = ISNULL(SellerDeliveryDays, 0)");
            AlterColumn("dbo.CartSeller", "SellerDeliveryDays", c => c.Int(nullable: false));

            DropColumn("dbo.CartItem", "ShippingDays");
            DropColumn("dbo.CartSeller", "SellerShippingDays");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartSeller", "SellerShippingDays", c => c.Int(nullable: false));
            AddColumn("dbo.CartItem", "ShippingDays", c => c.Int(nullable: false));
            DropColumn("dbo.CartSeller", "SellerDeliveryDays");
            DropColumn("dbo.CartItem", "DeliveryDays");
        }
    }
}
