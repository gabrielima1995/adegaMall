namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterProductToIntegrateWithGoogle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "MustBeIntegrate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Product", "LastIntegration", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "LastIntegration");
            DropColumn("dbo.Product", "MustBeIntegrate");
        }
    }
}
