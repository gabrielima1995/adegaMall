namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_9_2_Hotfixes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "DeleteWhenStockZero", c => c.Boolean());
            Sql("UPDATE dbo.Product SET DeleteWhenStockZero = 0");
            AlterColumn("dbo.Product", "DeleteWhenStockZero", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "DeleteWhenStockZero");
        }
    }
}
