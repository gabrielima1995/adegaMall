namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_30_0_Wishlist : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductWish",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Buyer_Id = c.String(maxLength: 128, unicode: false),
                    Product_Id = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Buyer_Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Buyer_Id)
                .Index(t => t.Product_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ProductWish", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.ProductWish", "Buyer_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ProductWish", new[] { "Product_Id" });
            DropIndex("dbo.ProductWish", new[] { "Buyer_Id" });
            DropTable("dbo.ProductWish");
        }
    }
}
