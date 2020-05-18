using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Events.EcommerceHome;
using Domain.Interfaces.Repositories;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    public class EcommerceHomeService
    {
        private static readonly object _lock = new object();

        private readonly IEcommerceHomeRepository _ecommerceHomeRepository;
        private readonly IEcommerceHomeCategoryRepository _ecommerceHomeCategoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomItemRepository _customItemRepository;

        public EcommerceHomeService(
            IEcommerceHomeRepository ecommerceHomeRepository,
            IEcommerceHomeCategoryRepository ecommerceHomeCategoryRepository,
            IProductRepository productRepository,
            ICustomItemRepository customItemRepository)
        {
            _ecommerceHomeRepository = ecommerceHomeRepository;
            _ecommerceHomeCategoryRepository = ecommerceHomeCategoryRepository;
            _productRepository = productRepository;
            _customItemRepository = customItemRepository;
        }

        public EcommerceHome GetOrNew()
        {
            var record = _ecommerceHomeRepository.Get();

            if (record == null)
            {
                lock (_lock)
                {
                    record = new EcommerceHome();
                    var addResult = _ecommerceHomeRepository.Add(record, saveChanges: true);
                    if (!addResult.OK) throw new Exception(addResult.Errors.ToString());
                }
            }

            return record;
        }

        public OperationResult AddHomeCategory(EcommerceHomeCategory category)
        {
            var home = GetOrNew();

            if (home.Categories.Count >= MaaSConfig.Current.EcommerceProducts.MaxCategories)
                return new OperationResult("", GlobalizationHelper.GetString("ECOMMERCE_HOME_MAX_CATEGORIES_MET"));

            if (home.Categories.Any(x => !x.CustomItemCategoryId.HasValue && x.ProductCategoryId == category.ProductCategoryId))
                return new OperationResult("", GlobalizationHelper.GetString("ECOMMERCE_HOME_DUPLICATE_CATEGORY_ERROR"));

            if (home.Categories.Any(x => x.CustomItemCategoryId.HasValue && x.CustomItemCategoryId == category.CustomItemCategoryId))
                return new OperationResult("", GlobalizationHelper.GetString("ECOMMERCE_HOME_DUPLICATE_CATEGORY_ERROR"));

            category.AddDomainEvent(new HomeCategoryAddedEvent(category.Id));
            var addResult = _ecommerceHomeCategoryRepository.Add(category, saveChanges: true);
            if (!addResult.OK) return addResult;

            return new OperationResult();
        }

        public EcommerceHomeCategory GetHomeCategory(Guid categoryId)
        {
            var category = _ecommerceHomeRepository.GetCategory(categoryId);
            return category;
        }

        public EcommerceHomeProducts GetHomeProducts()
        {
            var home = GetOrNew();
            var result = new EcommerceHomeProducts();

            var requireGatewayId = MaaSConfig.Current.Payment.Enabled;
            var requireActiveMembership = MaaSConfig.Current.Subscription.IsRequired;

            foreach (var category in home.Categories)
            {
                if (category.CustomItemCategoryId.HasValue)
                {
                    var customItems = _customItemRepository.ListAll().Where(x => x.CategoryId == category.CustomItemCategoryId.Value);
                    List<EcommerceHomeItemViewModel> vmList = new List<EcommerceHomeItemViewModel>();
                    foreach (var cItem in customItems)
                    {
                        vmList.Add(new EcommerceHomeItemViewModel(cItem));
                    }
                    result.Add(category, vmList);
                }
                else
                {
                    var products = _productRepository.ListForEcommerceHome(category.ProductCategoryId.Value, requireGatewayId, requireActiveMembership);
                    List<EcommerceHomeItemViewModel> vmList = new List<EcommerceHomeItemViewModel>();
                    foreach (var product in products)
                    {
                        vmList.Add(new EcommerceHomeItemViewModel(product));
                    }
                    result.Add(category, vmList);
                }
            }

            return result;
        }

        public OperationResult RemoveHomeCategory(EcommerceHomeCategory category)
        {
            category.Deleted = true;
            category.DeletedAt = DateTime.UtcNow;
            category.AddDomainEvent(new HomeCategoryRemovedEvent(category.Id));
            return _ecommerceHomeCategoryRepository.Update(category, saveChanges: true);
        }

        public OperationResult UpdateHomeCategory(EcommerceHomeCategory category)
        {
            category.AddDomainEvent(new HomeCategoryUpdatedEvent(category.Id));
            var updateResult = _ecommerceHomeCategoryRepository.Update(category, saveChanges: true);
            if (!updateResult.OK) return updateResult;
            return new OperationResult();
        }
    }
}