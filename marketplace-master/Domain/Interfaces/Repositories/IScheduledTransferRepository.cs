using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IScheduledTransferRepository : ICrudRepository<ScheduledTransfer>
    {
        List<ScheduledTransfer> ListByOrderId(string orderId);

        IReadOnlyCollection<ScheduledTransfer> ListPendingTransfers();
    }
}
