namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBlockChangesAfterApprovedInModeration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomField", "BlockChangesAfterApprovedInModeration", c => c.Boolean());
            Sql("UPDATE dbo.CustomField SET BlockChangesAfterApprovedInModeration = 0");
            AlterColumn("dbo.CustomField", "BlockChangesAfterApprovedInModeration", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomField", "BlockChangesAfterApprovedInModeration");
        }
    }
}
