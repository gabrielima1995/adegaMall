using Infrastructure;
using System;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ErrorReportingService _errorReportingService;

        public ErrorController(ErrorReportingService errorReportingService)
        {
            _errorReportingService = errorReportingService;
        }

        [ActionName("divzero")]
        public ActionResult DivideByZero()
        {
            int zero = 0;
            int result = 1 / zero;
            return Content(result.ToString());
        }

        public ActionResult Index(string id = null)
        {
            Exception ex = null;

            if (id != null)
            {
                ex = Session[id] as Exception;
                Session.Remove(id);
            }

            return View(ex);
        }

        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult TestSend()
        {
            try
            {
                int zero = 0;
                return Content((10 / zero).ToString());
            }
            catch (Exception ex)
            {
                _errorReportingService.ReportError(ex);
                return Content("Error email sent");
            }
        }
    }
}