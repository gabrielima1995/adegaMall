namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_14_8_FinancePanelV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransferJournal",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Type = c.Int(nullable: false),
                    OrderId = c.Guid(nullable: false),
                    BuyerId = c.String(maxLength: 128, unicode: false),
                    SellerId = c.String(maxLength: 128, unicode: false),
                    PaymentMethod = c.String(maxLength: 30, unicode: false),
                    OrderDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    OrderTotal = c.Decimal(nullable: false, precision: 7, scale: 2),
                    CommissionPct = c.Decimal(nullable: false, precision: 3, scale: 2),
                    InstallmentsCount = c.Short(nullable: false),
                    InstallmentNumber = c.Short(nullable: false),
                    InstallmentAmount = c.Decimal(nullable: false, precision: 7, scale: 2),
                    InstallmentCommissionAmount = c.Decimal(nullable: false, precision: 7, scale: 2),
                    TransferAmount = c.Decimal(nullable: false, precision: 7, scale: 2),
                    DueDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Completed = c.Boolean(nullable: false),
                    CompletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    GatewayReceivableId = c.String(maxLength: 50, unicode: false),
                    GatewayReceiverId = c.String(maxLength: 50, unicode: false),
                    GatewayTransactionId = c.String(maxLength: 50, unicode: false),
                    Cancelled = c.Boolean(nullable: false),
                    CancelledDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    AdditionalNotes = c.String(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            AlterColumn("dbo.AspNetUsers", "PreferredTimeZoneId", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "PreferredTimeZoneId", c => c.Int());
            DropTable("dbo.TransferJournal");
        }
    }
}
