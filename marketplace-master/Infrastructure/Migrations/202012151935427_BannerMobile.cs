namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BannerMobile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Banner", "MobileImageUrl", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Banner", "MobileImageUrl");
        }
    }
}
