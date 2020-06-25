using Domain;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class ApplicationConfigMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<ApplicationConfig>();

            config.Property(x => x.BRL_USD)
                .HasPrecision(8, 5);
        }
    }
}
