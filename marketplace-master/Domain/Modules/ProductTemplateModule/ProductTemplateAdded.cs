using Domain.Events;

namespace Domain.Modules.ProductTemplateModule
{
    public class ProductTemplateAdded : DomainEvent
    {
        public ProductTemplateAdded(ProductTemplate productTemplate)
        {
            ProductTemplate = productTemplate;
        }

        public ProductTemplate ProductTemplate { get; }
    }
}
