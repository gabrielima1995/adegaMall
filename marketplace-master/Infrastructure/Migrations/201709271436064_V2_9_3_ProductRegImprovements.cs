namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_9_3_ProductRegImprovements : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "CustomId", c => c.String(maxLength: 100, unicode: false));
            AlterColumn("dbo.Product", "Description", c => c.String());
            AlterColumn("dbo.Product", "Specifications", c => c.String());
        }

        public override void Down()
        {
            Sql(@"
                UPDATE dbo.Product SET Specifications = SUBSTRING(Specifications, 1, 8000) WHERE LEN(Specifications) > 8000
                UPDATE dbo.Product SET Description = SUBSTRING(Description, 1, 8000) WHERE LEN(Description) > 8000");

            AlterColumn("dbo.Product", "Specifications", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("dbo.Product", "Description", c => c.String(maxLength: 8000, unicode: false));
            DropColumn("dbo.Product", "CustomId");
        }
    }
}
