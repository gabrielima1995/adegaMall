using Application.Config;
using Domain.Config.General;

namespace Infra.SmartSearchAdapter.Sellers
{
    internal static class SellerSearchHelper
    {
        public static bool IsSellerSearchEnabled()
        {
            if (!MaaSConfig.Current.Search.Enabled)
                return false;

            if (MaaSConfig.Current.Search.ResultType != ResultType.Seller && MaaSConfig.Current.Search.SearchType != SearchType.Seller)
                return false;

            return true;
        }
    }
}