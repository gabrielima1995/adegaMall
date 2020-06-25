using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Services;
using Framework;
using Microsoft.AspNet.Identity.Owin;
using Presentation.Properties;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using V2.Application.Cities;

namespace Presentation.Controllers
{
    public class MigrateController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly ICityManager _cityManager;
        private readonly StrService _strService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;
        private readonly IRepository<SocialNetwork> _socialNetworkRepository;

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
            StrService strService,
            IRepository<Category> categoryRepository,
            IRepository<SubCategory> subCategoryRepository,
            IRepository<SocialNetwork> socialNetworkRepository)
        {
            _cityManager = cityManager;
            _strService = strService;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _socialNetworkRepository = socialNetworkRepository;
        }

        public ActionResult Categories()
        {
            var itemsSaved = _categoryRepository.Query().ToList();

            var items = Encoding.UTF8.GetString(Resources.categories).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (var i = 1; i < items.Length; i++)
            {
                var item = items[i];
                var values = item.Split('\t');

                var itemSaved = itemsSaved.FirstOrDefault(j => j.Name == values[0]);

                if (itemSaved == null)
                {
                    var itemToSave = new Category
                    {
                        Name = values[0]
                    };

                    itemsSaved.Add(itemToSave);

                    _categoryRepository.Insert(itemToSave);
                }
                else
                {
                    itemSaved.Name = values[0];

                    _categoryRepository.Update(itemSaved);
                }

                _categoryRepository.SaveChanges();
            }

            return Content("OK");
        }

        public ActionResult SubCategories()
        {
            var categories = _categoryRepository.Query().ToList();
            var itemsSaved = _subCategoryRepository.Query().ToList();

            var items = Encoding.UTF8.GetString(Resources.sub_categories).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 1; i < items.Length; i++)
            {
                var item = items[i];
                var values = item.Split('\t');

                var itemSaved = itemsSaved.FirstOrDefault(j => j.Name == values[0]);

                if (itemSaved == null)
                {
                    var itemToSave = new SubCategory
                    {
                        Name = values[0],
                        Category = categories.FirstOrDefault(j => j.Name == values[1])
                    };

                    itemsSaved.Add(itemToSave);

                    _subCategoryRepository.Insert(itemToSave);
                }
                else
                {
                    itemSaved.Name = values[0];
                    itemSaved.Category = categories.FirstOrDefault(j => j.Name == values[1]);

                    _subCategoryRepository.Update(itemSaved);
                }

                _subCategoryRepository.SaveChanges();
            }

            return Content("OK");
        }

        public ActionResult Cities()
        {
            _cityManager.PopulateDatabase();

            return Content("OK");
        }
    }
}