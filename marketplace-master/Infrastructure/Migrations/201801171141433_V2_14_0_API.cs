namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_14_0_API : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestReceivedApiLog",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    IP = c.String(maxLength: 8000, unicode: false),
                    Token = c.String(maxLength: 8000, unicode: false),
                    DateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Url = c.String(maxLength: 8000, unicode: false),
                    HttpVerb = c.String(maxLength: 8000, unicode: false),
                    HttpStatusCode = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.WebhookCalledApiLog",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Token = c.String(maxLength: 8000, unicode: false),
                    Seller = c.String(maxLength: 8000, unicode: false),
                    DateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Url = c.String(maxLength: 8000, unicode: false),
                    EventType = c.Int(nullable: false),
                    EntityRef = c.String(maxLength: 8000, unicode: false),
                    HttpVerb = c.String(maxLength: 8000, unicode: false),
                    HttpStatusCode = c.String(maxLength: 8000, unicode: false),
                    Attempt = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Webhook",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    EventType = c.Int(nullable: false),
                    Url = c.String(maxLength: 8000, unicode: false),
                    ApiToken_Id = c.Guid(nullable: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApiToken", t => t.ApiToken_Id)
                .Index(t => t.ApiToken_Id);

            CreateTable(
                "dbo.ApiToken",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(maxLength: 8000, unicode: false),
                    Token = c.String(maxLength: 8000, unicode: false),
                    Seller_Id = c.String(maxLength: 128, unicode: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Seller_Id)
                .Index(t => t.Seller_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Webhook", "ApiToken_Id", "dbo.ApiToken");
            DropForeignKey("dbo.ApiToken", "Seller_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApiToken", new[] { "Seller_Id" });
            DropIndex("dbo.Webhook", new[] { "ApiToken_Id" });
            DropTable("dbo.ApiToken");
            DropTable("dbo.Webhook");
            DropTable("dbo.WebhookCalledApiLog");
            DropTable("dbo.RequestReceivedApiLog");
        }
    }
}
