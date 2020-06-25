using Domain.Entities.DisputeModule;
using Domain.StoreModule.Interfaces;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Infrastructure.Data.Repositories
{
    public class DisputeRepository : EfCrudRepository<Dispute>, IDisputeRepository
    {
        public DisputeRepository(ScaffoldContext context) : base(context)
        {

        }

        public override Dispute GetById(params object[] idValues)
        {
            Guid id = (Guid)idValues[0];
            return Context.Set<Dispute>().Include(x => x.DisputeComments).Where(x => x.Id == id).FirstOrDefault();
        }


        public OperationResult AddComment(Dispute dispute, DisputeComment comment)
        {
            try
            {
                if (!dispute.DisputeComments.Contains(comment))
                    dispute.DisputeComments.Add(comment);

                Context.SaveChanges();

                return OperationResult.Success;
            }
            catch (Exception e)
            {
                return new OperationResult("Error", e.Message);
            }
        }

        public OperationResult AddComment(string disputeId, DisputeComment comment)
        {
            try
            {
                var dispute = GetById(new Guid(disputeId));
                dispute.DisputeComments.Add(comment);

                return Update(dispute, true);
            }
            catch (Exception e)
            {
                return new OperationResult("Error", e.Message);
            }
        }


        public Dispute GetByOrderId(string orderId)
        {
            var query = Context.Set<Dispute>().Include(x => x.DisputeComments).Where(x => x.OrderId == orderId);

            return CacheHelper.Get(query);
        }

        public List<DisputeComment> GetDisputeComments(string disputeId)
        {
            return GetById(disputeId).DisputeComments;
        }
    }
}
