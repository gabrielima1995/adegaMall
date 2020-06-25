namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_14_23_ProductFriendlyUrls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "FriendlyUrl", c => c.String(nullable: true, maxLength: 1000, unicode: false));
            Sql("UPDATE dbo.Product SET FriendlyUrl = CONVERT(VARCHAR(36), Id)");
            AlterColumn("dbo.Product", "FriendlyUrl", c => c.String(nullable: false, maxLength: 1000, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "FriendlyUrl");
        }
    }
}
