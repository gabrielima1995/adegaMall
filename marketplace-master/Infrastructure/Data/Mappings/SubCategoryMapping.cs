using Domain.Entities.PortfolioModule;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class SubCategoryMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<SubCategory>();

            config.HasMany(x => x.Portfolios)
                .WithMany(x => x.SubCategories)
                .Map(x => x.ToTable("SubCategoryPortfolio"));
        }
    }
}