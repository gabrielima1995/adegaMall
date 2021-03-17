using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.StoreModule.Entities;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Infrastructure.Shipping
{
    public class ShippingMethodAdapterCacheDecorator : IShippingMethodAdapter
    {
        private readonly ICache cache;
        private readonly IShippingMethodAdapter wrappedAdapter;

        public ShippingMethodAdapterCacheDecorator(ICache cache, IShippingMethodAdapter wrappedAdapter)
        {
            this.cache = cache;
            this.wrappedAdapter = wrappedAdapter;
        }

        public bool AcceptsTrackingCode => wrappedAdapter.AcceptsTrackingCode;

        public DateTime? CalculateDeliveryDate(int deliveryDays)
        {
            return wrappedAdapter.CalculateDeliveryDate(deliveryDays);
        }

        public OperationResult ChangeStatus(Order order, OrderStatus newStatus, Person person = null, string additionalInfo = null)
        {
            return wrappedAdapter.ChangeStatus(order, newStatus, person, additionalInfo);
        }

        public void ChangeStatusForNewOrder(Order order, bool paymentUnderReview)
        {
            wrappedAdapter.ChangeStatusForNewOrder(order, paymentUnderReview);
        }

        public void ChangeStatusToOrderPaid(Order order, bool deliveryAllowed = true)
        {
            wrappedAdapter.ChangeStatusToOrderPaid(order, deliveryAllowed);
        }

        public ShippingMethod GetMethod()
        {
            return wrappedAdapter.GetMethod();
        }

        public OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping)
        {
            var key = GetCacheKey(nameof(GetShippingForProduct), new object[]
            {
                product.Product.Id, sellerAddress.Id, shippingAddress?.Id, chargeDoubleShipping
            });

            if (cache.Get(key) == null)
            {
                var value = wrappedAdapter.GetShippingForProduct(product, sellerAddress, shippingAddress, chargeDoubleShipping);
                if (value.Result == null || !value.Result.Any(item => item.ResultType == CorreiosResultType.CommunicationError))
                    cache.Add(key, value, TimeSpan.FromMinutes(5), CacheTimeoutType.Sliding);

                else return value;
            }

            return cache.Get(key) as OperationResult<CorreiosShippingCollection>;
        }

        public OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, Store store, Address shippingAddress)
        {
            IEnumerable<object> GetVariablesCache()
            {
                foreach (var p in products)
                {
                    yield return p.Product.Id;
                    yield return p.Quantity;
                }

                yield return store.SellerId;
                yield return shippingAddress?.Id;
            }

            var key = GetCacheKey(nameof(GetShippingForSeller), GetVariablesCache().ToArray());

            if (cache.Get(key) == null)
            {
                var value = wrappedAdapter.GetShippingForSeller(products, store, shippingAddress);
                cache.Add(key, value, TimeSpan.FromMinutes(5), CacheTimeoutType.Sliding);
            }

            return cache.Get(key) as OperationResult<CorreiosShippingCollection>;
        }

        public OperationResult<Uri> GetTrackingLink(string trackingCode)
        {
            return wrappedAdapter.GetTrackingLink(trackingCode);
        }

        public bool IsOrderCompleted(Order order)
        {
            return wrappedAdapter.IsOrderCompleted(order);
        }

        public List<OrderStatus> ListAvailableStatuses(OrderStatus currentStatus)
        {
            return wrappedAdapter.ListAvailableStatuses(currentStatus);
        }

        private string GetCacheKey(string operation, IEnumerable<object> variables)
        {
            var adapter = wrappedAdapter.GetType().Name;
            var values = variables.Select(variable => variable?.ToString());
            return $"{adapter}_{operation}_{string.Join("|", values)}";
        }

    }
}
