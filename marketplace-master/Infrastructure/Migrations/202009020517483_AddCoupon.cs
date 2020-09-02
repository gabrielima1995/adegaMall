namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCoupon : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Coupon",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(maxLength: 8000, unicode: false),
                        Enabled = c.Boolean(nullable: false),
                        SellerId = c.String(maxLength: 128, unicode: false),
                        BenefitType = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UseMode = c.Int(nullable: false),
                        TotalMaxUse = c.Int(nullable: false),
                        TotalUse = c.Int(nullable: false),
                        DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                        Deleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Person", t => t.SellerId)
                .Index(t => t.SellerId);
            
            AddColumn("dbo.Cart", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Cart", "Coupon_Id", c => c.Guid());
            AddColumn("dbo.Order", "Coupon_Id", c => c.Guid());
            CreateIndex("dbo.Cart", "Coupon_Id");
            CreateIndex("dbo.Order", "Coupon_Id");
            AddForeignKey("dbo.Order", "Coupon_Id", "dbo.Coupon", "Id");
            AddForeignKey("dbo.Cart", "Coupon_Id", "dbo.Coupon", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cart", "Coupon_Id", "dbo.Coupon");
            DropForeignKey("dbo.Order", "Coupon_Id", "dbo.Coupon");
            DropForeignKey("dbo.Coupon", "SellerId", "dbo.Person");
            DropIndex("dbo.Coupon", new[] { "SellerId" });
            DropIndex("dbo.Order", new[] { "Coupon_Id" });
            DropIndex("dbo.Cart", new[] { "Coupon_Id" });
            DropColumn("dbo.Order", "Coupon_Id");
            DropColumn("dbo.Cart", "Coupon_Id");
            DropColumn("dbo.Cart", "Discount");
            DropTable("dbo.Coupon");
        }
    }
}
