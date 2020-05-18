using Domain.Entities;
using System;

namespace Domain.Interfaces.Repositories
{
    public interface IEcommerceHomeRepository : ICrudRepository<EcommerceHome>
    {
        EcommerceHome Get();

        EcommerceHomeCategory GetCategory(Guid ecommerceHomeCategoryId);
    }
}
