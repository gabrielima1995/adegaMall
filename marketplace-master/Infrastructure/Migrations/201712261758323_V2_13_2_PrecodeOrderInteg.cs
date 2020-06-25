namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_13_2_PrecodeOrderInteg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderTransaction", "Installments", c => c.Int());
            Sql("UPDATE dbo.OrderTransaction SET Installments = 1 WHERE Installments IS NULL");
            AlterColumn("dbo.OrderTransaction", "Installments", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.OrderTransaction", "Installments");
        }
    }
}
