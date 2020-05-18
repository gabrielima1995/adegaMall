using Domain.Modules.ProductTemplateModule;
using Framework;
using System;

namespace V2.Application.Modules.ProductTemplateModule
{
    public interface IProductTemplateRepository
    {
        OperationResult Add(ProductTemplate productTemplate);
        ProductTemplate Find(Guid id);
        OperationResult Remove(ProductTemplate productTemplate);
        OperationResult Update(ProductTemplate productTemplate);
        OperationResult OverwriteProductPropertiesFromTemplateWhereNotEditable(Guid templateId);
        OperationResult DeleteProductsAssociatedToDeletedTemplate(Guid templateId);
    }
}
