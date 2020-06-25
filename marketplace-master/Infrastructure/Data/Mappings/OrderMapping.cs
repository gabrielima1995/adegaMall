using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using PaymentIntegrator.Core.Payments;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class OrderMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            MapOrder(modelBuilder);
            MapOrderHistory(modelBuilder);
            MapOrderProduct(modelBuilder);
            MapOrderReview(modelBuilder);
            MapOrderTransaction(modelBuilder);
        }

        private static void MapOrder(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Order>();

            var rentalInfo = modelBuilder.ComplexType<OrderRentalInfo>();

            rentalInfo.Property(x => x.FullyReturnedAt)
                .HasColumnType("datetime2");

            entity.Property(e => e.TrackingCode).HasMaxLength(20);

            entity.Property(e => e.ExtraInfo).HasMaxLength(4000);
        }

        private static void MapOrderHistory(DbModelBuilder modelBuilder)
        {
        }

        private static void MapOrderProduct(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<OrderProduct>();

            config.HasMany(i => i.RentalsInfo)
                .WithRequired(i => i.OrderProduct)
                .HasForeignKey(i => i.OrderProduct_Id);

            var rentalInfo = modelBuilder.ComplexType<OrderProductRentalInfo>();

            rentalInfo.Property(x => x.StartDate)
                .HasColumnType("datetime2");

            rentalInfo.Property(x => x.EndDate)
                .HasColumnType("datetime2");

            rentalInfo.Property(x => x.ReturnedAt)
                .HasColumnType("datetime2");

            var rentalsInfo = modelBuilder.Entity<OrderProductRentalsInfo>();
            rentalsInfo.ToTable("OrderProductRentalsInfo");
        }

        private static void MapOrderReview(DbModelBuilder modelBuilder)
        {
        }

        private static void MapOrderTransaction(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<OrderTransaction>();
            modelBuilder.Ignore<MultiPaymentItem>();
            config.Ignore(x => x.IsMultiPayment);
        }
    }
}