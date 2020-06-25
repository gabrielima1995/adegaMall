using Domain.Entities.DisputeModule;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal static class DisputeMapping
    {
        public static void Apply(DbModelBuilder modelBuiler)
        {
            ApplyDisputes(modelBuiler);
            ApplyDisputeComments(modelBuiler);
        }

        private static void ApplyDisputes(DbModelBuilder builder)
        {
            var config = builder.Entity<Dispute>();

            config.HasMany(x => x.DisputeComments);

            config.Property(x => x.SellerId).HasMaxLength(128);
            config.Property(x => x.BuyerId).HasMaxLength(128);
            config.Property(x => x.OrderId).HasMaxLength(128);
        }

        private static void ApplyDisputeComments(DbModelBuilder builder)
        {
            var config = builder.Entity<DisputeComment>();

            config.Property(x => x.SenderId).HasMaxLength(128);
        }
    }
}
