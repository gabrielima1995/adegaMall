namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMinimumOrderAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SingleFreightParameter", "MinimumOrderAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SingleFreightParameter", "MinimumOrderAmount");
        }
    }
}
