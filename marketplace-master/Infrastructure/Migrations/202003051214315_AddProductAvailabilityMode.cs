namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductAvailabilityMode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "ProductAvailabilityMode", c => c.Int());
            Sql("UPDATE dbo.Product SET ProductAvailabilityMode = 0 WHERE ProductAvailabilityMode IS NULL");
            AlterColumn("dbo.Product", "ProductAvailabilityMode", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "ProductAvailabilityMode");
        }
    }
}
