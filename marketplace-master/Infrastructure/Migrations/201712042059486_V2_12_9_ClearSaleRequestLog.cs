namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_12_9_ClearSaleRequestLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AntifraudTransaction", "RequestContent", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.AntifraudTransaction", "RequestContent");
        }
    }
}
