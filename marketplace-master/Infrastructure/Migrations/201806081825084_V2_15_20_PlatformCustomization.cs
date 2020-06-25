namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_20_PlatformCustomization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlatformCustomization",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    HtmlHead = c.String(),
                    HtmlBodyStart = c.String(),
                    HtmlBodyEnd = c.String(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.PlatformCustomization");
        }
    }
}
