namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_0_ProductRental : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PreferredTimeZoneId", c => c.Int());
            Sql("UPDATE dbo.AspNetUsers SET PreferredTimeZoneId = 910 /* (UTC-03:00) Brasilia */");
            AlterColumn("dbo.AspNetUsers", "PreferredTimeZoneId", c => c.Int(nullable: false));

            AddColumn("dbo.Order", "BusinessModel", c => c.Int());
            Sql("UPDATE dbo.[Order] SET BusinessModel = 0");
            AlterColumn("dbo.Order", "BusinessModel", c => c.Int(nullable: false));

            AddColumn("dbo.Order", "RentalFullyReturned", c => c.Boolean());
            AddColumn("dbo.Order", "RentalFullyReturnedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Product", "RentalSettings_MinRentalPeriod", c => c.Int());
            AddColumn("dbo.Product", "RentalSettings_MaxRentalPeriod", c => c.Int());
            AddColumn("dbo.Product", "RentalSettings_MinRentalNotice", c => c.Int());
            AddColumn("dbo.Product", "RentalSettings_MinRentalNoticeUnit", c => c.Int());
            AddColumn("dbo.Product", "RentalSettings_AvailabilityWarning", c => c.String(maxLength: 500, unicode: false));
            AddColumn("dbo.Product", "RentalSettings_ChargeDoubleShipping", c => c.Boolean());
            AddColumn("dbo.OrderProduct", "RentalInfo_StartDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.OrderProduct", "RentalInfo_EndDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodInHours", c => c.Int());
            AddColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodUnit", c => c.Int());
            AddColumn("dbo.OrderProduct", "RentalInfo_ChargeDoubleShipping", c => c.Boolean());
            AddColumn("dbo.OrderProduct", "RentalInfo_IsReturned", c => c.Boolean());
            AddColumn("dbo.OrderProduct", "RentalInfo_ReturnedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.CartItem", "RentalInfo_StartDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.CartItem", "RentalInfo_EndDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.CartItem", "RentalInfo_RentalPeriod", c => c.Int());
            AddColumn("dbo.CartItem", "RentalInfo_RentalPeriodUnit", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.CartItem", "RentalInfo_RentalPeriodUnit");
            DropColumn("dbo.CartItem", "RentalInfo_RentalPeriod");
            DropColumn("dbo.CartItem", "RentalInfo_EndDate");
            DropColumn("dbo.CartItem", "RentalInfo_StartDate");
            DropColumn("dbo.OrderProduct", "RentalInfo_ReturnedAt");
            DropColumn("dbo.OrderProduct", "RentalInfo_IsReturned");
            DropColumn("dbo.OrderProduct", "RentalInfo_ChargeDoubleShipping");
            DropColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodUnit");
            DropColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodInHours");
            DropColumn("dbo.OrderProduct", "RentalInfo_EndDate");
            DropColumn("dbo.OrderProduct", "RentalInfo_StartDate");
            DropColumn("dbo.Product", "RentalSettings_ChargeDoubleShipping");
            DropColumn("dbo.Product", "RentalSettings_AvailabilityWarning");
            DropColumn("dbo.Product", "RentalSettings_MinRentalNoticeUnit");
            DropColumn("dbo.Product", "RentalSettings_MinRentalNotice");
            DropColumn("dbo.Product", "RentalSettings_MaxRentalPeriod");
            DropColumn("dbo.Product", "RentalSettings_MinRentalPeriod");
            DropColumn("dbo.Order", "RentalFullyReturnedAt");
            DropColumn("dbo.Order", "RentalFullyReturned");
            DropColumn("dbo.Order", "BusinessModel");
            DropColumn("dbo.AspNetUsers", "PreferredTimeZoneId");
        }
    }
}
