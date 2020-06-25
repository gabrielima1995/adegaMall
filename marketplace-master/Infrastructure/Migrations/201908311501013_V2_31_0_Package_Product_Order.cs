namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_31_0_Package_Product_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProduct", "ProductType", c => c.Int(nullable: false));
            AddColumn("dbo.OrderProduct", "PackageCredits", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.OrderProduct", "PackageCredits");
            DropColumn("dbo.OrderProduct", "ProductType");
        }
    }
}
