namespace Infrastructure.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationConfig",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    BRL_USD = c.Decimal(nullable: false, precision: 8, scale: 5),
                    MaaSConfig = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Banner",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    ImageUrl = c.String(maxLength: 8000, unicode: false),
                    ActionUrl = c.String(maxLength: 8000, unicode: false),
                    StartsAt = c.DateTime(),
                    ExpiresAt = c.DateTime(),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Cart",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    ShippingMethod = c.Int(nullable: false),
                    ShippingMode = c.Int(nullable: false),
                    Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Completed = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Buyer_Id = c.String(maxLength: 128, unicode: false),
                    ShippingAddress_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Buyer_Id)
                .ForeignKey("dbo.Address", t => t.ShippingAddress_Id)
                .Index(t => t.Buyer_Id)
                .Index(t => t.ShippingAddress_Id);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    FullName = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false),
                    Enabled = c.Boolean(nullable: false),
                    Email = c.String(maxLength: 256, unicode: false),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(maxLength: 8000, unicode: false),
                    SecurityStamp = c.String(maxLength: 8000, unicode: false),
                    PhoneNumber = c.String(maxLength: 8000, unicode: false),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256, unicode: false),
                    Avatar = c.String(maxLength: 8000, unicode: false),
                    LoginMode = c.Int(),
                    DateOfBirth = c.DateTime(),
                    Genre = c.Int(),
                    DocumentNumber = c.String(maxLength: 8000, unicode: false),
                    AdditionalInfo1 = c.String(maxLength: 8000, unicode: false),
                    AdditionalInfo2 = c.String(maxLength: 8000, unicode: false),
                    GatewayIdOld = c.String(maxLength: 8000, unicode: false),
                    SupplierGatewayId = c.String(maxLength: 8000, unicode: false),
                    CustomerGatewayId = c.String(maxLength: 8000, unicode: false),
                    Rating = c.Int(),
                    TotalRating = c.Int(),
                    TotalReviews = c.Int(),
                    Score = c.Int(),
                    AboutMe = c.String(maxLength: 8000, unicode: false),
                    NotAllowedTrial = c.Boolean(),
                    Lang = c.String(maxLength: 8000, unicode: false),
                    Nickname = c.String(maxLength: 8000, unicode: false),
                    SubscriptionStarted = c.Boolean(),
                    AlternativeName = c.String(maxLength: 8000, unicode: false),
                    FacebookProfile = c.String(maxLength: 8000, unicode: false),
                    LinkedInProfile = c.String(maxLength: 8000, unicode: false),
                    InstagramProfile = c.String(maxLength: 8000, unicode: false),
                    GooglePlusProfile = c.String(maxLength: 8000, unicode: false),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                    BankAccount_Id = c.Guid(),
                    Address_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccount", t => t.BankAccount_Id)
                .ForeignKey("dbo.Address", t => t.Address_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.BankAccount_Id)
                .Index(t => t.Address_Id);

            CreateTable(
                "dbo.Address",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Street = c.String(maxLength: 8000, unicode: false),
                    Number = c.String(maxLength: 8000, unicode: false),
                    AdditionalInfo = c.String(maxLength: 8000, unicode: false),
                    Neighborhood = c.String(maxLength: 8000, unicode: false),
                    ZipCode = c.String(maxLength: 8000, unicode: false),
                    CityAndState = c.String(maxLength: 8000, unicode: false),
                    Location = c.Geography(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128, unicode: false),
                    ClaimType = c.String(maxLength: 8000, unicode: false),
                    ClaimValue = c.String(maxLength: 8000, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128, unicode: false),
                    ProviderKey = c.String(nullable: false, maxLength: 128, unicode: false),
                    UserId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Quote",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(maxLength: 8000, unicode: false),
                    Description = c.String(maxLength: 8000, unicode: false),
                    Phone = c.String(maxLength: 8000, unicode: false),
                    Date = c.DateTime(),
                    Preference = c.Int(nullable: false),
                    Amount = c.Decimal(precision: 18, scale: 2),
                    AddedAt = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                    Address_Id = c.Guid(nullable: false),
                    Buyer_Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    Category_Id = c.Int(nullable: false),
                    Seller_Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    SubCategory_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.Address_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Buyer_Id)
                .ForeignKey("dbo.Category", t => t.Category_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Seller_Id)
                .ForeignKey("dbo.SubCategory", t => t.SubCategory_Id)
                .Index(t => t.Address_Id)
                .Index(t => t.Buyer_Id)
                .Index(t => t.Category_Id)
                .Index(t => t.Seller_Id)
                .Index(t => t.SubCategory_Id);

            CreateTable(
                "dbo.Category",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 8000, unicode: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.BankAccount",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Bank = c.Int(nullable: false),
                    AccountType = c.Int(nullable: false),
                    Branch = c.String(maxLength: 8000, unicode: false),
                    Number = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Portfolio",
                c => new
                {
                    SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.SellerId)
                .ForeignKey("dbo.AspNetUsers", t => t.SellerId)
                .Index(t => t.SellerId);

            CreateTable(
                "dbo.PortfolioItem",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(maxLength: 8000, unicode: false),
                    Description = c.String(maxLength: 8000, unicode: false),
                    VideoUrl = c.String(maxLength: 8000, unicode: false),
                    Price = c.Decimal(precision: 18, scale: 2),
                    CoverImageUrl = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Portfolio_SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Portfolio", t => t.Portfolio_SellerId)
                .Index(t => t.Portfolio_SellerId);

            CreateTable(
                "dbo.PortfolioItemImage",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Url = c.String(maxLength: 8000, unicode: false),
                    Position = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    PortfolioItem_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PortfolioItem", t => t.PortfolioItem_Id)
                .Index(t => t.PortfolioItem_Id);

            CreateTable(
                "dbo.SellerSocialMedia",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ResourceId = c.String(maxLength: 8000, unicode: false),
                    Nickname = c.String(maxLength: 8000, unicode: false),
                    Url = c.String(maxLength: 8000, unicode: false),
                    Followers = c.Int(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                    Portfolio_SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                    SocialNetwork_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Portfolio", t => t.Portfolio_SellerId)
                .ForeignKey("dbo.SocialNetwork", t => t.SocialNetwork_Id)
                .Index(t => t.Portfolio_SellerId)
                .Index(t => t.SocialNetwork_Id);

            CreateTable(
                "dbo.SocialNetwork",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 8000, unicode: false),
                    Image = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SubCategory",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 8000, unicode: false),
                    Deleted = c.Boolean(nullable: false),
                    Category_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.Category_Id)
                .Index(t => t.Category_Id);

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128, unicode: false),
                    RoleId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.ShippingAddress",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Address_Id = c.Guid(nullable: false),
                    Person_Id = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.Address_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Person_Id)
                .Index(t => t.Address_Id)
                .Index(t => t.Person_Id);

            CreateTable(
                "dbo.Store",
                c => new
                {
                    SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                    OnSitePickupEnabled = c.Boolean(nullable: false),
                    PrivateFleetEnabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.SellerId)
                .ForeignKey("dbo.AspNetUsers", t => t.SellerId)
                .Index(t => t.SellerId);

            CreateTable(
                "dbo.Product",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(maxLength: 8000, unicode: false),
                    Description = c.String(maxLength: 8000, unicode: false),
                    Specifications = c.String(maxLength: 8000, unicode: false),
                    Price = c.Decimal(precision: 18, scale: 2),
                    CoverImageUrl = c.String(maxLength: 8000, unicode: false),
                    UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CurrentStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PrivateFleetEnabled = c.Boolean(nullable: false),
                    SedexEnabled = c.Boolean(nullable: false),
                    PACEnabled = c.Boolean(nullable: false),
                    OnSitePickupEnabled = c.Boolean(nullable: false),
                    NoShippingEnabled = c.Boolean(nullable: false),
                    NoShippingUrl = c.String(maxLength: 8000, unicode: false),
                    Length = c.Decimal(precision: 18, scale: 2),
                    Width = c.Decimal(precision: 18, scale: 2),
                    Height = c.Decimal(precision: 18, scale: 2),
                    Weight = c.Decimal(precision: 18, scale: 2),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Category_Id = c.Int(),
                    Store_SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategory", t => t.Category_Id)
                .ForeignKey("dbo.Store", t => t.Store_SellerId)
                .Index(t => t.Category_Id)
                .Index(t => t.Store_SellerId);

            CreateTable(
                "dbo.ProductCategory",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 8000, unicode: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ProductImage",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Url = c.String(maxLength: 8000, unicode: false),
                    Position = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Product_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Product_Id);

            CreateTable(
                "dbo.PrivateFleetParameter",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DeliveryRange = c.Double(nullable: false),
                    Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Product_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Product_Id);

            CreateTable(
                "dbo.StockTransaction",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Type = c.Int(nullable: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Order_Id = c.Guid(),
                    Product_Id = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.Product_Id);

            CreateTable(
                "dbo.Order",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    ShippingMethod = c.Int(nullable: false),
                    Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    StartupFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    ProcessingGatewayFee = c.Boolean(nullable: false),
                    SplitId = c.String(maxLength: 8000, unicode: false),
                    PaymentMethod = c.Int(nullable: false),
                    Status = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Buyer_Id = c.String(maxLength: 128, unicode: false),
                    Seller_Id = c.String(maxLength: 128, unicode: false),
                    ShippingAddress_Id = c.Guid(),
                    Transaction_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Buyer_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Seller_Id)
                .ForeignKey("dbo.Address", t => t.ShippingAddress_Id)
                .ForeignKey("dbo.OrderTransaction", t => t.Transaction_Id)
                .Index(t => t.Buyer_Id)
                .Index(t => t.Seller_Id)
                .Index(t => t.ShippingAddress_Id)
                .Index(t => t.Transaction_Id);

            CreateTable(
                "dbo.OrderHistory",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    OccurredOn = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                    AdditionalInfo = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Order_Id = c.Guid(),
                    Person_Id = c.String(maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Person_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.Person_Id);

            CreateTable(
                "dbo.OrderProduct",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Order_Id = c.Guid(nullable: false),
                    Item = c.String(maxLength: 8000, unicode: false),
                    Image = c.String(maxLength: 8000, unicode: false),
                    Category = c.String(maxLength: 8000, unicode: false),
                    Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    NoShippingUrl = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Product_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.Product_Id);

            CreateTable(
                "dbo.OrderReview",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Rate = c.Int(nullable: false),
                    Description = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Order_Id = c.Guid(),
                    Reviewer_Id = c.String(maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Reviewer_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.Reviewer_Id);

            CreateTable(
                "dbo.OrderTransaction",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TransactionId = c.String(maxLength: 8000, unicode: false),
                    Status = c.String(maxLength: 8000, unicode: false),
                    Message = c.String(maxLength: 8000, unicode: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RefundedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    StartupFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RefundedStartupFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    StartupSplitId = c.String(maxLength: 8000, unicode: false),
                    GatewayFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RefundedGatewayFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PostbackUrl = c.String(maxLength: 8000, unicode: false),
                    BoletoUrl = c.String(maxLength: 8000, unicode: false),
                    BoletoBarCode = c.String(maxLength: 8000, unicode: false),
                    BoletoExpirationDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ProductVariant",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 8000, unicode: false),
                    Order = c.Int(nullable: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Type_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductVariantType", t => t.Type_Id)
                .Index(t => t.Type_Id);

            CreateTable(
                "dbo.ProductVariantType",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 8000, unicode: false),
                    Order = c.Int(nullable: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SingleFreightParameter",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DeliveryRange = c.Double(nullable: false),
                    Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Store_SellerId = c.String(maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Store", t => t.Store_SellerId)
                .Index(t => t.Store_SellerId);

            CreateTable(
                "dbo.Subscription",
                c => new
                {
                    SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                    Status = c.Int(nullable: false),
                    StartDate = c.DateTime(nullable: false),
                    FinishDate = c.DateTime(),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Currency = c.String(maxLength: 8000, unicode: false),
                    ReferenceId = c.String(maxLength: 8000, unicode: false),
                    PostbackUrl = c.String(maxLength: 8000, unicode: false),
                    Plan_Id = c.Guid(),
                })
                .PrimaryKey(t => t.SellerId)
                .ForeignKey("dbo.Plan", t => t.Plan_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.SellerId)
                .Index(t => t.SellerId)
                .Index(t => t.Plan_Id);

            CreateTable(
                "dbo.Plan",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(maxLength: 8000, unicode: false),
                    NameLabel = c.String(maxLength: 8000, unicode: false),
                    DescriptionLabel = c.String(maxLength: 8000, unicode: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Days = c.Int(nullable: false),
                    TrialDays = c.Int(nullable: false),
                    GatewayId = c.String(maxLength: 8000, unicode: false),
                    Enabled = c.Boolean(nullable: false),
                    DisabledAt = c.DateTime(),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SubscriptionTransaction",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    ReferenceId = c.String(maxLength: 8000, unicode: false),
                    Status = c.String(maxLength: 8000, unicode: false),
                    Date = c.DateTime(nullable: false),
                    Content = c.String(maxLength: 8000, unicode: false),
                    SubsReferenceId = c.String(maxLength: 8000, unicode: false),
                    Subscription_SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subscription", t => t.Subscription_SellerId)
                .Index(t => t.Subscription_SellerId);

            CreateTable(
                "dbo.CartItem",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    AddedAt = c.DateTime(nullable: false),
                    Quantity = c.Int(nullable: false),
                    Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Cart_Id = c.Guid(),
                    Product_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cart", t => t.Cart_Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Cart_Id)
                .Index(t => t.Product_Id);

            CreateTable(
                "dbo.CartSeller",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    SellerShipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Cart_Id = c.Guid(nullable: false),
                    Seller_Id = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cart", t => t.Cart_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Seller_Id)
                .Index(t => t.Cart_Id)
                .Index(t => t.Seller_Id);

            CreateTable(
                "dbo.City",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 8000, unicode: false),
                    State = c.String(maxLength: 8000, unicode: false),
                    Location = c.Geography(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.EcommerceHome",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.EcommerceHomeCategory",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    EcommerceHomeId = c.Guid(nullable: false),
                    ProductCategoryId = c.Int(nullable: false),
                    Order = c.Int(nullable: false),
                    DeletedAt = c.DateTime(precision: 7, storeType: "datetime2"),
                    Deleted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EcommerceHome", t => t.EcommerceHomeId)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategoryId)
                .Index(t => t.EcommerceHomeId)
                .Index(t => t.ProductCategoryId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    Name = c.String(nullable: false, maxLength: 256, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.Str",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Label = c.String(maxLength: 8000, unicode: false),
                    Lang = c.Int(nullable: false),
                    Value = c.String(maxLength: 8000, unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SubCategoryPortfolio",
                c => new
                {
                    SubCategory_Id = c.Int(nullable: false),
                    Portfolio_SellerId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => new { t.SubCategory_Id, t.Portfolio_SellerId })
                .ForeignKey("dbo.SubCategory", t => t.SubCategory_Id)
                .ForeignKey("dbo.Portfolio", t => t.Portfolio_SellerId)
                .Index(t => t.SubCategory_Id)
                .Index(t => t.Portfolio_SellerId);

            CreateTable(
                "dbo.ProductVariantProduct",
                c => new
                {
                    ProductVariant_Id = c.Int(nullable: false),
                    Product_Id = c.Guid(nullable: false),
                })
                .PrimaryKey(t => new { t.ProductVariant_Id, t.Product_Id })
                .ForeignKey("dbo.ProductVariant", t => t.ProductVariant_Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.ProductVariant_Id)
                .Index(t => t.Product_Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.EcommerceHomeCategory", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.EcommerceHomeCategory", "EcommerceHomeId", "dbo.EcommerceHome");
            DropForeignKey("dbo.Cart", "ShippingAddress_Id", "dbo.Address");
            DropForeignKey("dbo.CartSeller", "Seller_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CartSeller", "Cart_Id", "dbo.Cart");
            DropForeignKey("dbo.CartItem", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.CartItem", "Cart_Id", "dbo.Cart");
            DropForeignKey("dbo.Cart", "Buyer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Quote", "SubCategory_Id", "dbo.SubCategory");
            DropForeignKey("dbo.Quote", "Seller_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SubscriptionTransaction", "Subscription_SellerId", "dbo.Subscription");
            DropForeignKey("dbo.Subscription", "SellerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Subscription", "Plan_Id", "dbo.Plan");
            DropForeignKey("dbo.SingleFreightParameter", "Store_SellerId", "dbo.Store");
            DropForeignKey("dbo.Store", "SellerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductVariant", "Type_Id", "dbo.ProductVariantType");
            DropForeignKey("dbo.ProductVariantProduct", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.ProductVariantProduct", "ProductVariant_Id", "dbo.ProductVariant");
            DropForeignKey("dbo.Product", "Store_SellerId", "dbo.Store");
            DropForeignKey("dbo.StockTransaction", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.Order", "Transaction_Id", "dbo.OrderTransaction");
            DropForeignKey("dbo.StockTransaction", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.Order", "ShippingAddress_Id", "dbo.Address");
            DropForeignKey("dbo.Order", "Seller_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderReview", "Reviewer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderReview", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.OrderProduct", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.OrderProduct", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.OrderHistory", "Person_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderHistory", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.Order", "Buyer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PrivateFleetParameter", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.ProductImage", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.Product", "Category_Id", "dbo.ProductCategory");
            DropForeignKey("dbo.ShippingAddress", "Person_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Address_Id", "dbo.Address");
            DropForeignKey("dbo.ShippingAddress", "Address_Id", "dbo.Address");
            DropForeignKey("dbo.SubCategoryPortfolio", "Portfolio_SellerId", "dbo.Portfolio");
            DropForeignKey("dbo.SubCategoryPortfolio", "SubCategory_Id", "dbo.SubCategory");
            DropForeignKey("dbo.SubCategory", "Category_Id", "dbo.Category");
            DropForeignKey("dbo.SellerSocialMedia", "SocialNetwork_Id", "dbo.SocialNetwork");
            DropForeignKey("dbo.SellerSocialMedia", "Portfolio_SellerId", "dbo.Portfolio");
            DropForeignKey("dbo.Portfolio", "SellerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PortfolioItem", "Portfolio_SellerId", "dbo.Portfolio");
            DropForeignKey("dbo.PortfolioItemImage", "PortfolioItem_Id", "dbo.PortfolioItem");
            DropForeignKey("dbo.AspNetUsers", "BankAccount_Id", "dbo.BankAccount");
            DropForeignKey("dbo.Quote", "Category_Id", "dbo.Category");
            DropForeignKey("dbo.Quote", "Buyer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Quote", "Address_Id", "dbo.Address");
            DropIndex("dbo.ProductVariantProduct", new[] { "Product_Id" });
            DropIndex("dbo.ProductVariantProduct", new[] { "ProductVariant_Id" });
            DropIndex("dbo.SubCategoryPortfolio", new[] { "Portfolio_SellerId" });
            DropIndex("dbo.SubCategoryPortfolio", new[] { "SubCategory_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.EcommerceHomeCategory", new[] { "ProductCategoryId" });
            DropIndex("dbo.EcommerceHomeCategory", new[] { "EcommerceHomeId" });
            DropIndex("dbo.CartSeller", new[] { "Seller_Id" });
            DropIndex("dbo.CartSeller", new[] { "Cart_Id" });
            DropIndex("dbo.CartItem", new[] { "Product_Id" });
            DropIndex("dbo.CartItem", new[] { "Cart_Id" });
            DropIndex("dbo.SubscriptionTransaction", new[] { "Subscription_SellerId" });
            DropIndex("dbo.Subscription", new[] { "Plan_Id" });
            DropIndex("dbo.Subscription", new[] { "SellerId" });
            DropIndex("dbo.SingleFreightParameter", new[] { "Store_SellerId" });
            DropIndex("dbo.ProductVariant", new[] { "Type_Id" });
            DropIndex("dbo.OrderReview", new[] { "Reviewer_Id" });
            DropIndex("dbo.OrderReview", new[] { "Order_Id" });
            DropIndex("dbo.OrderProduct", new[] { "Product_Id" });
            DropIndex("dbo.OrderProduct", new[] { "Order_Id" });
            DropIndex("dbo.OrderHistory", new[] { "Person_Id" });
            DropIndex("dbo.OrderHistory", new[] { "Order_Id" });
            DropIndex("dbo.Order", new[] { "Transaction_Id" });
            DropIndex("dbo.Order", new[] { "ShippingAddress_Id" });
            DropIndex("dbo.Order", new[] { "Seller_Id" });
            DropIndex("dbo.Order", new[] { "Buyer_Id" });
            DropIndex("dbo.StockTransaction", new[] { "Product_Id" });
            DropIndex("dbo.StockTransaction", new[] { "Order_Id" });
            DropIndex("dbo.PrivateFleetParameter", new[] { "Product_Id" });
            DropIndex("dbo.ProductImage", new[] { "Product_Id" });
            DropIndex("dbo.Product", new[] { "Store_SellerId" });
            DropIndex("dbo.Product", new[] { "Category_Id" });
            DropIndex("dbo.Store", new[] { "SellerId" });
            DropIndex("dbo.ShippingAddress", new[] { "Person_Id" });
            DropIndex("dbo.ShippingAddress", new[] { "Address_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.SubCategory", new[] { "Category_Id" });
            DropIndex("dbo.SellerSocialMedia", new[] { "SocialNetwork_Id" });
            DropIndex("dbo.SellerSocialMedia", new[] { "Portfolio_SellerId" });
            DropIndex("dbo.PortfolioItemImage", new[] { "PortfolioItem_Id" });
            DropIndex("dbo.PortfolioItem", new[] { "Portfolio_SellerId" });
            DropIndex("dbo.Portfolio", new[] { "SellerId" });
            DropIndex("dbo.Quote", new[] { "SubCategory_Id" });
            DropIndex("dbo.Quote", new[] { "Seller_Id" });
            DropIndex("dbo.Quote", new[] { "Category_Id" });
            DropIndex("dbo.Quote", new[] { "Buyer_Id" });
            DropIndex("dbo.Quote", new[] { "Address_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Address_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "BankAccount_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Cart", new[] { "ShippingAddress_Id" });
            DropIndex("dbo.Cart", new[] { "Buyer_Id" });
            DropTable("dbo.ProductVariantProduct");
            DropTable("dbo.SubCategoryPortfolio");
            DropTable("dbo.Str");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.EcommerceHomeCategory");
            DropTable("dbo.EcommerceHome");
            DropTable("dbo.City");
            DropTable("dbo.CartSeller");
            DropTable("dbo.CartItem");
            DropTable("dbo.SubscriptionTransaction");
            DropTable("dbo.Plan");
            DropTable("dbo.Subscription");
            DropTable("dbo.SingleFreightParameter");
            DropTable("dbo.ProductVariantType");
            DropTable("dbo.ProductVariant");
            DropTable("dbo.OrderTransaction");
            DropTable("dbo.OrderReview");
            DropTable("dbo.OrderProduct");
            DropTable("dbo.OrderHistory");
            DropTable("dbo.Order");
            DropTable("dbo.StockTransaction");
            DropTable("dbo.PrivateFleetParameter");
            DropTable("dbo.ProductImage");
            DropTable("dbo.ProductCategory");
            DropTable("dbo.Product");
            DropTable("dbo.Store");
            DropTable("dbo.ShippingAddress");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.SubCategory");
            DropTable("dbo.SocialNetwork");
            DropTable("dbo.SellerSocialMedia");
            DropTable("dbo.PortfolioItemImage");
            DropTable("dbo.PortfolioItem");
            DropTable("dbo.Portfolio");
            DropTable("dbo.BankAccount");
            DropTable("dbo.Category");
            DropTable("dbo.Quote");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Address");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Cart");
            DropTable("dbo.Banner");
            DropTable("dbo.ApplicationConfig");
        }
    }
}
