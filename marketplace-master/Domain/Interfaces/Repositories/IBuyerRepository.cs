using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IBuyerRepository : ICrudRepository<Buyer>
    {
        Buyer GetByEmail(string email);

        Buyer GetDetailsForAdmin(string buyerId, bool includeOrders, bool includeQuotes);

        Buyer GetProfile(string buyerId);

        Buyer GetWithAddresses(string buyerId);
    }
}