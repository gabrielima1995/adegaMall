namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ProductWeightFixedSizeDecimal : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.Product SET Weight = 999.99 WHERE Weight > 999.999");
            AlterColumn("dbo.Product", "Weight", c => c.Decimal(precision: 6, scale: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "Weight", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
