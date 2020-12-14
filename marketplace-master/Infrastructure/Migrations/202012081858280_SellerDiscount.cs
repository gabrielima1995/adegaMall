namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SellerDiscount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartSeller", "SellerDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartSeller", "SellerDiscount");
        }
    }
}
