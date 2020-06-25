namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_12_0_IuguPaymentIntegration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "GatewayLiveApiToken", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.AspNetUsers", "GatewayTestApiToken", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.AspNetUsers", "GatewayUserToken", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.AspNetUsers", "AccountableName", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.AspNetUsers", "AccountableDocumentNumber", c => c.String(maxLength: 20, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AccountableDocumentNumber");
            DropColumn("dbo.AspNetUsers", "AccountableName");
            DropColumn("dbo.AspNetUsers", "GatewayUserToken");
            DropColumn("dbo.AspNetUsers", "GatewayTestApiToken");
            DropColumn("dbo.AspNetUsers", "GatewayLiveApiToken");
        }
    }
}
