using Domain.Entities.DisputeModule;
using Domain.Interfaces.Repositories;
using Framework;
using System.Collections.Generic;

namespace Domain.StoreModule.Interfaces
{
    public interface IDisputeRepository : ICrudRepository<Dispute>
    {
        Dispute GetByOrderId(string orderId);

        List<DisputeComment> GetDisputeComments(string disputeId);

        OperationResult AddComment(Dispute dispute, DisputeComment comment);

        OperationResult AddComment(string disputeId, DisputeComment comment);
    }
}
