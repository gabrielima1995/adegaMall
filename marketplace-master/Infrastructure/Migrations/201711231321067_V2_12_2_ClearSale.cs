namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_12_2_ClearSale : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AntifraudTransaction",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TransactionId = c.String(maxLength: 50, unicode: false),
                    Status = c.String(maxLength: 150, unicode: false),
                    Message = c.String(maxLength: 1000, unicode: false),
                    Content = c.String(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.Order", "CartId", c => c.Guid());
            AddColumn("dbo.Order", "AntifraudTransactionId", c => c.Guid());

            AddColumn("dbo.Order", "IsPaid", c => c.Boolean());
            Sql(@"
                UPDATE [Order] SET IsPaid = 0
                UPDATE [Order] SET [Order].IsPaid = 1
                    FROM [Order]
                    INNER JOIN OrderHistory ON [Order].Id = OrderHistory.Order_Id AND OrderHistory.[Status] = 1 /*completed_payment*/");
            AlterColumn("dbo.Order", "IsPaid", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Order", "IsPaid");
            DropColumn("dbo.Order", "AntifraudTransactionId");
            DropColumn("dbo.Order", "CartId");
            DropTable("dbo.AntifraudTransaction");
        }
    }
}
