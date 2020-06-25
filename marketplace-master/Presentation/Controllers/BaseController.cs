using Domain.DTO;
using Microsoft.AspNet.Identity;
using Presentation.Globalization;
using Presentation.Models;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

namespace Presentation.Controllers
{
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Retorna o LoginProfile para o usuário + perfil logado no momento. Nunca retorna null.
        /// </summary>
        protected LoginProfile GetLoginProfile() => User.Identity.GetLoginProfile();
        /// <summary>
        /// Retorna o id real do usuário, na seguinte lógica: id_do_assistente ?? id_usuario_gerenciado.
        /// </summary>
        protected string GetActualUserId() => GetLoginProfile().ActualUserId();
        /// <summary>
        /// Retorna o id real da pessoa, na seguinte lógica: personid_do_assistente ?? personid_usuario_gerenciado.
        /// </summary>
        protected string GetActualPersonId() => GetLoginProfile().ActualPersonId();
        /// <summary>
        /// Retorna o id do usuário gerenciado. Atenção! No caso de estar logado como assistente, será o id do usuário assistido (ex. vendedor).
        /// </summary>
        protected string GetUserId() => GetLoginProfile().UserId;
        /// <summary>
        /// Retorna o personId do perfil gerenciado. Anteção! No caso de estar logado como assistente, será o personId do perfil gerenciado (ex. vendedor).
        /// </summary>
        protected string GetPersonId() => GetLoginProfile().PersonId;

        protected virtual IEnumerable<FileUpload> GetFileUploads()
        {
            var fileUploads = new List<FileUpload>();

            foreach (var fileName in Request.Files.AllKeys)
            {
                var file = Request.Files[fileName];

                if (file.ContentLength == 0)
                    continue;

                fileUploads.Add(new FileUpload(file.InputStream, file.FileName, file.ContentType));
            }

            return fileUploads;
        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home", new { lang = Request.GetCurrentLang() });
        }

        protected ViewModelBase BuildViewModel()
        {
            var profile = User.Identity.GetLoginProfile();
            return new ViewModelBase(profile);
        }

        protected ViewModelBase<T> BuildViewModel<T>(T data)
        {
            var profile = User.Identity.GetLoginProfile();
            return new ViewModelBase<T>(profile, data);
        }

        #region RedirectToActionWithLang methods

        protected RedirectToRouteResult RedirectToActionWithLang(string actionName, object routeValues, string lang = null)
        {
            var merged = new RouteValueDictionary(routeValues);
            merged.Add("lang", GetLang(lang));
            return RedirectToAction(actionName, routeValues);
        }

        protected RedirectToRouteResult RedirectToActionWithLang(string actionName, RouteValueDictionary routeValues, string lang = null)
        {
            var merged = new RouteValueDictionary(routeValues);
            merged.Add("lang", GetLang(lang));
            return RedirectToAction(actionName, routeValues);
        }

        protected RedirectToRouteResult RedirectToActionWithLang(string actionName, string controllerName, string lang = null)
            => RedirectToAction(actionName, controllerName, new { lang = GetLang(lang) });

        protected RedirectToRouteResult RedirectToActionWithLang(string actionName, string controllerName, object routeValues, string lang = null)
        {
            var merged = new RouteValueDictionary(routeValues);
            merged.Add("lang", GetLang(lang));
            return RedirectToAction(actionName, controllerName, routeValues);
        }

        protected RedirectToRouteResult RedirectToActionWithLang(string actionName, string controllerName, RouteValueDictionary routeValues, string lang = null)
        {
            var merged = new RouteValueDictionary(routeValues);
            merged.Add("lang", GetLang(lang));
            return RedirectToAction(actionName, controllerName, routeValues);
        }

        private string GetLang(string lang)
            => lang ?? Request.GetCurrentLang();

        #endregion
    }
}