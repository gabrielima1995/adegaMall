using Application.Config;
using Application.StaticPages;
using DevTrends.MvcDonutCaching;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [DonutOutputCache(CacheProfile = "General")]
    public class HowItWorksController : Controller
    {
        private readonly IStaticPagesAppService _staticPagesAppService;

        public HowItWorksController(
            IStaticPagesAppService staticPagesAppService)
        {
            _staticPagesAppService = staticPagesAppService;
        }

        public ActionResult Index()
        {
            /*
             * A página de Como Funciona foi desabilitada.
             * Os links para ela foram removidos.
             * Se for acessada por link direto, agora ela redireciona conforme regras abaixo.
             */

            var config = MaaSConfig.Current;

            string url =
                config.Enterprise.TermsOfUseUrl ??
                Url.Action("Index", "Home");

            return Redirect(url);
        }

        // Código mantido caso volte a fazer sentido mostrar a partir de um resource.
        //public ActionResult Index()
        //{
        //    ViewBag.Content = _staticPagesAppService.HowItWorks(Request.GetCurrentLang());
        //    return View();
        //}
    }
}