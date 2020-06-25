using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Framework;
using System.Collections.Generic;

namespace Domain.Services.StoreModule
{
    public class ProductCategoryService : ServiceBase<ProductCategory>
    {
        private readonly IProductCategoryRepository _repository;

        public ProductCategoryService(
            IProductCategoryRepository repository,
            IRepository<ProductCategory> repository_DEPRECATED)
            : base(repository_DEPRECATED)
        {
            _repository = repository;
        }

        public List<ProductCategory> ListActive()
        {
            return _repository.ListActive();
        }
    }
}