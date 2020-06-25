namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_31_0_Package_Product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "ProductType", c => c.Int(nullable: false));
            AddColumn("dbo.Product", "PackageCredits", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "PackageCredits");
            DropColumn("dbo.Product", "ProductType");
        }
    }
}
