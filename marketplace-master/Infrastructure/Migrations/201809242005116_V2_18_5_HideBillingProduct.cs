namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_18_5_HideBillingProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "IsBillingProduct", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "IsBillingProduct");
        }
    }
}
