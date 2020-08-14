namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTotalExpress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Store", "TotalExpressEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Store", "TotalExpressUser", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.Store", "TotalExpressPassword", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.Store", "TotalExpressReid", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Store", "TotalExpressReid");
            DropColumn("dbo.Store", "TotalExpressPassword");
            DropColumn("dbo.Store", "TotalExpressUser");
            DropColumn("dbo.Store", "TotalExpressEnabled");
        }
    }
}
