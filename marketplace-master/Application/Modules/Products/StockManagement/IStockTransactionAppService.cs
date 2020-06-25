using System;

namespace Application.Modules.Products.StockManagement
{
    public interface IStockTransactionAppService
    {
        void EnqueueStockProcessing(Guid productId);
    }
}