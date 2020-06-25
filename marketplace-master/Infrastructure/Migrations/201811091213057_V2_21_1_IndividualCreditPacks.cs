namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_21_1_IndividualCreditPacks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditsPackTransaction",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Status = c.String(maxLength: 8000, unicode: false),
                    Message = c.String(maxLength: 8000, unicode: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RefundedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    StartupFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RefundedStartupFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    StartupSplitId = c.String(maxLength: 8000, unicode: false),
                    GatewayFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RefundedGatewayFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PostbackUrl = c.String(maxLength: 8000, unicode: false),
                    BoletoUrl = c.String(maxLength: 8000, unicode: false),
                    BoletoBarCode = c.String(maxLength: 8000, unicode: false),
                    BoletoExpirationDate = c.DateTime(),
                    Installments = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    LeadSalesProfile_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LeadSalesProfile", t => t.LeadSalesProfile_Id)
                .Index(t => t.LeadSalesProfile_Id);

            CreateTable(
                "dbo.CreditsPack",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(maxLength: 8000, unicode: false),
                    Description = c.String(maxLength: 8000, unicode: false),
                    CreditsAmount = c.Int(nullable: false),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    ValidityDays = c.Int(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.CreditsPackTransaction", "LeadSalesProfile_Id", "dbo.LeadSalesProfile");
            DropIndex("dbo.CreditsPackTransaction", new[] { "LeadSalesProfile_Id" });
            DropTable("dbo.CreditsPack");
            DropTable("dbo.CreditsPackTransaction");
        }
    }
}
