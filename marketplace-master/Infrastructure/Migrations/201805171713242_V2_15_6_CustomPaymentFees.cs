namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_6_CustomPaymentFees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "OverrideGlobalPaymentFees", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "CustomIsLiable", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "CustomPaymentFeePercent", c => c.Decimal(precision: 5, scale: 2));
            AddColumn("dbo.AspNetUsers", "CustomPaymentFeeAmount", c => c.Decimal(precision: 7, scale: 2));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CustomPaymentFeeAmount");
            DropColumn("dbo.AspNetUsers", "CustomPaymentFeePercent");
            DropColumn("dbo.AspNetUsers", "CustomIsLiable");
            DropColumn("dbo.AspNetUsers", "OverrideGlobalPaymentFees");
        }
    }
}
