namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_17_15_DisableProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "IsDisabled", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "IsDisabled");
        }
    }
}
