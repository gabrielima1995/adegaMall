using Domain;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Framework;
using Infrastructure.Data.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Infrastructure.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ScaffoldContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(ScaffoldContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            AddRoles(roleManager);
            AddMasterAdmin(context, userManager);
            AddApplicationConfig(context);
            AddSampleCategories(context);
        }

        private static void AddSampleCategories(ScaffoldContext context)
        {
            if (context.ProductCategories.FirstOrDefault() == null)
            {
                context.ProductCategories.Add(new ProductCategory("Categoria Exemplo"));
                context.SaveChanges();
            }
            if (context.Set<SubCategory>().FirstOrDefault() == null)
            {
                var category = new Category("Categoria Exemplo");
                category.AddSubcategory(new SubCategory("Subcategoria Exemplo"));

                context.Set<Category>().Add(category);
                context.SaveChanges();
            }
        }

        private static void AddApplicationConfig(ScaffoldContext context)
        {
            var appConfig = context.Set<ApplicationConfig>().FirstOrDefault();
            if (appConfig == null)
            {
                context.ApplicationConfig.Add(new ApplicationConfig());
                context.SaveChanges();
            }
        }

        private static void AddMasterAdmin(ScaffoldContext context, UserManager<ApplicationUser> userManager)
        {
            var adminEmail = Domain.Resources.AppResources.SYSADM_EMAIL;
            if (userManager.FindByEmail(adminEmail) == null)
            {
                var userAndProfile = AdminUserAndProfile.Create(UserOrPersonId.Generate(), adminEmail, "Gestão INA");
                userAndProfile.User.PasswordHash = Domain.Resources.AppResources.SYSADM_PWD_HASH;
                userAndProfile.User.SecurityStamp = Domain.Resources.AppResources.SYSADM_SEC_STAMP;

                context.People.Add(userAndProfile.Profile);
                context.Users.Add(userAndProfile.User);
                context.SaveChanges();

                userManager.AddToRole(userAndProfile.User.Id, AppRole.GestaoIna);
            }
        }

        private static void AddRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in AppRole.AllRoles)
            {
                var roleModel = new IdentityRole { Name = role };
                roleManager.Create(roleModel);
            }
        }
    }
}