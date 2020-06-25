namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRecurringProductAvailability : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAvailability", "Recurring_DaysOfWeekInline", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.ProductAvailability", "Recurring_Unit", c => c.Int(nullable: false));
            AddColumn("dbo.ProductAvailability", "Recurring_UnitValue", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductAvailability", "Recurring_UnitValue");
            DropColumn("dbo.ProductAvailability", "Recurring_Unit");
            DropColumn("dbo.ProductAvailability", "Recurring_DaysOfWeekInline");
        }
    }
}
