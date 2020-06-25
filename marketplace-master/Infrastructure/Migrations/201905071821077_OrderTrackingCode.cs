namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OrderTrackingCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "TrackingCode", c => c.String(maxLength: 20, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Order", "TrackingCode");
        }
    }
}
