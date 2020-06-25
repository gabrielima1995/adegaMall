namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_17_2_CustomQuoteFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomField",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(maxLength: 8000, unicode: false),
                    Order = c.Int(nullable: false),
                    DataType = c.Int(nullable: false),
                    EntityType = c.Int(nullable: false),
                    InputType = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CustomFieldOption",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CustomField_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomField", t => t.CustomField_Id)
                .Index(t => t.CustomField_Id);

            CreateTable(
                "dbo.CustomFieldValue",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    EntityId = c.Guid(nullable: false),
                    Value = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CustomField_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomField", t => t.CustomField_Id)
                .Index(t => t.CustomField_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.CustomFieldValue", "CustomField_Id", "dbo.CustomField");
            DropForeignKey("dbo.CustomFieldOption", "CustomField_Id", "dbo.CustomField");
            DropIndex("dbo.CustomFieldValue", new[] { "CustomField_Id" });
            DropIndex("dbo.CustomFieldOption", new[] { "CustomField_Id" });
            DropTable("dbo.CustomFieldValue");
            DropTable("dbo.CustomFieldOption");
            DropTable("dbo.CustomField");
        }
    }
}
