using System;

namespace Application.Modules.Products.StockManagement
{
    public class StockTransactionAppService : IStockTransactionAppService
    {
        public void EnqueueStockProcessing(Guid productId)
        {
            //JobHelper.Enqueue<StockTransactionService>(service => service.ProcessStock(productId), "stockprocessing");
        }
    }
}
