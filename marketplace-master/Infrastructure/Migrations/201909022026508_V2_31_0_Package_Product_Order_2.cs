namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_31_0_Package_Product_Order_2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderProductRentalsInfo",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    OrderProduct_Id = c.Guid(nullable: false),
                    StartDate = c.DateTime(),
                    EndDate = c.DateTime(),
                    RentalPeriodUnit = c.Int(),
                    RentalPeriodValue = c.Decimal(precision: 18, scale: 2),
                    ChargeDoubleShipping = c.Boolean(),
                    IsReturned = c.Boolean(),
                    ReturnedAt = c.DateTime(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderProduct", t => t.OrderProduct_Id)
                .Index(t => t.OrderProduct_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.OrderProductRentalsInfo", "OrderProduct_Id", "dbo.OrderProduct");
            DropIndex("dbo.OrderProductRentalsInfo", new[] { "OrderProduct_Id" });
            DropTable("dbo.OrderProductRentalsInfo");
        }
    }
}
