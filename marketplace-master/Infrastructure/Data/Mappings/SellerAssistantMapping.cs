using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class SellerAssistantMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<SellerAssistant>();

            entity.Property(e => e.SellerId)
                .HasColumnName("AssistantToSellerId")
                .HasMaxLength(128);
        }
    }
}