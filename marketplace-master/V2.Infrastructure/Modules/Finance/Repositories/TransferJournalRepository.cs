using Domain.Modules.Finance;
using Domain.Modules.Finance.Interfaces;
using Framework;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace V2.Infrastructure.Modules.Finance.Repositories
{
    public class TransferJournalRepository : EfRepository, ITransferJournalRepository
    {
        public TransferJournalRepository(ScaffoldContext context) : base(context)
        {
        }

        public OperationResult AddRange(IEnumerable<TransferJournalEntry> entry)
        {
            try
            {
                Context.TransferJournalEntries.AddRange(entry);
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("", ex.Message);
            }
        }

        public IEnumerable<TransferJournalEntry> ListPendingTransfersForOrder(Guid orderId)
        {
            return Context.TransferJournalEntries
                .Where(x => !x.Cancelled && !x.Completed && x.OrderId == orderId)
                .ToList();
        }

        public IEnumerable<TransferJournalEntry> ListPendingTransfersToDate(DateTime endDate)
        {
            endDate = endDate.ToEndOfDay();

            return Context.TransferJournalEntries
                .Where(x => !x.Cancelled && !x.Completed && x.DueDate <= endDate)
                .ToList();
        }

        public OperationResult Update(TransferJournalEntry entry)
        {
            try
            {
                Context.Entry(entry).State = EntityState.Modified;
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("", ex.Message);
            }
        }
    }
}
