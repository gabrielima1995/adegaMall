namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_16_1_ActualNoShipping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "ActualNoShippingEnabled", c => c.Boolean(nullable: true));
            Sql("UPDATE dbo.Product SET ActualNoShippingEnabled = 0");
            AlterColumn("dbo.Product", "ActualNoShippingEnabled", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "ActualNoShippingEnabled");
        }
    }
}
