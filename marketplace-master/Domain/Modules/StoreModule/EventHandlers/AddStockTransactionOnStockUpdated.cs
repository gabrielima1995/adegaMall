using Domain.Core.Exceptions;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Events.StoreModule;
using Domain.Interfaces.Repositories;
using System;

namespace Domain.StoreModule.EventHandlers
{
    public class AddStockTransactionOnStockUpdated : IDomainEventHandler<ProductStockUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddStockTransactionOnStockUpdated(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Handle(ProductStockUpdatedEvent domainEvent)
        {
            //var product = _unitOfWork.ProductRepository.GetById(domainEvent.ProductId);
            var product = domainEvent.Product;
            var stockDiff = domainEvent.NewStock - domainEvent.OldStock;
            var stockTran = StockTransaction.Create(Guid.NewGuid(), product, domainEvent.Variant, stockDiff, StockTransactionType.ManualUpdate);

            var addResult = _unitOfWork.StockTransactionRepository.Add(stockTran);
            if (!addResult.OK) throw new OperationResultException(addResult);

            var saveResult = _unitOfWork.Save();
            if (!saveResult.OK) throw new OperationResultException(saveResult);
        }
    }
}
