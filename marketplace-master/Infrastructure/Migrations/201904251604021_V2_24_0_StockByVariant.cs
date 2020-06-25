namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V2_24_0_StockByVariant : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockProductVariant",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CurrentStock = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Variant_Id = c.Int(nullable: false),
                    Product_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductVariant", t => t.Variant_Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Variant_Id)
                .Index(t => t.Product_Id);

            AddColumn("dbo.Product", "StockByVariant", c => c.Boolean(nullable: false));
            AddColumn("dbo.Product", "StockVariantType_Id", c => c.Int());
            AddColumn("dbo.OrderProduct", "ByVariant", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderProduct", "Variant_Id", c => c.Int());
            AddColumn("dbo.OrderProduct", "VariantName", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.OrderProduct", "VariantTypeName", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.StockTransaction", "Variant_Id", c => c.Int());
            AddColumn("dbo.CartItem", "ByVariant", c => c.Boolean(nullable: false));
            AddColumn("dbo.CartItem", "Variant_Id", c => c.Int());
            CreateIndex("dbo.Product", "StockVariantType_Id");
            CreateIndex("dbo.OrderProduct", "Variant_Id");
            CreateIndex("dbo.StockTransaction", "Variant_Id");
            CreateIndex("dbo.CartItem", "Variant_Id");
            AddForeignKey("dbo.Product", "StockVariantType_Id", "dbo.ProductVariantType", "Id");
            AddForeignKey("dbo.OrderProduct", "Variant_Id", "dbo.ProductVariant", "Id");
            AddForeignKey("dbo.StockTransaction", "Variant_Id", "dbo.ProductVariant", "Id");
            AddForeignKey("dbo.CartItem", "Variant_Id", "dbo.ProductVariant", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.CartItem", "Variant_Id", "dbo.ProductVariant");
            DropForeignKey("dbo.StockProductVariant", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.StockTransaction", "Variant_Id", "dbo.ProductVariant");
            DropForeignKey("dbo.OrderProduct", "Variant_Id", "dbo.ProductVariant");
            DropForeignKey("dbo.Product", "StockVariantType_Id", "dbo.ProductVariantType");
            DropForeignKey("dbo.StockProductVariant", "Variant_Id", "dbo.ProductVariant");
            DropIndex("dbo.CartItem", new[] { "Variant_Id" });
            DropIndex("dbo.StockTransaction", new[] { "Variant_Id" });
            DropIndex("dbo.StockProductVariant", new[] { "Product_Id" });
            DropIndex("dbo.StockProductVariant", new[] { "Variant_Id" });
            DropIndex("dbo.OrderProduct", new[] { "Variant_Id" });
            DropIndex("dbo.Product", new[] { "StockVariantType_Id" });
            DropColumn("dbo.CartItem", "Variant_Id");
            DropColumn("dbo.CartItem", "ByVariant");
            DropColumn("dbo.StockTransaction", "Variant_Id");
            DropColumn("dbo.OrderProduct", "VariantTypeName");
            DropColumn("dbo.OrderProduct", "VariantName");
            DropColumn("dbo.OrderProduct", "Variant_Id");
            DropColumn("dbo.OrderProduct", "ByVariant");
            DropColumn("dbo.Product", "StockVariantType_Id");
            DropColumn("dbo.Product", "StockByVariant");
            DropTable("dbo.StockProductVariant");
        }
    }
}
