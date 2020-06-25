namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_19_2_LeadSales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeadSalesProfile",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    SellerId = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CreditsBundle",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    ExpirationDate = c.DateTime(nullable: false),
                    InitialCredits = c.Int(nullable: false),
                    CurrentCredits = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    LeadSalesProfile_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LeadSalesProfile", t => t.LeadSalesProfile_Id)
                .Index(t => t.LeadSalesProfile_Id);

            CreateTable(
                "dbo.UnlockedQuote",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    UnlockedQuoteId = c.Guid(nullable: false),
                    LeadSalesProfileId = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LeadSalesProfile", t => t.LeadSalesProfileId)
                .Index(t => t.LeadSalesProfileId);

            AddColumn("dbo.AspNetUsers", "LeadSalesProfile_Id", c => c.Guid());
            CreateIndex("dbo.AspNetUsers", "LeadSalesProfile_Id");
            AddForeignKey("dbo.AspNetUsers", "LeadSalesProfile_Id", "dbo.LeadSalesProfile", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "LeadSalesProfile_Id", "dbo.LeadSalesProfile");
            DropForeignKey("dbo.UnlockedQuote", "LeadSalesProfileId", "dbo.LeadSalesProfile");
            DropForeignKey("dbo.CreditsBundle", "LeadSalesProfile_Id", "dbo.LeadSalesProfile");
            DropIndex("dbo.UnlockedQuote", new[] { "LeadSalesProfileId" });
            DropIndex("dbo.CreditsBundle", new[] { "LeadSalesProfile_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "LeadSalesProfile_Id" });
            DropColumn("dbo.AspNetUsers", "LeadSalesProfile_Id");
            DropTable("dbo.UnlockedQuote");
            DropTable("dbo.CreditsBundle");
            DropTable("dbo.LeadSalesProfile");
        }
    }
}
