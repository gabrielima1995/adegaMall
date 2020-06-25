namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_18_10_RentalMargin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Store", "RentalSettings_MinRentalMargin", c => c.Int());
            AddColumn("dbo.Store", "RentalSettings_RentalMarginUnit", c => c.Int());
            AddColumn("dbo.Product", "RentalSettings_MinRentalMargin", c => c.Int());
            AddColumn("dbo.Product", "RentalSettings_RentalMarginUnit", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "RentalSettings_RentalMarginUnit");
            DropColumn("dbo.Product", "RentalSettings_MinRentalMargin");
            DropColumn("dbo.Store", "RentalSettings_RentalMarginUnit");
            DropColumn("dbo.Store", "RentalSettings_MinRentalMargin");
        }
    }
}
