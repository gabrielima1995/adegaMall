using Domain.Events;
using Domain.Modules.ProductTemplateModule;

namespace V2.Application.Modules.ProductTemplateModule
{
    public class DeleteProductsWhenTemplateIsDeleted : IDomainEventHandler<ProductTemplateDeleted>
    {
        private readonly IProductTemplateRepository repository;

        public DeleteProductsWhenTemplateIsDeleted(IProductTemplateRepository repository)
        {
            this.repository = repository;
        }

        public void Handle(ProductTemplateDeleted domainEvent)
        {
            repository.DeleteProductsAssociatedToDeletedTemplate(domainEvent.ProductTemplate.Id);
        }
    }
}
