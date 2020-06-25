namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_17_1_HomeCustomItems : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.EcommerceHomeCategory", new[] { "ProductCategoryId" });
            CreateTable(
                "dbo.CustomItem",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(maxLength: 8000, unicode: false),
                    ImageUrl = c.String(maxLength: 8000, unicode: false),
                    TargetUrl = c.String(maxLength: 8000, unicode: false),
                    StartDate = c.DateTime(),
                    EndDate = c.DateTime(),
                    CategoryId = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CustomItemCategory_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomItemCategory", t => t.CustomItemCategory_Id)
                .Index(t => t.CustomItemCategory_Id);

            CreateTable(
                "dbo.CustomItemCategory",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(maxLength: 8000, unicode: false),
                    CarouselDisplay = c.Boolean(nullable: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.EcommerceHomeCategory", "CustomItemCategoryId", c => c.Guid());
            AlterColumn("dbo.EcommerceHomeCategory", "ProductCategoryId", c => c.Int());
            CreateIndex("dbo.EcommerceHomeCategory", "ProductCategoryId");
            CreateIndex("dbo.EcommerceHomeCategory", "CustomItemCategoryId");
            AddForeignKey("dbo.EcommerceHomeCategory", "CustomItemCategoryId", "dbo.CustomItemCategory", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.EcommerceHomeCategory", "CustomItemCategoryId", "dbo.CustomItemCategory");
            DropForeignKey("dbo.CustomItem", "CustomItemCategory_Id", "dbo.CustomItemCategory");
            DropIndex("dbo.EcommerceHomeCategory", new[] { "CustomItemCategoryId" });
            DropIndex("dbo.EcommerceHomeCategory", new[] { "ProductCategoryId" });
            DropIndex("dbo.CustomItem", new[] { "CustomItemCategory_Id" });
            AlterColumn("dbo.EcommerceHomeCategory", "ProductCategoryId", c => c.Int(nullable: false));
            DropColumn("dbo.EcommerceHomeCategory", "CustomItemCategoryId");
            DropTable("dbo.CustomItemCategory");
            DropTable("dbo.CustomItem");
            CreateIndex("dbo.EcommerceHomeCategory", "ProductCategoryId");
        }
    }
}
