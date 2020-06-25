using System.Linq;
using Domain.Entities.StoreModule;
using Framework;

namespace V2.Application.StartupProcesses
{
    public class SlugifyProductsBuilder
    {
        private readonly IRepository<ProductCategory> _categoryRepository;
        private readonly IRepository<ProductVariantType> _variantTypeRepository;
        private readonly IRepository<ProductVariant> _variantRepository;

        public SlugifyProductsBuilder(
            IRepository<ProductCategory> categoryRepository,
            IRepository<ProductVariantType> variantTypeRepository,
            IRepository<ProductVariant> variantRepository)
        {
            _categoryRepository = categoryRepository;
            _variantTypeRepository = variantTypeRepository;
            _variantRepository = variantRepository;
        }

        private void SlugifyCategories()
        {
            var items = _categoryRepository
                .Query()
                .Where(i => i.Slug == null)
                .ToList();

            foreach (var item in items)
            {
                item.Slugify();

                _categoryRepository.Update(item);
            }

            _categoryRepository.SaveChanges();
        }

        private void SlugifyVariantTypes()
        {
            var items = _variantTypeRepository
                .Query()
                .Where(i => i.Slug == null)
                .ToList();

            foreach (var item in items)
            {
                item.Slugify();

                _variantTypeRepository.Update(item);
            }

            _variantTypeRepository.SaveChanges();
        }

        private void SlugifyVariants()
        {
            var items = _variantRepository
                .Query()
                .Where(i => i.Slug == null)
                .ToList();

            foreach (var item in items)
            {
                item.Slugify();

                _variantRepository.Update(item);
            }

            _variantRepository.SaveChanges();
        }

        public void Run()
        {
            SlugifyCategories();
            SlugifyVariantTypes();
            SlugifyVariants();
        }
    }
}