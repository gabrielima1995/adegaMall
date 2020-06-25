namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_19_1_RentalMultiplier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "MinMultiplier", c => c.Int());
            AddColumn("dbo.Product", "MaxMultiplier", c => c.Int());
            AddColumn("dbo.Product", "MultiplierPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.OrderProduct", "Multiplier", c => c.Int(nullable: false));
            AddColumn("dbo.OrderProduct", "MultiplierPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CartItem", "Multiplier", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.CartItem", "Multiplier");
            DropColumn("dbo.OrderProduct", "MultiplierPrice");
            DropColumn("dbo.OrderProduct", "Multiplier");
            DropColumn("dbo.Product", "MultiplierPrice");
            DropColumn("dbo.Product", "MaxMultiplier");
            DropColumn("dbo.Product", "MinMultiplier");
        }
    }
}
