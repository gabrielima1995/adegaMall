namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddTableGatewayRegistrationLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GatewayRegistrationLog",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    SellerId = c.String(maxLength: 36, unicode: false),
                    Gateway = c.String(maxLength: 10, unicode: false),
                    GatewayRecipientId = c.String(maxLength: 50, unicode: false),
                    GatewayBankAccountId = c.String(maxLength: 50, unicode: false),
                    GatewayApiToken = c.String(maxLength: 50, unicode: false),
                    ExtraInfo = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.GatewayRegistrationLog");
        }
    }
}
