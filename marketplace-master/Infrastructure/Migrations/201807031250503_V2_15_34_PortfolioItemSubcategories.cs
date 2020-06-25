namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_34_PortfolioItemSubcategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PortfolioItemSubcategories",
                c => new
                {
                    PortfolioItem_Id = c.Guid(nullable: false),
                    SubCategory_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.PortfolioItem_Id, t.SubCategory_Id })
                .ForeignKey("dbo.PortfolioItem", t => t.PortfolioItem_Id)
                .ForeignKey("dbo.SubCategory", t => t.SubCategory_Id)
                .Index(t => t.PortfolioItem_Id)
                .Index(t => t.SubCategory_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PortfolioItemSubcategories", "SubCategory_Id", "dbo.SubCategory");
            DropForeignKey("dbo.PortfolioItemSubcategories", "PortfolioItem_Id", "dbo.PortfolioItem");
            DropIndex("dbo.PortfolioItemSubcategories", new[] { "SubCategory_Id" });
            DropIndex("dbo.PortfolioItemSubcategories", new[] { "PortfolioItem_Id" });
            DropTable("dbo.PortfolioItemSubcategories");
        }
    }
}
