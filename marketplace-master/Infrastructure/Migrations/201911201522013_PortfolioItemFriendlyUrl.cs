namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PortfolioItemFriendlyUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PortfolioItem", "FriendlyUrl", c => c.String(maxLength: 1000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PortfolioItem", "FriendlyUrl");
        }
    }
}
