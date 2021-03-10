namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderTransactionRefuseReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditsPackTransaction", "RefuseReason", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.OrderTransaction", "RefuseReason", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderTransaction", "RefuseReason");
            DropColumn("dbo.CreditsPackTransaction", "RefuseReason");
        }
    }
}
