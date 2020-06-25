namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonModifiedAt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ModifiedAt", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ModifiedAt");
        }
    }
}
