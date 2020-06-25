namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddBlockScheduleForProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "RentalSettings_BlockScheduleForProduct", c => c.Boolean());
            Sql("UPDATE dbo.Product SET RentalSettings_BlockScheduleForProduct = 0");
            AlterColumn("dbo.Product", "RentalSettings_BlockScheduleForProduct", c => c.Boolean(nullable: false));

            AddColumn("dbo.Store", "RentalSettings_BlockScheduleForProduct", c => c.Boolean());
            Sql("UPDATE dbo.Store SET RentalSettings_BlockScheduleForProduct = 0");
            AlterColumn("dbo.Store", "RentalSettings_BlockScheduleForProduct", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "RentalSettings_BlockScheduleForProduct");
            DropColumn("dbo.Store", "RentalSettings_BlockScheduleForProduct");
        }
    }
}
