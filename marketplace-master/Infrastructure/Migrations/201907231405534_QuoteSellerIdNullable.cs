namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class QuoteSellerIdNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Quote", new[] { "Seller_Id" });
            AlterColumn("dbo.Quote", "Seller_Id", c => c.String(maxLength: 128, unicode: false));
            CreateIndex("dbo.Quote", "Seller_Id");
        }

        public override void Down()
        {
            DropIndex("dbo.Quote", new[] { "Seller_Id" });
            AlterColumn("dbo.Quote", "Seller_Id", c => c.String(nullable: false, maxLength: 128, unicode: false));
            CreateIndex("dbo.Quote", "Seller_Id");
        }
    }
}
