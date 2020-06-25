namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddCartAndOrderExtraInfoField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cart", "ExtraInfo", c => c.String(maxLength: 4000, unicode: false));
            AddColumn("dbo.Order", "ExtraInfo", c => c.String(maxLength: 4000, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Order", "ExtraInfo");
            DropColumn("dbo.Cart", "ExtraInfo");
        }
    }
}
