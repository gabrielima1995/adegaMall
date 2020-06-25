namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductAvailability : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductAvailability",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Product_Id = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        DayOfWeek = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        CurrentStock = c.Int(nullable: false),
                        DiscountPercentage = c.Int(nullable: false),
                        DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Product_Id);

            AddColumn("dbo.OrderProduct", "RentalInfo_ProductAvailability_Id", c => c.Guid());
            CreateIndex("dbo.OrderProduct", "RentalInfo_ProductAvailability_Id");
            AddForeignKey("dbo.OrderProduct", "RentalInfo_ProductAvailability_Id", "dbo.ProductAvailability", "Id");

            AddColumn("dbo.OrderProductRentalsInfo", "ProductAvailability_Id", c => c.Guid());
            CreateIndex("dbo.OrderProductRentalsInfo", "ProductAvailability_Id");
            AddForeignKey("dbo.OrderProductRentalsInfo", "ProductAvailability_Id", "dbo.ProductAvailability", "Id");

            AddColumn("dbo.CartItem", "Discount", c => c.Decimal(precision: 18, scale: 2));
            Sql("UPDATE dbo.CartItem SET Discount = 0 WHERE Discount IS NULL");
            AlterColumn("dbo.CartItem", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));

            AddColumn("dbo.CartItem", "RentalInfo_ProductAvailability_Id", c => c.Guid());
            CreateIndex("dbo.CartItem", "RentalInfo_ProductAvailability_Id");
            AddForeignKey("dbo.CartItem", "RentalInfo_ProductAvailability_Id", "dbo.ProductAvailability", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderProduct", "RentalInfo_ProductAvailability_Id", "dbo.ProductAvailability");
            DropForeignKey("dbo.OrderProductRentalsInfo", "ProductAvailability_Id", "dbo.ProductAvailability");
            DropForeignKey("dbo.CartItem", "RentalInfo_ProductAvailability_Id", "dbo.ProductAvailability");
            DropForeignKey("dbo.ProductAvailability", "Product_Id", "dbo.Product");

            DropIndex("dbo.OrderProduct", new[] { "RentalInfo_ProductAvailability_Id" });
            DropIndex("dbo.OrderProductRentalsInfo", new[] { "ProductAvailability_Id" });
            DropIndex("dbo.CartItem", new[] { "RentalInfo_ProductAvailability_Id" });
            DropIndex("dbo.ProductAvailability", new[] { "Product_Id" });

            DropColumn("dbo.CartItem", "RentalInfo_ProductAvailability_Id");
            DropColumn("dbo.CartItem", "Discount");
            DropColumn("dbo.OrderProductRentalsInfo", "ProductAvailability_Id");
            DropColumn("dbo.OrderProduct", "RentalInfo_ProductAvailability_Id");

            DropTable("dbo.ProductAvailability");
        }
    }
}
