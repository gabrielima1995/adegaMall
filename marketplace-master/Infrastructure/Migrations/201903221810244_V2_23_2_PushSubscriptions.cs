namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_23_2_PushSubscriptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MarketplacePushSubscription",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Token = c.String(maxLength: 8000, unicode: false),
                    UserId = c.String(maxLength: 8000, unicode: false),
                    UserRole = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Token);

        }

        public override void Down()
        {
            DropIndex("dbo.MarketplacePushSubscription", new[] { "Token" });
            DropTable("dbo.MarketplacePushSubscription");
        }
    }
}
