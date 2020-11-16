using Domain.Entities.StoreModule;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IProductRepository : ICrudRepository<Product>
    {
        bool FriendlyUrlExists(string friendlyUrl);

        Product GetByRef(string customId);

        Product GetDetailsForPublicPage(Guid id, bool requireSellerGatewayId, bool requireSellerActiveMembership);

        Product GetForCartOperations(Guid id);

        List<KeyValuePair<Guid, string>> ListCustomIds(List<Guid> productIds);

        List<Product> ListForEcommerceHome(int productCategoryId, bool requireGatewayId, bool requireActiveMembership);

        List<Product> ListProductsByExternalIds(params string[] externalIds);

        List<Product> ListTopForSeller(string sellerId);

        List<Product> ListByPriceVersionOut();

        List<Product> ListByPrice();

        List<Product> ListByScheduleTimeline(Guid? productId, string sellerId);

        List<Product> ListWithoutTemplate();
    }
}