namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddPersonUtmTags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UtmTags", c => c.String(maxLength: 1500, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UtmTags");
        }
    }
}
