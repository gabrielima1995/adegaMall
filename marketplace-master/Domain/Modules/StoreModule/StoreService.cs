using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Events.StoreModule;
using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Linq;

namespace Domain.Services.StoreModule
{
    public class StoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Store GetStore(string sellerId)
        {
            return GetStoreInternal(sellerId);
        }

        public OperationResult Save(Store store)
        {
            try
            {
                var current = GetStoreInternal(store.SellerId);

                current.OnSitePickupEnabled = store.OnSitePickupEnabled;
                current.PrivateFleetEnabled = store.PrivateFleetEnabled;
                current.SedexEnabled = store.SedexEnabled;
                current.PacEnabled = store.PacEnabled;
                current.AsapLogEnabled = store.AsapLogEnabled;
                current.AsapLogUser = store.AsapLogUser;
                current.AsapLogPassword = store.AsapLogPassword;
                current.TotalExpressEnabled = store.TotalExpressEnabled;
                current.TotalExpressUser = store.TotalExpressUser;
                current.TotalExpressPassword = store.TotalExpressPassword;
                current.TotalExpressReid = store.TotalExpressReid;

                current.SingleFreightParams.Clear();
                foreach (var param in store.SingleFreightParams)
                    current.SingleFreightParams.Add(
                        new SingleFreightParameter { DeliveryRange = param.DeliveryRange, Shipping = param.Shipping, MinimumOrderAmount = param.MinimumOrderAmount});

                Sanitize(current);

                var validationResult = ValidateForSave(current);
                if (!validationResult.OK) return validationResult;

                current.AddDomainEvent(new StoreUpdatedEvent(current.SellerId));

                var updateResult = _unitOfWork.StoreRepository.Update(current, saveChanges: true);
                if (!updateResult.OK) return updateResult;

                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return new OperationResult("", ex.Message);
            }
        }

        public OperationResult ValidateForSave(Store store)
        {
            var errors = new ErrorDictionary();

            if (store == null)
                errors.Add(nameof(store), "Store cannot be null");

            if (string.IsNullOrEmpty(store.SellerId) || store.SellerId == Guid.Empty.ToString())
                errors.Add(nameof(store.SellerId), "The store must reference a seller.");

            if (store.SingleFreightParams != null)
            {
                var config = MaaSConfig.Current.Product;
                var shippingCanBeZero = !config.PrivateFleetShipping.Enabled || config.PrivateFleetShipping.AllowFreeShipping;

                foreach (var param in store.SingleFreightParams)
                {
                    if (param.DeliveryRange <= 0) errors.Add(nameof(param.DeliveryRange), "Delivery range must be greater than zero.");
                    if (param.Shipping == 0 && !shippingCanBeZero) errors.Add(nameof(param.Shipping), "Private fleet free shipping is not allowed.");
                }
            }

            return errors;
        }

        private Store GetStoreInternal(string sellerId)
        {
            var store = _unitOfWork.StoreRepository.GetForEditing(sellerId);

            if (store == null)
            {
                var seller = _unitOfWork.SellerRepository.GetById(sellerId);
                store = new Store { Seller = seller };

                var result = _unitOfWork.StoreRepository.Add(store, saveChanges: true);
                if (!result.OK) throw new Exception(result.Errors.ToSimpleString());
            }

            return store;
        }

        private void Sanitize(Store store)
        {
            foreach (var item in store.SingleFreightParams)
                if (item.Id == Guid.Empty) item.Id = Guid.NewGuid();

            var comparer = DynamicEqualityComprarer<SingleFreightParameter>.Create(x => x.DeliveryRange.ToString() + x.MinimumOrderAmount.ToString());
            store.SingleFreightParams = store.SingleFreightParams.Distinct(comparer).ToList();
        }
    }
}
