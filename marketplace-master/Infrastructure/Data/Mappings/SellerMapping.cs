using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class SellerMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<Seller>();

            config.Property(x => x.AccountableName)
                .HasMaxLength(100);

            config.Property(x => x.AccountableDocumentNumber)
                .HasMaxLength(20);

            config.Property(x => x.WhatsAppNumber)
                .HasMaxLength(21);

            config.Property(x => x.CustomPaymentFeePercent).HasPrecision(5, 2);
            config.Property(x => x.CustomPaymentFeeAmount).HasPrecision(7, 2);

            config.Property(x => x.SellerCompleteProfileVersion)
                .HasMaxLength(2);
        }
    }
}