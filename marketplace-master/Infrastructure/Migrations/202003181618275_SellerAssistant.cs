namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SellerAssistant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "AssistantToSellerId", c => c.String(maxLength: 128, unicode: false));
            AddColumn("dbo.AspNetUsers", "ProfileId", c => c.String(maxLength: 128, unicode: false));
            AddColumn("dbo.AspNetUsers", "ProfileType", c => c.String(maxLength: 30, unicode: false));

            Sql(@"
                UPDATE dbo.AspNetUsers SET
                    ProfileId = COALESCE(AdminId, BuyerId, SellerId),
                    ProfileType = (CASE
                        WHEN AdminId IS NOT NULL THEN 'AdminPerson'
                        WHEN BuyerId IS NOT NULL THEN 'Buyer'
                        WHEN SellerId IS NOT NULL THEN 'Seller' END)");

            DropColumn("dbo.AspNetUsers", "AdminId");
            DropColumn("dbo.AspNetUsers", "BuyerId");
            DropColumn("dbo.AspNetUsers", "SellerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "SellerId", c => c.String(maxLength: 128, unicode: false));
            AddColumn("dbo.AspNetUsers", "BuyerId", c => c.String(maxLength: 128, unicode: false));
            AddColumn("dbo.AspNetUsers", "AdminId", c => c.String(maxLength: 128, unicode: false));

            Sql(@"
                UPDATE dbo.AspNetUsers SET
                    AdminId = (CASE WHEN ProfileType = 'AdminPerson' THEN ProfileId ELSE NULL END),
                    BuyerId = (CASE WHEN ProfileType = 'Buyer' THEN ProfileId ELSE NULL END),
                    SellerId = (CASE WHEN ProfileType = 'Seller' THEN ProfileId ELSE NULL END)");

            DropColumn("dbo.AspNetUsers", "ProfileType");
            DropColumn("dbo.AspNetUsers", "ProfileId");
            DropColumn("dbo.Person", "AssistantToSellerId");
        }
    }
}
