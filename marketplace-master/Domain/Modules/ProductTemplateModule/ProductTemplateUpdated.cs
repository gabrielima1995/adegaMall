using Domain.Events;

namespace Domain.Modules.ProductTemplateModule
{
    public class ProductTemplateUpdated : DomainEvent
    {
        public ProductTemplateUpdated(ProductTemplate productTemplate)
        {
            ProductTemplate = productTemplate;
        }

        public ProductTemplate ProductTemplate { get; }
    }
}
