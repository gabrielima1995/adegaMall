using Microsoft.EntityFrameworkCore;

namespace NewContext.Infrastructure.Data.Mappers
{
    internal interface IMapper
    {
        void Map(ModelBuilder modelBuilder);
    }
}
