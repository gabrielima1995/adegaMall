namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_18_6_InstallmentPerProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "MaxInstallments", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "MaxInstallments");
        }
    }
}
