namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateIntegrationWithGoogleShopping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IntegrationParameterFromGoogle",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountId = c.String(maxLength: 200, unicode: false),
                        ClientId = c.String(maxLength: 200, unicode: false),
                        ClientEmail = c.String(maxLength: 300, unicode: false),
                        PrivateKey = c.String(maxLength: 8000, unicode: false),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IntegrationParameterFromGoogle");
        }
    }
}
