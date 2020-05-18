using Domain.Events;

namespace Domain.Modules.ProductTemplateModule
{
    public class ProductTemplateDeleted : DomainEvent
    {
        public ProductTemplateDeleted(ProductTemplate productTemplate)
        {
            ProductTemplate = productTemplate;
        }

        public ProductTemplate ProductTemplate { get; }
    }
}
