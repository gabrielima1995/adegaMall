using Framework;

namespace Domain.Modules.Portfolio
{
    public class PortfolioService : ServiceBase<Portfolio>
    {
        public PortfolioService(IRepository<Portfolio> repository)
            : base(repository)
        {
        }
    }
}