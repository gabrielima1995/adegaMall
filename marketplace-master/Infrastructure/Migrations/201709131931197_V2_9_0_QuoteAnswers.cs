namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_9_0_QuoteAnswers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuoteAnswer",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Text = c.String(maxLength: 8000, unicode: false),
                    Price = c.Decimal(precision: 18, scale: 2),
                    AuthorId = c.String(nullable: false, maxLength: 128, unicode: false),
                    Quote_Id = c.Guid(nullable: false),
                    Buyer_Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    Seller_Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Buyer_Id)
                .ForeignKey("dbo.Quote", t => t.Quote_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Seller_Id)
                .Index(t => t.Quote_Id)
                .Index(t => t.Buyer_Id)
                .Index(t => t.Seller_Id);

            AddColumn("dbo.Quote", "CreatedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            Sql("UPDATE dbo.Quote SET CreatedAt = AddedAt");
            AlterColumn("dbo.Quote", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Quote", "AddedAt");

            AddColumn("dbo.Quote", "LatestAnswer_Id", c => c.Guid());
            CreateIndex("dbo.Quote", "LatestAnswer_Id");
            AddForeignKey("dbo.Quote", "LatestAnswer_Id", "dbo.QuoteAnswer", "Id");
        }

        public override void Down()
        {
            AddColumn("dbo.Quote", "AddedAt", c => c.DateTime());
            Sql("UPDATE dbo.Quote SET AddedAt = CreatedAt");
            AlterColumn("dbo.Quote", "AddedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Quote", "CreatedAt");

            DropForeignKey("dbo.Quote", "LatestAnswer_Id", "dbo.QuoteAnswer");
            DropForeignKey("dbo.QuoteAnswer", "Seller_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.QuoteAnswer", "Quote_Id", "dbo.Quote");
            DropForeignKey("dbo.QuoteAnswer", "Buyer_Id", "dbo.AspNetUsers");
            DropIndex("dbo.QuoteAnswer", new[] { "Seller_Id" });
            DropIndex("dbo.QuoteAnswer", new[] { "Buyer_Id" });
            DropIndex("dbo.QuoteAnswer", new[] { "Quote_Id" });
            DropIndex("dbo.Quote", new[] { "LatestAnswer_Id" });
            DropColumn("dbo.Quote", "LatestAnswer_Id");
            DropTable("dbo.QuoteAnswer");
        }
    }
}
