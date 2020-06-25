namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_28_4_ProductMinRentalAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Store", "RentalSettings_MinRentalAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Product", "RentalSettings_MinRentalAmount", c => c.Decimal(precision: 18, scale: 2));
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "RentalSettings_MinRentalAmount");
            DropColumn("dbo.Store", "RentalSettings_MinRentalAmount");
        }
    }
}
