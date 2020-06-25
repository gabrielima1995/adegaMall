using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal interface IDbMapping
    {
        void Map(DbModelBuilder modelBuilder);
    }
}