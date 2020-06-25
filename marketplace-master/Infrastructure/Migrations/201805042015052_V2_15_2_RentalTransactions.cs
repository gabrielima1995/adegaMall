namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_2_RentalTransactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RentalTransaction",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Type = c.Int(nullable: false),
                    DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    OrderProduct_Id = c.Guid(),
                    Product_Id = c.Guid(nullable: false),
                    Order_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderProduct", t => t.OrderProduct_Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .Index(t => t.OrderProduct_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.Order_Id);

            AddColumn("dbo.Product", "CurrentRentalStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }

        public override void Down()
        {
            DropForeignKey("dbo.RentalTransaction", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.RentalTransaction", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.RentalTransaction", "OrderProduct_Id", "dbo.OrderProduct");
            DropIndex("dbo.RentalTransaction", new[] { "Order_Id" });
            DropIndex("dbo.RentalTransaction", new[] { "Product_Id" });
            DropIndex("dbo.RentalTransaction", new[] { "OrderProduct_Id" });
            DropColumn("dbo.Product", "CurrentRentalStock");
            DropTable("dbo.RentalTransaction");
        }
    }
}
