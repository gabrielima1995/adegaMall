using Domain.Modules.Portfolio;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class PortfolioMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            MapPortfolioItem(modelBuilder);
        }

        private void MapPortfolioItem(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<PortfolioItem>();

            config.HasMany(x => x.SubCategories)
                .WithMany()
                .Map(x => x.ToTable("PortfolioItemSubcategories"));
        }
    }
}
