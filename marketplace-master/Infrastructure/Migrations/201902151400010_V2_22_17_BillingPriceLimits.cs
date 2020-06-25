namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_22_17_BillingPriceLimits : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QuoteBilling", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }

        public override void Down()
        {
            AlterColumn("dbo.QuoteBilling", "Amount", c => c.Decimal(nullable: false, precision: 7, scale: 2));
        }
    }
}
