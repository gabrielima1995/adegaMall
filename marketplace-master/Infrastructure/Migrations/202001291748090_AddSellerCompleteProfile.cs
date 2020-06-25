namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSellerCompleteProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SellerCompleteProfile", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.AspNetUsers", "SellerCompleteProfileVersion", c => c.String(maxLength: 2, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SellerCompleteProfileVersion");
            DropColumn("dbo.AspNetUsers", "SellerCompleteProfile");
        }
    }
}
