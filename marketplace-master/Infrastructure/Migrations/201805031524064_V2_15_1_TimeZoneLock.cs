namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_1_TimeZoneLock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RentalLock", "TimeZoneId", c => c.String(maxLength: 8000, unicode: false));
            Sql("UPDATE dbo.RentalLock SET TimeZoneId = 'E. South America Standard Time' WHERE TimeZoneId IS NULL");
        }

        public override void Down()
        {
            DropColumn("dbo.RentalLock", "TimeZoneId");
        }
    }
}
