namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_22_18_SellerWhatsApp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "WhatsAppNumber", c => c.String(maxLength: 21, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "WhatsAppNumber");
        }
    }
}
