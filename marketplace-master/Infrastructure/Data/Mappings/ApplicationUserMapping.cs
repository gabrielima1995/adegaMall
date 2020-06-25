using Domain.Entities;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class ApplicationUserMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<ApplicationUser>();
        }
    }
}