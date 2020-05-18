using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Modules.ProductWishModule;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2.Application.Modules.ProductWishModule
{
    public class ProductWishAppService
    {
        private readonly IProductWishService _service;
        private readonly IProductWishRepository _repository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductWishAppService(
            IProductWishService service,
            IProductWishRepository repository,
            IBuyerRepository buyerRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _service = service;
            _repository = repository;
            _buyerRepository = buyerRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public OperationResult AddOrRemove(string buyerId, Guid productId)
        {
            var buyer = _buyerRepository.GetById(buyerId);

            return AddOrRemove(buyer, productId);
        }

        public OperationResult AddOrRemove(Buyer buyer, Guid productId)
        {
            if (buyer == null) return OperationResult.Fail("Buyer not found.");

            var productWish = _repository.Get(buyer.Id, productId);

            if (productWish != null)
            {
                var result = _service.Remove(buyer.Id, productId);

                if (!result.OK) return result;
            }
            else
            {
                var product = _productRepository.GetById(productId);

                if (product == null) return OperationResult.Fail("Product not found.");

                var result = _service.Add(buyer, product);

                if (!result.OK) return result;
            }

            return _unitOfWork.Save();
        }

        public IList<Guid> VerifyProductsWish(string buyerId, IEnumerable<Guid> productsIds)
        {
            if (productsIds == null)
                return new List<Guid>(0);

            return _repository.VerifyProductsWish(buyerId, productsIds);
        }

        public MyWishesViewModel GetWishList(string buyerId, int page, int pageSize)
        {
            var model = new MyWishesViewModel();

            var queryable = _repository.GetWishlist(buyerId);

            model.Page = page;
            model.PageSize = pageSize;
            model.Items = queryable.OrderByDescending(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            model.TotalItems = queryable.Count();

            return model;
        }
    }
}