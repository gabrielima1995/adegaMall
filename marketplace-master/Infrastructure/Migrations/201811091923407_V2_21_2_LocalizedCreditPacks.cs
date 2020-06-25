namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_21_2_LocalizedCreditPacks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditsPackTransaction", "CreditsBundle_Id", c => c.Guid());
            AddColumn("dbo.CreditsPack", "Name_EN", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.CreditsPack", "Description_EN", c => c.String(maxLength: 8000, unicode: false));
            CreateIndex("dbo.CreditsPackTransaction", "CreditsBundle_Id");
            AddForeignKey("dbo.CreditsPackTransaction", "CreditsBundle_Id", "dbo.CreditsBundle", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.CreditsPackTransaction", "CreditsBundle_Id", "dbo.CreditsBundle");
            DropIndex("dbo.CreditsPackTransaction", new[] { "CreditsBundle_Id" });
            DropColumn("dbo.CreditsPack", "Description_EN");
            DropColumn("dbo.CreditsPack", "Name_EN");
            DropColumn("dbo.CreditsPackTransaction", "CreditsBundle_Id");
        }
    }
}
