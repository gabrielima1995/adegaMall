namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_18_3_CustomFieldsV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomField", "Name_EN", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.CustomField", "Placeholder", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.CustomField", "Placeholder_EN", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.CustomField", "IsPublic", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomField", "IsMandatory", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.CustomField", "IsMandatory");
            DropColumn("dbo.CustomField", "IsPublic");
            DropColumn("dbo.CustomField", "Placeholder_EN");
            DropColumn("dbo.CustomField", "Placeholder");
            DropColumn("dbo.CustomField", "Name_EN");
        }
    }
}
