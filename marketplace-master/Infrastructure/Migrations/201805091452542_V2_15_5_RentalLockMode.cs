namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_15_5_RentalLockMode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RentalLock", "Mode", c => c.Int(nullable: true));
            Sql("UPDATE dbo.RentalLock SET Mode = 0");
            AlterColumn("dbo.RentalLock", "Mode", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.RentalLock", "Mode");
        }
    }
}
