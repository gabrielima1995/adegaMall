using Domain.Entities.PortfolioModule;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class QuoteMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<Quote>();

            config.HasOptional(x => x.LatestAnswer)
                .WithOptionalDependent()
                .Map(x => x.MapKey("LatestAnswer_Id"));

            config.Property(x => x.Category_Id);

            config.Property(x => x.SubCategory_Id);

            config.Property(x => x.Buyer_Id)
                .IsRequired();
        }
    }
}
