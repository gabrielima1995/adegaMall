using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.ProductTemplateModule;
using Framework;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using V2.Application.Cities;
using V2.Application.Modules.ProductTemplateModule;

namespace Presentation.Controllers
{
    public class MigrateController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly ICityManager _cityManager;
        private readonly IProductRepository _productRepository;
        private readonly IProductTemplateRepository _productTemplateRepository;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? OwinContextExtensions.Get<ApplicationSignInManager>(HttpContext.GetOwinContext());
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public MigrateController(
            ICityManager cityManager,
            IProductRepository productRepository,
            IProductTemplateRepository productTemplateRepository)
        {
            _cityManager = cityManager;
            _productRepository = productRepository;
            _productTemplateRepository = productTemplateRepository;
        }

        //public ActionResult Categories()
        //{
        //    var itemsSaved = _categoryRepository.Query().ToList();

        //    var items = Encoding.UTF8.GetString(Resources.categories).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        //    for (var i = 1; i < items.Length; i++)
        //    {
        //        var item = items[i];
        //        var values = item.Split('\t');

        //        var itemSaved = itemsSaved.FirstOrDefault(j => j.Name == values[0]);

        //        if (itemSaved == null)
        //        {
        //            var itemToSave = new Category
        //            {
        //                Name = values[0]
        //            };

        //            itemsSaved.Add(itemToSave);

        //            _categoryRepository.Insert(itemToSave);
        //        }
        //        else
        //        {
        //            itemSaved.Name = values[0];

        //            _categoryRepository.Update(itemSaved);
        //        }

        //        _categoryRepository.SaveChanges();
        //    }

        //    return Content("OK");
        //}

        //public ActionResult SubCategories()
        //{
        //    var categories = _categoryRepository.Query().ToList();
        //    var itemsSaved = _subCategoryRepository.Query().ToList();

        //    var items = Encoding.UTF8.GetString(Resources.sub_categories).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        //    for (int i = 1; i < items.Length; i++)
        //    {
        //        var item = items[i];
        //        var values = item.Split('\t');

        //        var itemSaved = itemsSaved.FirstOrDefault(j => j.Name == values[0]);

        //        if (itemSaved == null)
        //        {
        //            var itemToSave = new SubCategory
        //            {
        //                Name = values[0],
        //                Category = categories.FirstOrDefault(j => j.Name == values[1])
        //            };

        //            itemsSaved.Add(itemToSave);

        //            _subCategoryRepository.Insert(itemToSave);
        //        }
        //        else
        //        {
        //            itemSaved.Name = values[0];
        //            itemSaved.Category = categories.FirstOrDefault(j => j.Name == values[1]);

        //            _subCategoryRepository.Update(itemSaved);
        //        }

        //        _subCategoryRepository.SaveChanges();
        //    }

        //    return Content("OK");
        //}

        public ActionResult Cities()
        {
            _cityManager.PopulateDatabase();

            return Content("OK");
        }

        public ActionResult ProductsTemplates()
        {
            var products = _productRepository.ListWithoutTemplate();

            var total = products.Count;
            var totalMigrated = 0;

            foreach (var product in products)
            {
                var template = ProductTemplate.Create();
                template.Title = !string.IsNullOrWhiteSpace(product.Title)
                    ? product.Title
                    : ".";
                template.TitleIsEditable = false;

                template.CoverImageUrl = product.CoverImageUrl;

                template.CategoryId = product.Category_Id.Value;
                template.CategoryIsEditable = false;

                template.Description = !string.IsNullOrWhiteSpace(product.Specifications)
                    ? product.Description
                    : ".";
                template.DescriptionIsEditable = false;

                template.Specification = !string.IsNullOrWhiteSpace(product.Specifications)
                    ? product.Specifications
                    : ".";
                template.SpecificationIsEditable = false;

                template.Variants.AddRange(product.Variants.SelectList(v => new ProductVariant { Id = v.Id }));
                template.VariantsIsEditable = false;

                template.IsEnabled = true;

                var position = 1;
                template.Images.AddRange(product.Images.SelectList(i => new ProductTemplateImage(i.Url.ToString(), position++)));
                template.ImagesIsEditable = false;

                template.Dimensions.Length = product.Length;
                template.Dimensions.Width = product.Width;
                template.Dimensions.Height = product.Height;
                template.Dimensions.Weight = product.Weight;
                template.DimensionsIsEditable = false;

                var result = _productTemplateRepository.Add(template);

                result.OnSuccess(() =>
                {
                    product.ProductTemplateId = template.Id;
                    result = _productRepository.Update(product, true);

                    result.OnSuccess(() => totalMigrated++);
                });
            }

            return Content($"Products: {total} - Migrated: {totalMigrated}");
        }
    }
}