namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_21_11_ModerationV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RequireModeration", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "RequireModeration");
        }
    }
}
