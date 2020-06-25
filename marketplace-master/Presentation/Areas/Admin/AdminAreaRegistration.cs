using System.Web.Mvc;

namespace Presentation.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] {
                    "Presentation.Areas.Admin.Controllers",
                    "Presentation.Areas.Admin.Controllers.Portfolio",
                    "Presentation.Areas.Admin.Controllers.Products"
                }
            );
        }
    }
}