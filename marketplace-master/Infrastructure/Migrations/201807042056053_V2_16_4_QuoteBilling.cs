namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_16_4_QuoteBilling : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuoteBilling",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    QuoteId = c.Guid(nullable: false),
                    Amount = c.Decimal(nullable: false, precision: 7, scale: 2),
                    Deadline = c.DateTime(nullable: false),
                    SellerId = c.String(maxLength: 128, unicode: false),
                    BuyerId = c.String(maxLength: 128, unicode: false),
                    ProductId = c.Guid(),
                    OrderId = c.Guid(),
                    IsCancelled = c.Boolean(nullable: false),
                    IsPaid = c.Boolean(nullable: false),
                    IsRefunded = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.QuoteBilling");
        }
    }
}
