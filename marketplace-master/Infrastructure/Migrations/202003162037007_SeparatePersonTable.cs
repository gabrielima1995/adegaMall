namespace Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeparatePersonTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[Subscription] DROP CONSTRAINT [FK_dbo.Subscription_dbo.AspNetUsers_SellerId]");
            Sql("ALTER TABLE [dbo].[Store] DROP CONSTRAINT [FK_dbo.Store_dbo.AspNetUsers_SellerId]");
            Sql("ALTER TABLE [dbo].[ShippingAddress] DROP CONSTRAINT [FK_dbo.ShippingAddress_dbo.AspNetUsers_Person_Id]");
            Sql("ALTER TABLE [dbo].[RentalLock] DROP CONSTRAINT [FK_dbo.RentalLock_dbo.AspNetUsers_Seller_Id]");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] DROP CONSTRAINT [FK_dbo.QuoteAnswer_dbo.AspNetUsers_Seller_Id]");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] DROP CONSTRAINT [FK_dbo.QuoteAnswer_dbo.AspNetUsers_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[Quote] DROP CONSTRAINT [FK_dbo.Quote_dbo.AspNetUsers_Seller_Id]");
            Sql("ALTER TABLE [dbo].[Quote] DROP CONSTRAINT [FK_dbo.Quote_dbo.AspNetUsers_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[ProductWish] DROP CONSTRAINT [FK_dbo.ProductWish_dbo.AspNetUsers_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[Portfolio] DROP CONSTRAINT [FK_dbo.Portfolio_dbo.AspNetUsers_SellerId]");
            Sql("ALTER TABLE [dbo].[OrderReview] DROP CONSTRAINT [FK_dbo.OrderReview_dbo.AspNetUsers_Reviewer_Id]");
            Sql("ALTER TABLE [dbo].[OrderHistory] DROP CONSTRAINT [FK_dbo.OrderHistory_dbo.AspNetUsers_Person_Id]");
            Sql("ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_dbo.Order_dbo.AspNetUsers_Seller_Id]");
            Sql("ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_dbo.Order_dbo.AspNetUsers_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[CartSeller] DROP CONSTRAINT [FK_dbo.CartSeller_dbo.AspNetUsers_Seller_Id]");
            Sql("ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK_dbo.Cart_dbo.AspNetUsers_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[AspNetUsers] DROP CONSTRAINT [FK_dbo.AspNetUsers_dbo.LeadSalesProfile_LeadSalesProfile_Id]");
            Sql("ALTER TABLE [dbo].[AspNetUsers] DROP CONSTRAINT [FK_dbo.AspNetUsers_dbo.BankAccount_BankAccount_Id]");
            Sql("ALTER TABLE [dbo].[AspNetUsers] DROP CONSTRAINT [FK_dbo.AspNetUsers_dbo.Address_Address_Id]");
            Sql("ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]");
            Sql("ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]");
            Sql("ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]");
            Sql("ALTER TABLE [dbo].[ApiToken] DROP CONSTRAINT [FK_dbo.ApiToken_dbo.AspNetUsers_Seller_Id]");

            Sql("DROP INDEX [UserNameIndex] ON [dbo].[AspNetUsers]");
            Sql("DROP INDEX [IX_LeadSalesProfile_Id] ON [dbo].[AspNetUsers]");
            Sql("DROP INDEX [IX_BankAccount_Id] ON [dbo].[AspNetUsers]");
            Sql("DROP INDEX [IX_Address_Id] ON [dbo].[AspNetUsers]");

            RenameTable(name: "dbo.AspNetUsers", newName: "Person");
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    AdminId = c.String(maxLength: 128, unicode: false),
                    BuyerId = c.String(maxLength: 128, unicode: false),
                    SellerId = c.String(maxLength: 128, unicode: false),
                    LoginMode = c.Int(nullable: false),
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
                })
                .PrimaryKey(t => t.Id);

            Sql("UPDATE dbo.Person SET Rating = 0 WHERE Rating IS NULL");
            Sql("UPDATE dbo.Person SET TotalRating = 0 WHERE TotalRating IS NULL");
            Sql("UPDATE dbo.Person SET TotalReviews = 0 WHERE TotalReviews IS NULL");
            Sql("UPDATE dbo.Person SET Score = 0 WHERE Score IS NULL");
            Sql("UPDATE dbo.Person SET NotAllowedTrial = 0 WHERE NotAllowedTrial IS NULL");
            Sql("UPDATE dbo.Person SET Discriminator = 'AdminPerson' WHERE Discriminator IN ('AdminUser', 'Person')");

            Sql(@"
	            INSERT INTO dbo.AspNetUsers (
		            AdminId, BuyerId, SellerId,
		            Id, LoginMode, Enabled, Email, EmailConfirmed, PasswordHash,
		            SecurityStamp, PhoneNumber, PhoneNumberConfirmed,
		            TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled,
		            AccessFailedCount, UserName)
	            SELECT
		            CASE WHEN Discriminator = 'AdminPerson' THEN Id ELSE NULL END,
		            CASE WHEN Discriminator = 'Buyer' THEN Id ELSE NULL END,
		            CASE WHEN Discriminator = 'Seller' THEN Id ELSE NULL END,
		            Id, ISNULL(LoginMode, 0), Enabled, Email, EmailConfirmed, PasswordHash,
		            SecurityStamp, PhoneNumber, PhoneNumberConfirmed,
		            TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled,
		            AccessFailedCount, UserName
	            FROM dbo.Person");

            AlterColumn("dbo.Person", "UserName", c => c.String(maxLength: 256, unicode: false));
            AlterColumn("dbo.Person", "Rating", c => c.Int(nullable: false));
            AlterColumn("dbo.Person", "TotalRating", c => c.Int(nullable: false));
            AlterColumn("dbo.Person", "TotalReviews", c => c.Int(nullable: false));
            AlterColumn("dbo.Person", "Score", c => c.Int(nullable: false));
            AlterColumn("dbo.Person", "NotAllowedTrial", c => c.Boolean(nullable: false));

            DropColumn("dbo.Person", "EmailConfirmed");
            DropColumn("dbo.Person", "PasswordHash");
            DropColumn("dbo.Person", "SecurityStamp");
            DropColumn("dbo.Person", "PhoneNumberConfirmed");
            DropColumn("dbo.Person", "TwoFactorEnabled");
            DropColumn("dbo.Person", "LockoutEndDateUtc");
            DropColumn("dbo.Person", "LockoutEnabled");
            DropColumn("dbo.Person", "AccessFailedCount");
            DropColumn("dbo.Person", "LoginMode");

            Sql("CREATE NONCLUSTERED INDEX [IX_Address_Id] ON [dbo].[Person] ([Address_Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            Sql("CREATE NONCLUSTERED INDEX [IX_BankAccount_Id] ON [dbo].[Person] ([BankAccount_Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            Sql("CREATE NONCLUSTERED INDEX [IX_LeadSalesProfile_Id] ON [dbo].[Person] ([LeadSalesProfile_Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            Sql("CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([UserName] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            Sql("ALTER TABLE [dbo].[ApiToken]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApiToken_dbo.AspNetUsers_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[ApiToken] CHECK CONSTRAINT [FK_dbo.ApiToken_dbo.AspNetUsers_Seller_Id]");

            Sql("ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]");

            Sql("ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]");

            Sql("ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]");

            Sql("ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Person_dbo.Address_Address_Id] FOREIGN KEY([Address_Id]) REFERENCES [dbo].[Address] ([Id])");
            Sql("ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_dbo.Person_dbo.Address_Address_Id]");

            Sql("ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Person_dbo.BankAccount_BankAccount_Id] FOREIGN KEY([BankAccount_Id]) REFERENCES [dbo].[BankAccount] ([Id])");
            Sql("ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_dbo.Person_dbo.BankAccount_BankAccount_Id]");

            Sql("ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Person_dbo.LeadSalesProfile_LeadSalesProfile_Id] FOREIGN KEY([LeadSalesProfile_Id]) REFERENCES [dbo].[LeadSalesProfile] ([Id])");
            Sql("ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_dbo.Person_dbo.LeadSalesProfile_LeadSalesProfile_Id]");

            Sql("ALTER TABLE [dbo].[Cart]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Cart_dbo.Person_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Cart] CHECK CONSTRAINT [FK_dbo.Cart_dbo.Person_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[CartSeller]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CartSeller_dbo.Person_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[CartSeller] CHECK CONSTRAINT [FK_dbo.CartSeller_dbo.Person_Seller_Id]");

            Sql("ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.Person_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.Person_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.Person_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.Person_Seller_Id]");

            Sql("ALTER TABLE [dbo].[OrderHistory]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderHistory_dbo.Person_Person_Id] FOREIGN KEY([Person_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[OrderHistory] CHECK CONSTRAINT [FK_dbo.OrderHistory_dbo.Person_Person_Id]");

            Sql("ALTER TABLE [dbo].[OrderReview]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderReview_dbo.Person_Reviewer_Id] FOREIGN KEY([Reviewer_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[OrderReview] CHECK CONSTRAINT [FK_dbo.OrderReview_dbo.Person_Reviewer_Id]");

            Sql("ALTER TABLE [dbo].[Portfolio]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Portfolio_dbo.Person_SellerId] FOREIGN KEY([SellerId]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Portfolio] CHECK CONSTRAINT [FK_dbo.Portfolio_dbo.Person_SellerId]");

            Sql("ALTER TABLE [dbo].[ProductWish]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProductWish_dbo.Person_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[ProductWish] CHECK CONSTRAINT [FK_dbo.ProductWish_dbo.Person_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Quote_dbo.Person_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_dbo.Quote_dbo.Person_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Quote_dbo.Person_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_dbo.Quote_dbo.Person_Seller_Id]");

            Sql("ALTER TABLE [dbo].[QuoteAnswer]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuoteAnswer_dbo.Person_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] CHECK CONSTRAINT [FK_dbo.QuoteAnswer_dbo.Person_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[QuoteAnswer]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuoteAnswer_dbo.Person_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] CHECK CONSTRAINT [FK_dbo.QuoteAnswer_dbo.Person_Seller_Id]");

            Sql("ALTER TABLE [dbo].[RentalLock]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RentalLock_dbo.Person_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[RentalLock] CHECK CONSTRAINT [FK_dbo.RentalLock_dbo.Person_Seller_Id]");

            Sql("ALTER TABLE [dbo].[ShippingAddress]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ShippingAddress_dbo.Person_Person_Id] FOREIGN KEY([Person_Id]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[ShippingAddress] CHECK CONSTRAINT [FK_dbo.ShippingAddress_dbo.Person_Person_Id]");

            Sql("ALTER TABLE [dbo].[Store]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Store_dbo.Person_SellerId] FOREIGN KEY([SellerId]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Store] CHECK CONSTRAINT [FK_dbo.Store_dbo.Person_SellerId]");

            Sql("ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Subscription_dbo.Person_SellerId] FOREIGN KEY([SellerId]) REFERENCES [dbo].[Person] ([Id])");
            Sql("ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_dbo.Subscription_dbo.Person_SellerId]");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[Subscription] DROP CONSTRAINT [FK_dbo.Subscription_dbo.Person_SellerId]");
            Sql("ALTER TABLE [dbo].[Store] DROP CONSTRAINT [FK_dbo.Store_dbo.Person_SellerId]");
            Sql("ALTER TABLE [dbo].[ShippingAddress] DROP CONSTRAINT [FK_dbo.ShippingAddress_dbo.Person_Person_Id]");
            Sql("ALTER TABLE [dbo].[RentalLock] DROP CONSTRAINT [FK_dbo.RentalLock_dbo.Person_Seller_Id]");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] DROP CONSTRAINT [FK_dbo.QuoteAnswer_dbo.Person_Seller_Id]");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] DROP CONSTRAINT [FK_dbo.QuoteAnswer_dbo.Person_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[Quote] DROP CONSTRAINT [FK_dbo.Quote_dbo.Person_Seller_Id]");
            Sql("ALTER TABLE [dbo].[Quote] DROP CONSTRAINT [FK_dbo.Quote_dbo.Person_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[ProductWish] DROP CONSTRAINT [FK_dbo.ProductWish_dbo.Person_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[Portfolio] DROP CONSTRAINT [FK_dbo.Portfolio_dbo.Person_SellerId]");
            Sql("ALTER TABLE [dbo].[OrderReview] DROP CONSTRAINT [FK_dbo.OrderReview_dbo.Person_Reviewer_Id]");
            Sql("ALTER TABLE [dbo].[OrderHistory] DROP CONSTRAINT [FK_dbo.OrderHistory_dbo.Person_Person_Id]");
            Sql("ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_dbo.Order_dbo.Person_Seller_Id]");
            Sql("ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_dbo.Order_dbo.Person_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[CartSeller] DROP CONSTRAINT [FK_dbo.CartSeller_dbo.Person_Seller_Id]");
            Sql("ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK_dbo.Cart_dbo.Person_Buyer_Id]");
            Sql("ALTER TABLE [dbo].[Person] DROP CONSTRAINT [FK_dbo.Person_dbo.LeadSalesProfile_LeadSalesProfile_Id]");
            Sql("ALTER TABLE [dbo].[Person] DROP CONSTRAINT [FK_dbo.Person_dbo.BankAccount_BankAccount_Id]");
            Sql("ALTER TABLE [dbo].[Person] DROP CONSTRAINT [FK_dbo.Person_dbo.Address_Address_Id]");
            Sql("ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]");
            Sql("ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]");
            Sql("ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]");
            Sql("ALTER TABLE [dbo].[ApiToken] DROP CONSTRAINT [FK_dbo.ApiToken_dbo.AspNetUsers_Seller_Id]");

            Sql("DROP INDEX [UserNameIndex] ON [dbo].[AspNetUsers]");
            Sql("DROP INDEX [IX_LeadSalesProfile_Id] ON [dbo].[Person]");
            Sql("DROP INDEX [IX_BankAccount_Id] ON [dbo].[Person]");
            Sql("DROP INDEX [IX_Address_Id] ON [dbo].[Person]");

            AddColumn("dbo.Person", "LoginMode", c => c.Int());
            AddColumn("dbo.Person", "AccessFailedCount", c => c.Int());
            AddColumn("dbo.Person", "LockoutEnabled", c => c.Boolean());
            AddColumn("dbo.Person", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.Person", "TwoFactorEnabled", c => c.Boolean());
            AddColumn("dbo.Person", "PhoneNumberConfirmed", c => c.Boolean());
            AddColumn("dbo.Person", "SecurityStamp", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.Person", "PasswordHash", c => c.String(maxLength: 8000, unicode: false));
            AddColumn("dbo.Person", "EmailConfirmed", c => c.Boolean());

            Sql(@"
                UPDATE dbo.Person SET
                    LoginMode = dbo.AspNetUsers.LoginMode,
                    AccessFailedCount = dbo.AspNetUsers.AccessFailedCount,
                    LockoutEnabled = dbo.AspNetUsers.LockoutEnabled,
                    LockoutEndDateUtc = dbo.AspNetUsers.LockoutEndDateUtc,
                    TwoFactorEnabled = dbo.AspNetUsers.TwoFactorEnabled,
                    PhoneNumberConfirmed = dbo.AspNetUsers.PhoneNumberConfirmed,
                    SecurityStamp = dbo.AspNetUsers.SecurityStamp,
                    PasswordHash = dbo.AspNetUsers.PasswordHash,
                    EmailConfirmed = dbo.AspNetUsers.EmailConfirmed,
                    UserName = dbo.AspNetUsers.UserName,
                    Email = dbo.AspNetUsers.UserName
                FROM dbo.Person
                INNER JOIN dbo.AspNetUsers ON dbo.Person.Id = dbo.AspNetUsers.Id");

            AlterColumn("dbo.Person", "LoginMode", c => c.Int(nullable: false));
            AlterColumn("dbo.Person", "AccessFailedCount", c => c.Int(nullable: false));
            AlterColumn("dbo.Person", "LockoutEnabled", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Person", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Person", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Person", "EmailConfirmed", c => c.Boolean(nullable: false));

            AlterColumn("dbo.Person", "NotAllowedTrial", c => c.Boolean());
            AlterColumn("dbo.Person", "Score", c => c.Int());
            AlterColumn("dbo.Person", "TotalReviews", c => c.Int());
            AlterColumn("dbo.Person", "TotalRating", c => c.Int());
            AlterColumn("dbo.Person", "Rating", c => c.Int());
            AlterColumn("dbo.Person", "UserName", c => c.String(nullable: false, maxLength: 256, unicode: false));
            DropTable("dbo.AspNetUsers");
            RenameTable(name: "dbo.Person", newName: "AspNetUsers");

            Sql("UPDATE dbo.AspNetUsers SET Discriminator = 'Person' WHERE UserName = 'marketplace@ideianoar.com.br'");
            Sql("UPDATE dbo.AspNetUsers SET Discriminator = 'AdminUser' WHERE Id IN (SELECT UserId FROM AspNetUserRoles INNER JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id AND AspNetRoles.Name = 'Admin')");

            Sql("CREATE NONCLUSTERED INDEX [IX_Address_Id] ON [dbo].[AspNetUsers] ([Address_Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            Sql("CREATE NONCLUSTERED INDEX [IX_BankAccount_Id] ON [dbo].[AspNetUsers] ([BankAccount_Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            Sql("CREATE NONCLUSTERED INDEX [IX_LeadSalesProfile_Id] ON [dbo].[AspNetUsers] ([LeadSalesProfile_Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            Sql("CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([UserName] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            Sql("ALTER TABLE [dbo].[ApiToken]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApiToken_dbo.AspNetUsers_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[ApiToken] CHECK CONSTRAINT [FK_dbo.ApiToken_dbo.AspNetUsers_Seller_Id]");

            Sql("ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]");

            Sql("ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]");

            Sql("ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]");

            Sql("ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUsers_dbo.Address_Address_Id] FOREIGN KEY([Address_Id]) REFERENCES [dbo].[Address] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_dbo.AspNetUsers_dbo.Address_Address_Id]");

            Sql("ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUsers_dbo.BankAccount_BankAccount_Id] FOREIGN KEY([BankAccount_Id]) REFERENCES [dbo].[BankAccount] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_dbo.AspNetUsers_dbo.BankAccount_BankAccount_Id]");

            Sql("ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUsers_dbo.LeadSalesProfile_LeadSalesProfile_Id] FOREIGN KEY([LeadSalesProfile_Id]) REFERENCES [dbo].[LeadSalesProfile] ([Id])");
            Sql("ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_dbo.AspNetUsers_dbo.LeadSalesProfile_LeadSalesProfile_Id]");

            Sql("ALTER TABLE [dbo].[Cart]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Cart_dbo.AspNetUsers_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Cart] CHECK CONSTRAINT [FK_dbo.Cart_dbo.AspNetUsers_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[CartSeller]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CartSeller_dbo.AspNetUsers_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[CartSeller] CHECK CONSTRAINT [FK_dbo.CartSeller_dbo.AspNetUsers_Seller_Id]");

            Sql("ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.AspNetUsers_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.AspNetUsers_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Order_dbo.AspNetUsers_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_dbo.Order_dbo.AspNetUsers_Seller_Id]");

            Sql("ALTER TABLE [dbo].[OrderHistory]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderHistory_dbo.AspNetUsers_Person_Id] FOREIGN KEY([Person_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[OrderHistory] CHECK CONSTRAINT [FK_dbo.OrderHistory_dbo.AspNetUsers_Person_Id]");

            Sql("ALTER TABLE [dbo].[OrderReview]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderReview_dbo.AspNetUsers_Reviewer_Id] FOREIGN KEY([Reviewer_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[OrderReview] CHECK CONSTRAINT [FK_dbo.OrderReview_dbo.AspNetUsers_Reviewer_Id]");

            Sql("ALTER TABLE [dbo].[Portfolio]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Portfolio_dbo.AspNetUsers_SellerId] FOREIGN KEY([SellerId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Portfolio] CHECK CONSTRAINT [FK_dbo.Portfolio_dbo.AspNetUsers_SellerId]");

            Sql("ALTER TABLE [dbo].[ProductWish]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProductWish_dbo.AspNetUsers_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[ProductWish] CHECK CONSTRAINT [FK_dbo.ProductWish_dbo.AspNetUsers_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Quote_dbo.AspNetUsers_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_dbo.Quote_dbo.AspNetUsers_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[Quote]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Quote_dbo.AspNetUsers_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Quote] CHECK CONSTRAINT [FK_dbo.Quote_dbo.AspNetUsers_Seller_Id]");

            Sql("ALTER TABLE [dbo].[QuoteAnswer]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuoteAnswer_dbo.AspNetUsers_Buyer_Id] FOREIGN KEY([Buyer_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] CHECK CONSTRAINT [FK_dbo.QuoteAnswer_dbo.AspNetUsers_Buyer_Id]");

            Sql("ALTER TABLE [dbo].[QuoteAnswer]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuoteAnswer_dbo.AspNetUsers_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[QuoteAnswer] CHECK CONSTRAINT [FK_dbo.QuoteAnswer_dbo.AspNetUsers_Seller_Id]");

            Sql("ALTER TABLE [dbo].[RentalLock]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RentalLock_dbo.AspNetUsers_Seller_Id] FOREIGN KEY([Seller_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[RentalLock] CHECK CONSTRAINT [FK_dbo.RentalLock_dbo.AspNetUsers_Seller_Id]");

            Sql("ALTER TABLE [dbo].[ShippingAddress]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ShippingAddress_dbo.AspNetUsers_Person_Id] FOREIGN KEY([Person_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[ShippingAddress] CHECK CONSTRAINT [FK_dbo.ShippingAddress_dbo.AspNetUsers_Person_Id]");

            Sql("ALTER TABLE [dbo].[Store]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Store_dbo.AspNetUsers_SellerId] FOREIGN KEY([SellerId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Store] CHECK CONSTRAINT [FK_dbo.Store_dbo.AspNetUsers_SellerId]");

            Sql("ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Subscription_dbo.AspNetUsers_SellerId] FOREIGN KEY([SellerId]) REFERENCES [dbo].[AspNetUsers] ([Id])");
            Sql("ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_dbo.Subscription_dbo.AspNetUsers_SellerId]");
        }
    }
}
