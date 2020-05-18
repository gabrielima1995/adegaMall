using Domain.Entities;
using Domain.Entities.StoreModule;
using System;
using System.Collections.Generic;

namespace Domain.Modules.ProductTemplateModule
{
    public class ProductTemplate : BaseEntity<Guid>
    {
        private bool updatedEventAdded = false;

        private ProductTemplate(/*Ef*/)
        {
            Id = Guid.NewGuid();
        }

        public static ProductTemplate Create()
        {
            var template = new ProductTemplate();
            template.AddDomainEvent(new ProductTemplateAdded(template));
            return template;
        }

        public string Title { get; set; }
        public bool TitleIsEditable { get; set; }

        public string CoverImageUrl { get; set; }

        public ProductCategory Category { get; set; }
        public int CategoryId { get; set; }
        public bool CategoryIsEditable { get; set; }

        public string Description { get; set; }
        public bool DescriptionIsEditable { get; set; }

        public string Specification { get; set; }
        public bool SpecificationIsEditable { get; set; }

        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public bool VariantsIsEditable { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; private set; }

        public List<ProductTemplateImage> Images { get; set; } = new List<ProductTemplateImage>();
        public bool ImagesIsEditable { get; set; }

        public ProductTemplateDimensions Dimensions { get; set; } = new ProductTemplateDimensions();
        public bool DimensionsIsEditable { get; set; }

        public void MarkAsDeleted()
        {
            AddDomainEvent(new ProductTemplateDeleted(this));
        }

        public void MarkAsUpdated()
        {
            if (!updatedEventAdded || DomainEvents.Count == 0)
            {
                AddDomainEvent(new ProductTemplateUpdated(this));
                updatedEventAdded = true;
            }
        }
    }
}
