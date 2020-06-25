namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_21_10_DoubleBlindRating : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserConfigurationRaw",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    RawConfig = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.UserRating", "PairedRating", c => c.Guid());
            AddColumn("dbo.UserRating", "IsCompleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserRating", "IsPairCompleted", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.UserRating", "IsPairCompleted");
            DropColumn("dbo.UserRating", "IsCompleted");
            DropColumn("dbo.UserRating", "PairedRating");
            DropTable("dbo.UserConfigurationRaw");
        }
    }
}
