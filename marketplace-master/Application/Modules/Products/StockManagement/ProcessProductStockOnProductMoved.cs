using Domain.Events;
using Domain.Modules.Products.StockManagement;

namespace Application.Modules.Products.StockManagement
{
    public class ProcessProductStockOnProductMoved : IDomainEventHandler<ProductMovedEvent>
    {
        private readonly StockTransactionService _stockTransactionAppService;

        public ProcessProductStockOnProductMoved(StockTransactionService stockTransactionAppService)
        {
            _stockTransactionAppService = stockTransactionAppService;
        }

        public void Handle(ProductMovedEvent message)
        {
            //_stockTransactionAppService.EnqueueStockProcessing(message.Product.Id);

            _stockTransactionAppService.ProcessStock(message.Product, message.Variant);
        }
    }
}
