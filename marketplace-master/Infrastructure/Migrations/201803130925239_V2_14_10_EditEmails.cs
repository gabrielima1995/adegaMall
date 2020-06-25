namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_14_10_EditEmails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageTemplate",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Key = c.String(maxLength: 100, unicode: false),
                    Lang = c.String(maxLength: 7, unicode: false),
                    SubjectTemplate = c.String(maxLength: 256, unicode: false),
                    BodyTemplate = c.String(maxLength: 8000, unicode: false),
                    Enabled = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.MessageTemplate");
        }
    }
}
