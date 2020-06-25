using Domain.StoreModule.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class RentalShiftMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<RentalShift>();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Start.Hours)
                .HasColumnName("StartHours");

            entity.Property(e => e.Start.Minutes)
                .HasColumnName("StartMinutes");

            entity.Property(e => e.End.Hours)
                .HasColumnName("EndHours");

            entity.Property(e => e.End.Minutes)
                .HasColumnName("EndMinutes");
        }
    }

    internal class ProductRentalShiftMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ProductRentalShift>();

            entity.HasIndex(e => new { e.ProductId, e.RentalShiftId })
                .IsUnique();
        }
    }
}
