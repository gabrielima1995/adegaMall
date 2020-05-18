using PaymentIntegrator.Core.Plans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentIntegrator.Iugu.Plans
{
    public class PlanOperations : IPlanOperations
    {
        public Task<IList<IPlan>> GetListAsync(int page = 1, int resultsByPage = 50)
        {
            throw new NotImplementedException();
        }
    }
}