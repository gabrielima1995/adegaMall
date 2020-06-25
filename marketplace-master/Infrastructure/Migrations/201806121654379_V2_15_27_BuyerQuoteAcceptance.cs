namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_27_BuyerQuoteAcceptance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quote", "Deadline", c => c.DateTime());
            AddColumn("dbo.Quote", "AcceptedPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.QuoteAnswer", "DeliveryDeadline", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.QuoteAnswer", "DeliveryDeadline");
            DropColumn("dbo.Quote", "AcceptedPrice");
            DropColumn("dbo.Quote", "Deadline");
        }
    }
}
