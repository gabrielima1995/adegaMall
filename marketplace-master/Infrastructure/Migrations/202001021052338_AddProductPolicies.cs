namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductPolicies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "CancelOrderPolicy", c => c.Int(nullable: false));
            AddColumn("dbo.Product", "ChangePeriodRentalPolicy", c => c.Int(nullable: false));
            AddColumn("dbo.OrderProduct", "CancelOrderPolicy", c => c.Int(nullable: false));
            AddColumn("dbo.OrderProduct", "CancelOrderPolicyHours", c => c.Int(nullable: false));
            AddColumn("dbo.OrderProduct", "ChangePeriodRentalPolicy", c => c.Int(nullable: false));
            AddColumn("dbo.OrderProduct", "ChangePeriodRentalPolicyHours", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProduct", "ChangePeriodRentalPolicyHours");
            DropColumn("dbo.OrderProduct", "ChangePeriodRentalPolicy");
            DropColumn("dbo.OrderProduct", "CancelOrderPolicyHours");
            DropColumn("dbo.OrderProduct", "CancelOrderPolicy");
            DropColumn("dbo.Product", "ChangePeriodRentalPolicy");
            DropColumn("dbo.Product", "CancelOrderPolicy");
        }
    }
}
