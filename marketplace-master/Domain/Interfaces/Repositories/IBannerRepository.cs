using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IBannerRepository : ICrudRepository<Banner>
    {
        List<Banner> ListActive();

        List<Banner> ListForAdmin();
    }
}
