using Domain.Entities.PortfolioModule;
using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Categories
{
    public class CategoryService
    {
        private readonly IPortfolioCategoryRepository _categoryRepository;
        private readonly IPortfolioSubCategoryRepository _subCategoryRepository;

        public CategoryService(
            IPortfolioCategoryRepository categoryRepository,
            IPortfolioSubCategoryRepository subCategoryRepository)
        {
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
        }

        public Category GetCategory(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public SubCategory GetSubCategory(int id)
        {
            return _subCategoryRepository.GetById(id);
        }

        public List<Category> ListActiveCategories()
        {
            return _categoryRepository.ListActive();
        }

        public List<Category> ListCategoriesServicedBySeller(string sellerId)
        {
            return _categoryRepository.ListServicedBySeller(sellerId);
        }

        public List<SubCategory> ListActiveSubCategories(int? categoryId = null)
        {
            return _subCategoryRepository.ListActive(categoryId);
        }
    }

    [Obsolete]
    public class CategoryService_Obsolete : ServiceBase<Category>
    {
        private readonly IRepository<Category> _repository;

        public CategoryService_Obsolete(IRepository<Category> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IList<Category> SelectorList()
        {
            return _repository
                .Query()
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Name)
                .ToList();
        }
    }
}