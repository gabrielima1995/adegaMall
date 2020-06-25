namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_21_3_InstallmentFees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "InstallmentFees", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }

        public override void Down()
        {
            DropColumn("dbo.Order", "InstallmentFees");
        }
    }
}
