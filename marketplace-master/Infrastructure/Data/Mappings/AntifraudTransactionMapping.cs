using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class AntifraudTransactionMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<AntifraudTransaction>();

            config.Property(x => x.RequestContent)
                .HasColumnType("nvarchar(max)");

            config.Property(x => x.Content)
                .HasColumnType("nvarchar(max)");

            config.Property(x => x.Message)
                .HasMaxLength(1000);

            config.Property(x => x.Status)
                .HasMaxLength(150);

            config.Property(x => x.TransactionId)
                .HasMaxLength(50);
        }
    }
}
