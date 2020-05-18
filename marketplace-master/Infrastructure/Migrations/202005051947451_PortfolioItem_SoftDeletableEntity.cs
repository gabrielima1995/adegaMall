namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PortfolioItem_SoftDeletableEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PortfolioItem", "DeletedAt", c => c.DateTime());

            AddColumn("dbo.PortfolioItem", "Deleted", c => c.Boolean());
            Sql("UPDATE dbo.PortfolioItem SET Deleted = 0");
            AlterColumn("dbo.PortfolioItem", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PortfolioItem", "Deleted");
            DropColumn("dbo.PortfolioItem", "DeletedAt");
        }
    }
}
