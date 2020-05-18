using Domain.DTO;
using System.Collections.Generic;

namespace V2.Application.Modules.Sellers
{
    public interface ISellerScheduledTransferQueries
    {
        List<SellerScheduledTransferViewModel> GetScheduledTransferForAdmin(string sellerId);
    }
}
