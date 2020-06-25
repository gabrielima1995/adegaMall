using Domain.Entities.PortfolioModule;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IQuoteRepository : ICrudRepository<Quote>
    {
        Quote GetDetails(Guid id);

        Quote GetDetailsForAdmin(Guid id);

        List<Quote> GetListByGroup(Guid groupId);

        List<Quote> List(string buyerId, string sellerId, QuoteStatus? status);

        List<Quote> ListByGroupIds(IEnumerable<Guid> ids);
    }
}
