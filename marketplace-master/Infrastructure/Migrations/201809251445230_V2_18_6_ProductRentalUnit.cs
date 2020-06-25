namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_18_6_ProductRentalUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Store", "RentalSettings_RentalPeriodUnit", c => c.Int());
            AddColumn("dbo.Product", "RentalSettings_RentalPeriodUnit", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "RentalSettings_RentalPeriodUnit");
            DropColumn("dbo.Store", "RentalSettings_RentalPeriodUnit");
        }
    }
}
