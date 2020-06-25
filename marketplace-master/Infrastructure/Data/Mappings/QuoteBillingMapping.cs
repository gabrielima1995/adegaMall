using Domain.Modules.QuoteBillingModule.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class QuoteBillingMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<QuoteBilling>();

            config.Property(x => x.SellerId)
                .HasMaxLength(128);

            config.Property(x => x.BuyerId)
                .HasMaxLength(128);

            config.Property(x => x.Amount)
                .HasPrecision(18, 2);
        }
    }
}