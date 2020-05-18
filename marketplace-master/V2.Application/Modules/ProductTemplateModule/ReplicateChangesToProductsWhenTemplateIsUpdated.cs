using Domain.Events;
using Domain.Modules.ProductTemplateModule;

namespace V2.Application.Modules.ProductTemplateModule
{
    public class ReplicateChangesToProductsWhenTemplateIsUpdated : IDomainEventHandler<ProductTemplateUpdated>
    {
        private readonly IProductTemplateRepository repository;

        public ReplicateChangesToProductsWhenTemplateIsUpdated(IProductTemplateRepository repository)
        {
            this.repository = repository;
        }

        public void Handle(ProductTemplateUpdated domainEvent)
        {
            repository.OverwriteProductPropertiesFromTemplateWhereNotEditable(domainEvent.ProductTemplate.Id);
        }
    }
}
