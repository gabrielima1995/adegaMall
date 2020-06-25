using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class CartMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            MapCart(modelBuilder);
            MapCartItem(modelBuilder);
        }

        private void MapCart(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Cart>();

            entity.Property(e => e.ExtraInfo).HasMaxLength(4000);
        }

        private void MapCartItem(DbModelBuilder modelBuilder)
        {
            var rentalInfo = modelBuilder.ComplexType<CartItemRentalInfo>();

            rentalInfo.Property(x => x.StartDate)
                .HasColumnType("datetime2");

            rentalInfo.Property(x => x.EndDate)
                .HasColumnType("datetime2");
        }
    }
}
