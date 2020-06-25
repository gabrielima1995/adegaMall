namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_14_24_PostalShippingPerSeller : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Store", "SedexEnabled", c => c.Boolean(nullable: true));
            Sql("UPDATE dbo.Store SET SedexEnabled = 0");
            AlterColumn("dbo.Store", "SedexEnabled", c => c.Boolean(nullable: false));

            AddColumn("dbo.Store", "PacEnabled", c => c.Boolean(nullable: true));
            Sql("UPDATE dbo.Store SET PacEnabled = 0");
            AlterColumn("dbo.Store", "PacEnabled", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Store", "PacEnabled");
            DropColumn("dbo.Store", "SedexEnabled");
        }
    }
}
