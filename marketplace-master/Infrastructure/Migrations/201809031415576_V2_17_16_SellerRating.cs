namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_17_16_SellerRating : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRating",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    RatingTargetId = c.String(maxLength: 8000, unicode: false),
                    RatingSenderId = c.String(maxLength: 8000, unicode: false),
                    Hidden = c.Boolean(nullable: false),
                    Comment = c.String(maxLength: 8000, unicode: false),
                    Rating = c.Int(),
                    OrderId = c.Guid(),
                    QuoteId = c.Guid(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.Quote", t => t.QuoteId)
                .Index(t => t.OrderId)
                .Index(t => t.QuoteId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.UserRating", "QuoteId", "dbo.Quote");
            DropForeignKey("dbo.UserRating", "OrderId", "dbo.Order");
            DropIndex("dbo.UserRating", new[] { "QuoteId" });
            DropIndex("dbo.UserRating", new[] { "OrderId" });
            DropTable("dbo.UserRating");
        }
    }
}
