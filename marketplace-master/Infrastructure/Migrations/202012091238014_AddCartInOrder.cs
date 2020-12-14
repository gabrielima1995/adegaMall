namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCartInOrder : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Order", "CartId");
            AddForeignKey("dbo.Order", "CartId", "dbo.Cart", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order", "CartId", "dbo.Cart");
            DropIndex("dbo.Order", new[] { "CartId" });
        }
    }
}
