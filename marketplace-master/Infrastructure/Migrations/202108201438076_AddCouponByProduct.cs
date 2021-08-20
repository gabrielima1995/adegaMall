namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCouponByProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Coupon", "ProductId", c => c.Guid());
            CreateIndex("dbo.Coupon", "ProductId");
            AddForeignKey("dbo.Coupon", "ProductId", "dbo.Product", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Coupon", "ProductId", "dbo.Product");
            DropIndex("dbo.Coupon", new[] { "ProductId" });
            DropColumn("dbo.Coupon", "ProductId");
        }
    }
}
