using Domain.Core.Exceptions;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Events.StoreModule;
using Domain.Interfaces.Repositories;
using System;

namespace Domain.StoreModule.EventHandlers
{
    public class AddStockTransactionOnProductAdded : IDomainEventHandler<ProductAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddStockTransactionOnProductAdded(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Handle(ProductAddedEvent domainEvent)
        {
            //var product = _unitOfWork.ProductRepository.GetById(domainEvent.ProductId);
            var product = domainEvent.Product;

            if (!product.StockByVariant)
            {
                var stockTransaction = StockTransaction.Create(Guid.NewGuid(), product, null, domainEvent.InitialStock,
                    StockTransactionType.StockOpening);

                var addResult = _unitOfWork.StockTransactionRepository.Add(stockTransaction);
                if (!addResult.OK) throw new OperationResultException(addResult);
            }
            else
            {
                foreach (var stockVariant in product.StockVariants)
                {
                    var stockTransaction = StockTransaction.Create(Guid.NewGuid(), product, stockVariant.Variant, stockVariant.UpdatedStock,
                        StockTransactionType.StockOpening);

                    var addResult = _unitOfWork.StockTransactionRepository.Add(stockTransaction);
                    if (!addResult.OK) throw new OperationResultException(addResult);
                }
            }

            var saveResult = _unitOfWork.Save();
            if (!saveResult.OK) throw new OperationResultException(saveResult);
        }
    }
}
