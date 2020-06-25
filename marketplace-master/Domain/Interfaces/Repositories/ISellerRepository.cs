using Domain.DTO;
using Domain.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Interfaces.Repositories
{
    public interface ISellerRepository : ICrudRepository<Seller>
    {
        Seller GetByFriendlyUrl(string friendlyUrl);

        Seller GetByNickname(string nickname);

        Seller GetDetailsForAdmin(string sellerId);

        Seller GetPublicProfileInfos(string sellerId, bool requireActiveSubscription);

        Seller GetWithSubscription(string sellerId);

        Seller GetWithLeadSalesProfile(string sellerId);

        Seller GetWithCompleteLeadSalesProfile(string sellerId);

        List<Seller> ListForAdmin();

        List<SellerSelectorItem> ListForSelector();

        bool ExistsByAlternativeNameEncoded(string alternativeNameEncoded, string ignoreSellerId);

        bool ExistsByFriendlyUrl(string friendlyUrl, string ignoreSellerId);

        bool ExistsByNickname(string nickname, string ignoreSellerId);

        ReadOnlyCollection<string> ListSellerIdsPendingCheckForProfileComplete();

        ReadOnlyCollection<string> ListSellerIds();
    }
}