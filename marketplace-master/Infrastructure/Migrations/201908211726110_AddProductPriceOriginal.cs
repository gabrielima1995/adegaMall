namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddProductPriceOriginal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "PriceOriginal", c => c.Decimal(precision: 18, scale: 2));
        }

        public override void Down()
        {
            DropColumn("dbo.Product", "PriceOriginal");
        }
    }
}
