using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class StockTransactionRepository : IStockTransactionRepository
    {
        private readonly ScaffoldContext _context;

        public StockTransactionRepository(ScaffoldContext context)
        {
            _context = context;
        }

        public OperationResult Add(StockTransaction entity)
        {
            try
            {
                _context.Set<StockTransaction>().Add(entity);
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return OperationResult.Fail("", ex.Message);
            }
        }

        public List<StockTransaction> ListAllByProductId(Guid productId)
        {
            return _context.Set<StockTransaction>()
                .Where(x => x.Product.Id == productId)
                .ToList();
        }
    }
}
