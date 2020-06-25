using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Modules.Products.StockManagement
{
    public class StockTransactionService
    {
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<StockTransaction> _stockTransactionRepository;
        private readonly IList<StockTransaction> _pendingTransactions;

        public StockTransactionService(IDomainEventDispatcher eventDispatcher, IRepository<Product> productRepository, IRepository<StockTransaction> stockTransactionRepository)
        {
            _eventDispatcher = eventDispatcher;
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _pendingTransactions = new List<StockTransaction>();
        }

        public void Add(StockTransaction transaction)
        {
            _pendingTransactions.Add(transaction);
        }

        public void CommitPending()
        {
            if (_pendingTransactions.Count == 0)
                return;

            foreach (var item in _pendingTransactions)
                _stockTransactionRepository.Insert(item);

            _stockTransactionRepository.SaveChanges();

            foreach (var item in _pendingTransactions)
            {
                _eventDispatcher.Dispatch(new ProductMovedEvent(item.Product, item.Variant, item.Amount, item.Type));
            }

            _pendingTransactions.Clear();
        }

        public void ProcessStock(Product product, ProductVariant variant)
        {
            if (product.StockByVariant)
            {
                product.CurrentStock = _stockTransactionRepository.Query()
                    .Where(x => x.Product_Id == product.Id && x.Variant_Id != null)
                    .Sum(x => x.Amount);
            }
            else
            {
                product.CurrentStock = _stockTransactionRepository.Query()
                    .Where(x => x.Product_Id == product.Id && x.Variant_Id == null)
                    .Sum(x => x.Amount);
            }

            if (variant != null)
            {
                var currentStock = _stockTransactionRepository.Query()
                    .Where(x => x.Product_Id == product.Id && x.Variant_Id == variant.Id)
                    .Sum(x => x.Amount);

                var stockVariant = product.StockVariants.FirstOrDefault(i => i.Variant_Id == variant.Id);

                if (stockVariant != null)
                {
                    stockVariant.NewStock((int)currentStock);
                }
                else
                {
                    product.StockVariants.Add(StockProductVariant.New(variant, (int)currentStock));
                }
            }

            if (product.CurrentStock <= 0 && product.DeleteWhenStockZero)
            {
                product.Deleted = true;
                product.DeletedAt = DateTime.UtcNow;
            }

            _productRepository.Update(product);
            _productRepository.SaveChanges();

            _eventDispatcher.Dispatch(new ProductCurrentStockChanged(product));
        }
    }
}