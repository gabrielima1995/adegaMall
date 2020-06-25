using Domain.Entities.StoreModule;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IStockTransactionRepository
    {
        OperationResult Add(StockTransaction entity);

        List<StockTransaction> ListAllByProductId(Guid productId);
    }
}
