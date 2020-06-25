namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_7_SellerFriendlyUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FriendlyUrl", c => c.String(maxLength: 1000, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FriendlyUrl");
        }
    }
}
