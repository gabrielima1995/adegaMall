namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_13_6_QuoteAttachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuoteAttachment",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    QuoteId = c.Guid(nullable: false),
                    QuoteAnswerId = c.Guid(),
                    AttachmentUrl = c.String(maxLength: 1000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuoteAnswer", t => t.QuoteAnswerId)
                .ForeignKey("dbo.Quote", t => t.QuoteId)
                .Index(t => t.QuoteId)
                .Index(t => t.QuoteAnswerId);

            AddColumn("dbo.Quote", "IsClosed", c => c.Boolean());
            Sql("UPDATE dbo.Quote SET IsClosed = CONVERT(BIT, Status)");
            AlterColumn("dbo.Quote", "IsClosed", c => c.Boolean(nullable: false));

            AddColumn("dbo.Quote", "ClosedAt", c => c.DateTime());
            AddColumn("dbo.Quote", "DealResult", c => c.Int());
            Sql("UPDATE dbo.Quote SET ClosedAt = GETUTCDATE(), DealResult = 2 WHERE IsClosed = 1");
        }

        public override void Down()
        {
            DropForeignKey("dbo.QuoteAttachment", "QuoteId", "dbo.Quote");
            DropForeignKey("dbo.QuoteAttachment", "QuoteAnswerId", "dbo.QuoteAnswer");
            DropIndex("dbo.QuoteAttachment", new[] { "QuoteAnswerId" });
            DropIndex("dbo.QuoteAttachment", new[] { "QuoteId" });
            DropColumn("dbo.Quote", "DealResult");
            DropColumn("dbo.Quote", "ClosedAt");
            DropColumn("dbo.Quote", "IsClosed");
            DropTable("dbo.QuoteAttachment");
        }
    }
}
