namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_17_0_TransactionWarranty : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dispute",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    OrderId = c.String(maxLength: 8000, unicode: false),
                    BuyerId = c.String(maxLength: 8000, unicode: false),
                    SellerId = c.String(maxLength: 8000, unicode: false),
                    Status = c.Int(nullable: false),
                    MediationRequested = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.DisputeComment",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Message = c.String(maxLength: 1000, unicode: false),
                    SenderId = c.String(maxLength: 8000, unicode: false),
                    IsSystemMessage = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Dispute_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dispute", t => t.Dispute_Id)
                .Index(t => t.Dispute_Id);

            CreateTable(
                "dbo.ScheduledTransfer",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    OrderId = c.String(maxLength: 8000, unicode: false),
                    BuyerId = c.String(maxLength: 8000, unicode: false),
                    SellerId = c.String(maxLength: 8000, unicode: false),
                    FullAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    FeesAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TransferAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DueDate = c.DateTime(nullable: false),
                    OriginalDueDate = c.DateTime(nullable: false),
                    CompletionDate = c.DateTime(),
                    CancelationDate = c.DateTime(),
                    Anticipated = c.Boolean(nullable: false),
                    TransferAuthorized = c.Boolean(nullable: false),
                    Transferred = c.Boolean(nullable: false),
                    Canceled = c.Boolean(nullable: false),
                    GatewayRecipientId = c.String(maxLength: 8000, unicode: false),
                    GatewayPayableId = c.String(maxLength: 8000, unicode: false),
                    GatewayTransferId = c.String(maxLength: 8000, unicode: false),
                    Installments = c.Int(nullable: false),
                    CurrentInstallment = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.AspNetUsers", "GatewayTransferEnabled", c => c.Boolean());
            AddColumn("dbo.Order", "Dispute_Id", c => c.Guid());
            AddColumn("dbo.Order", "Deadline", c => c.DateTime());
            AddColumn("dbo.Order", "AutoCompletionDate", c => c.DateTime());
            CreateIndex("dbo.Order", "Dispute_Id");
            AddForeignKey("dbo.Order", "Dispute_Id", "dbo.Dispute", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Order", "Dispute_Id", "dbo.Dispute");
            DropForeignKey("dbo.DisputeComment", "Dispute_Id", "dbo.Dispute");
            DropIndex("dbo.DisputeComment", new[] { "Dispute_Id" });
            DropIndex("dbo.Order", new[] { "Dispute_Id" });
            DropColumn("dbo.Order", "AutoCompletionDate");
            DropColumn("dbo.Order", "Deadline");
            DropColumn("dbo.Order", "Dispute_Id");
            DropColumn("dbo.AspNetUsers", "GatewayTransferEnabled");
            DropTable("dbo.ScheduledTransfer");
            DropTable("dbo.DisputeComment");
            DropTable("dbo.Dispute");
        }
    }
}
