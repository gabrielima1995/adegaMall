using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class ApiTokenMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<ApiToken>();

            config.HasOptional(x => x.Seller)
                .WithMany()
                .HasForeignKey(x => x.Seller_Id);
        }
    }
}