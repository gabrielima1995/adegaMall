namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_26_0_ProductRentalShifts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RentalShift",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 50, unicode: false),
                    StartHours = c.Int(nullable: false),
                    StartMinutes = c.Int(nullable: false),
                    EndHours = c.Int(nullable: false),
                    EndMinutes = c.Int(nullable: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ProductRentalShift",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    ProductId = c.Guid(nullable: false),
                    RentalShiftId = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.ProductId, t.RentalShiftId }, unique: true);

        }

        public override void Down()
        {
            DropIndex("dbo.ProductRentalShift", new[] { "ProductId", "RentalShiftId" });
            DropTable("dbo.ProductRentalShift");
            DropTable("dbo.RentalShift");
        }
    }
}
