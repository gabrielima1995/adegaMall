using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class ProductMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Product>();

            entity.Property(b => b.ProductPrice).HasColumnName("ProductPrice");

            var rentalConfig = modelBuilder.ComplexType<RentalSettings>();

            rentalConfig.Property(x => x.AvailabilityWarning)
                .HasMaxLength(500);

            entity.Property(e => e.ProductTemplateId);
            entity.Property(e => e.Weight).HasPrecision(6,3);

            entity.Property(e => e.DefaultShipping.Amount)
                .HasColumnName("DefaultShippingAmount")
                .HasPrecision(7, 2);

            entity.Property(e => e.DefaultShipping.DeliveryDays)
                .HasColumnName("DefaultShippingDeliveryDays");

        }
    }

    internal class StoreMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Store>();

            entity.Property(e => e.DefaultShipping.Amount)
                .HasColumnName("DefaultShippingAmount")
                .HasPrecision(7, 2);

            entity.Property(e => e.DefaultShipping.DeliveryDays)
                .HasColumnName("DefaultShippingDeliveryDays");
        }
    }
}
