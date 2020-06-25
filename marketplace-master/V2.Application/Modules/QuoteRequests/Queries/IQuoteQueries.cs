using System;
using System.Collections.ObjectModel;
using V2.Application.Modules.QuoteRequests.Model;

namespace V2.Application.Modules.QuoteRequests.Queries
{
    public interface IQuoteQueries
    {
        QuoteDetailsForAdminDto GetDetailsForAdmin(Guid quoteId);
        QuoteGroupDetailsForBuyer GetQuoteGroupDetailsForBuyer(Guid groupId);
        QuoteListForBuyer GetQuoteListForBuyer(string buyerId);
        ReadOnlyCollection<QuoteListForAdminDto> ListForAdmin(int pageSize, int startIndex, out int totalCount);
        ReadOnlyCollection<Guid> ListQuoteIdsInGroup(Guid groupId);
        ReadOnlyCollection<QuoteWithoutSellerListDto> ListWithoutSeller(int pageSize, int startIndex, out int totalCount);
        ReadOnlyCollection<SellerGeneralDataDto> ListSellersWhoReceivedQuoteByGroupId(Guid groupId);
        ReadOnlyCollection<SellerGeneralDataDto> ListSellerToSendQuoteTo();
    }
}
