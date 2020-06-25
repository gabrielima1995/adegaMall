using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class ScheduledTransferRepository : EfCrudRepository<ScheduledTransfer>, IScheduledTransferRepository
    {
        public ScheduledTransferRepository(ScaffoldContext context) : base(context)
        {
        }

        public List<ScheduledTransfer> ListByOrderId(string orderId)
        {
            return Context.ScheduledTransfers.Where(x => x.OrderId == orderId).ToList();
        }

        public IReadOnlyCollection<ScheduledTransfer> ListPendingTransfers()
        {
            return Context.ScheduledTransfers
                .Where(t =>
                    t.TransferAuthorized && !t.Transferred && !t.Canceled &&
                    t.DueDate <= DateTime.UtcNow)
                .ToList().AsReadOnly();
        }
    }
}
