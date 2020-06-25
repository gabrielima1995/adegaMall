using Domain.Entities.StoreModule;
using Framework;
using System;

namespace Application.Modules.Products.Store
{
    public interface IStoreAppService
    {
        ProductEditorViewModel PrepareNewProduct(Guid? templateId);

        ProductEditorViewModel PrepareProduct(ProductEditorViewModel model);

        ProductEditorViewModel GetProduct(Guid productId, string sellerId);

        OperationResult<Product> SaveProduct(ProductEditorViewModel model);

        OperationResult<Domain.Entities.StoreModule.Store> SaveStore(StoreViewModel model);

        void DeleteProduct(Guid productId, string sellerId);
    }
}