namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_21_1_LeadSalesSubscription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscription", "LastCreditsAddDate", c => c.DateTime());
            AddColumn("dbo.Subscription", "NextCreditsAddDate", c => c.DateTime());
            AddColumn("dbo.Plan", "CreditsPerMonth", c => c.Int());
            AddColumn("dbo.Plan", "CreditsValidityDays", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Plan", "CreditsValidityDays");
            DropColumn("dbo.Plan", "CreditsPerMonth");
            DropColumn("dbo.Subscription", "NextCreditsAddDate");
            DropColumn("dbo.Subscription", "LastCreditsAddDate");
        }
    }
}
