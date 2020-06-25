namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_9_1_QuoteGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quote", "GroupId", c => c.Guid());

            AddColumn("dbo.SubCategory", "DeletedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));

            AddColumn("dbo.SubCategory", "CreatedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
            Sql("UPDATE dbo.SubCategory SET CreatedAt = '2017-01-01' WHERE CreatedAt IS NULL");
            AlterColumn("dbo.SubCategory", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            DropColumn("dbo.Quote", "GroupId");
            DropColumn("dbo.SubCategory", "CreatedAt");
            DropColumn("dbo.SubCategory", "DeletedAt");
        }
    }
}
