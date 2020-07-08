namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAsapLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Store", "AsapLogEnabled", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Store", "AsapLogUser", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.Store", "AsapLogPassword", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Store", "AsapLogPassword");
            DropColumn("dbo.Store", "AsapLogUser");
            DropColumn("dbo.Store", "AsapLogEnabled");
        }
    }
}
