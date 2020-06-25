using Domain;
using Domain.Core;
using Domain.Entities;
using Domain.Entities.DisputeModule;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Entities.SubscriptionModule;
using Domain.Modules.Customization.Entities;
using Domain.Modules.Finance;
using Domain.Modules.ProductTemplateModule;
using Domain.Modules.ProductWishModule;
using Domain.Modules.QuoteBillingModule.Entities;
using Domain.QuoteRequests;
using Domain.StoreModule.Entities;
using Domain.UserConfig;
using Infra.Crosscutting.Globalization;
using Infrastructure.Data.Mappings;
using Infrastructure.Data.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Context
{
    public class ScaffoldContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IDomainEventDispatcher dispatcher;

        public DbSet<AntifraudTransaction> AntifraudTransactions { get; set; }
        public DbSet<ApplicationConfig> ApplicationConfig { get; set; }
        public DbSet<UserConfigurationRaw> UserConfig { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<EcommerceHome> EcomerceHome { get; set; }
        public DbSet<EcommerceHomeCategory> EcommerceHomeCategories { get; set; }
        public DbSet<GatewayRegistrationLog> GatewayRegistrationLogs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlatformCustomization> PlatformCustomization { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductRentalShift> ProductRentalShifts { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteBilling> QuoteBillings { get; set; }
        public DbSet<QuoteAttachment> QuoteAttachments { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }
        public DbSet<Str> Strings { get; set; }
        public DbSet<TransferJournalEntry> TransferJournalEntries { get; set; }
        public DbSet<Webhook> Webhooks { get; set; }
        public DbSet<RequestReceivedApiLog> RequestReceivedApiLogs { get; set; }
        public DbSet<WebhookCalledApiLog> WebhookCalledApiLogs { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<RentalLock> RentalLocks { get; set; }
        public DbSet<RentalShift> RentalShifts { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<ScheduledTransfer> ScheduledTransfers { get; set; }
        public DbSet<DisputeComment> DisputeComments { get; set; }
        public DbSet<CustomItem> CustomItems { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<CustomFieldValue> CustomFieldValues { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<CreditsPack> CreditsPacks { get; set; }
        public DbSet<ProductWish> ProductWishs { get; set; }
        public DbSet<ProductTemplate> ProductTemplates { get; set; }
        public DbSet<MarketplacePushSubscription> PushSubscriptions { get; set; }

        public bool IsDisposed { get; private set; }

        static ScaffoldContext()
        {
            System.Data.Entity.Infrastructure.Interception.DbInterception.Add(new CommandInterceptor());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ScaffoldContext, Configuration>());
        }

        public ScaffoldContext() : base("ScaffoldContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public ScaffoldContext(IDomainEventDispatcher eventDispatcher) : this()
        {
            dispatcher = eventDispatcher;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ApplyMappings(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetPersonModifiedAt();
            var affectedRows = base.SaveChanges();

            if (dispatcher != null)
                dispatcher.DispatchDomainEvents(this);

            return affectedRows;
        }

        private void ApplyMappings(DbModelBuilder modelBuilder)
        {
            var interfaceType = typeof(IDbMapping);

            var concreteTypes = interfaceType.Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && interfaceType.IsAssignableFrom(x));

            foreach (var t in concreteTypes)
            {
                var mapper = (IDbMapping)Activator.CreateInstance(t);
                mapper.Map(modelBuilder);
            }
        }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }

        private void SetPersonModifiedAt()
        {
            var entries = ChangeTracker.Entries<Person>()
                .Where(p => p.State.In(EntityState.Added, EntityState.Modified))
                .ToList();
            
            if (entries.Count > 0)
                entries.ForEach(p => p.Entity.ModifiedAt = DateTime.UtcNow);
        }
    }
}