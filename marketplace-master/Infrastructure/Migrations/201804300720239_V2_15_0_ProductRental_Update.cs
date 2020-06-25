namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_0_ProductRental_Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RentalLock",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Type = c.Int(nullable: false),
                    StartDate = c.DateTime(),
                    EndDate = c.DateTime(),
                    DayOfWeek = c.Int(),
                    StartTime = c.Time(precision: 7),
                    EndTime = c.Time(precision: 7),
                    Seller_Id = c.String(maxLength: 128, unicode: false),
                    Product_Id = c.Guid(),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Seller_Id)
                .Index(t => t.Seller_Id)
                .Index(t => t.Product_Id);

            DropColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodInHours");
            DropColumn("dbo.CartItem", "RentalInfo_RentalPeriod");
        }

        public override void Down()
        {
            AddColumn("dbo.CartItem", "RentalInfo_RentalPeriod", c => c.Int());
            AddColumn("dbo.OrderProduct", "RentalInfo_RentalPeriodInHours", c => c.Int());
            DropForeignKey("dbo.RentalLock", "Seller_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.RentalLock", "Product_Id", "dbo.Product");
            DropIndex("dbo.RentalLock", new[] { "Product_Id" });
            DropIndex("dbo.RentalLock", new[] { "Seller_Id" });
            DropTable("dbo.RentalLock");
        }
    }
}
