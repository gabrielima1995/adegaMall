namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_3_RentalInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "RentalInfo_WasFullyReturned", c => c.Boolean());
            AddColumn("dbo.Order", "RentalInfo_FullyReturnedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Order", "RentalInfo_StartReminderSent", c => c.Boolean());
            AddColumn("dbo.Order", "RentalInfo_EndReminderSent", c => c.Boolean());
            AddColumn("dbo.Store", "RentalSettings_MinRentalPeriod", c => c.Int());
            AddColumn("dbo.Store", "RentalSettings_MaxRentalPeriod", c => c.Int());
            AddColumn("dbo.Store", "RentalSettings_MinRentalNotice", c => c.Int());
            AddColumn("dbo.Store", "RentalSettings_MinRentalNoticeUnit", c => c.Int());
            AddColumn("dbo.Store", "RentalSettings_AvailabilityWarning", c => c.String(maxLength: 500, unicode: false));
            AddColumn("dbo.Store", "RentalSettings_ChargeDoubleShipping", c => c.Boolean());

            Sql("UPDATE [dbo].[Order] SET RentalInfo_WasFullyReturned = RentalFullyReturned, RentalInfo_FullyReturnedAt = RentalFullyReturnedAt WHERE RentalFullyReturned IS NOT NULL");

            DropColumn("dbo.Order", "RentalFullyReturned");
            DropColumn("dbo.Order", "RentalFullyReturnedAt");
        }

        public override void Down()
        {
            AddColumn("dbo.Order", "RentalFullyReturnedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Order", "RentalFullyReturned", c => c.Boolean());
            DropColumn("dbo.Store", "RentalSettings_ChargeDoubleShipping");
            DropColumn("dbo.Store", "RentalSettings_AvailabilityWarning");
            DropColumn("dbo.Store", "RentalSettings_MinRentalNoticeUnit");
            DropColumn("dbo.Store", "RentalSettings_MinRentalNotice");
            DropColumn("dbo.Store", "RentalSettings_MaxRentalPeriod");
            DropColumn("dbo.Store", "RentalSettings_MinRentalPeriod");
            DropColumn("dbo.Order", "RentalInfo_EndReminderSent");
            DropColumn("dbo.Order", "RentalInfo_StartReminderSent");
            DropColumn("dbo.Order", "RentalInfo_FullyReturnedAt");
            DropColumn("dbo.Order", "RentalInfo_WasFullyReturned");
        }
    }
}
