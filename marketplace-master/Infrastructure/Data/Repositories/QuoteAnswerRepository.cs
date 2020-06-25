using Domain.Entities.PortfolioModule;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class QuoteAnswerRepository : EfCrudRepository<QuoteAnswer>, IQuoteAnswerRepository
    {
        public QuoteAnswerRepository(ScaffoldContext context) : base(context)
        {
        }
    }
}
