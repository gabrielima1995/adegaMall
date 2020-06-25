namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_4_RentalInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CartItem", "RentalInfo_RentalPeriodValue", c => c.Decimal(precision: 18, scale: 2));
        }

        public override void Down()
        {
            DropColumn("dbo.CartItem", "RentalInfo_RentalPeriodValue");
            DropColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodValue");
        }
    }
}
