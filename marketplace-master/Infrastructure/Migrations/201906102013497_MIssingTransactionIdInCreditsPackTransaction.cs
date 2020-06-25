namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MIssingTransactionIdInCreditsPackTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditsPackTransaction", "TransactionId", c => c.String(maxLength: 8000, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.CreditsPackTransaction", "TransactionId");
        }
    }
}
