namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_32_0_ShortQuote : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Quote", new[] { "Category_Id" });
            DropIndex("dbo.Quote", new[] { "SubCategory_Id" });
            AddColumn("dbo.Quote", "PortfolioItem_Id", c => c.Guid());
            AddColumn("dbo.Quote", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.Quote", "IsShortQuote", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Quote", "Category_Id", c => c.Int());
            AlterColumn("dbo.Quote", "SubCategory_Id", c => c.Int());
            CreateIndex("dbo.Quote", "Category_Id");
            CreateIndex("dbo.Quote", "SubCategory_Id");
            CreateIndex("dbo.Quote", "PortfolioItem_Id");
            AddForeignKey("dbo.Quote", "PortfolioItem_Id", "dbo.PortfolioItem", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Quote", "PortfolioItem_Id", "dbo.PortfolioItem");
            DropIndex("dbo.Quote", new[] { "PortfolioItem_Id" });
            DropIndex("dbo.Quote", new[] { "SubCategory_Id" });
            DropIndex("dbo.Quote", new[] { "Category_Id" });
            AlterColumn("dbo.Quote", "SubCategory_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Quote", "Category_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Quote", "IsShortQuote");
            DropColumn("dbo.Quote", "Quantity");
            DropColumn("dbo.Quote", "PortfolioItem_Id");
            CreateIndex("dbo.Quote", "SubCategory_Id");
            CreateIndex("dbo.Quote", "Category_Id");
        }
    }
}
