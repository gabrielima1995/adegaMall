using Domain.Modules.ProductTemplateModule;
using System.Data.Entity;

namespace Infrastructure.Data.Mappings
{
    internal class ProductTemplateMapping : IDbMapping
    {
        public void Map(DbModelBuilder modelBuilder)
        {
            MapProductTemplate(modelBuilder);
            MapProductTemplateDimensions(modelBuilder);
            MapProductTemplateImage(modelBuilder);
        }

        private void MapProductTemplate(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ProductTemplate>();

            entity.Property(e => e.Title).HasMaxLength(100);

            entity.Property(e => e.CoverImageUrl).HasMaxLength(1000);

            entity.HasRequired(e => e.Category).WithMany();

            entity.Property(e => e.Description);

            entity.Property(e => e.Specification);

            entity.HasMany(e => e.Variants).WithMany();

            entity.Property(e => e.IsEnabled);

            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.Images).WithRequired().HasForeignKey(e => e.ProductTemplateId);

            entity.HasMany(e => e.Variants).WithMany();
        }

        private void MapProductTemplateDimensions(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.ComplexType<ProductTemplateDimensions>();

            entity.Property(e => e.Height);

            entity.Property(e => e.Length);

            entity.Property(e => e.Weight);

            entity.Property(e => e.Width);
        }

        private void MapProductTemplateImage(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ProductTemplateImage>();

            // https://stackoverflow.com/a/32983252/484108
            // Composition and aggregation in EF
            entity.HasKey(e => new { e.ProductTemplateId, e.Id });

            entity.Property(e => e.Url).HasMaxLength(1000);

            entity.Property(e => e.Position);
        }
    }
}
