using System;
using System.Collections.Generic;

namespace V2.Infrastructure.Modules.Metrics
{
    public interface IMetricsQueries
    {
        MetricsQueriesResultVM RunQuery(DateTime? startDate, DateTime? endDate);

        MetricsQueriesSellerResultVM RunSellerQuery(DateTime? startDate, DateTime? endDate, Guid sellerId);

        List<MetricsQueriesCartVM> RunCartsQuery(DateTime? startDate, DateTime? endDate);
    }
}
