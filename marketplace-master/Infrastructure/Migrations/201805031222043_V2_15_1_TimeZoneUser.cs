namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_1_TimeZoneUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "PreferredTimeZoneId", c => c.String(maxLength: 8000, unicode: false));
            Sql("UPDATE dbo.AspNetUsers SET PreferredTimeZoneId = 'E. South America Standard Time'");
        }

        public override void Down()
        {
            Sql("UPDATE dbo.AspNetUsers SET PreferredTimeZoneId = '910'");
            AlterColumn("dbo.AspNetUsers", "PreferredTimeZoneId", c => c.Int(nullable: false));
        }
    }
}
