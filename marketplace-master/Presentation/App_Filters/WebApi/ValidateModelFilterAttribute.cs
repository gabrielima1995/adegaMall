using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Infra.Crosscutting.WebHelpers.WebApi.Filters
{
    public class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }
    }
}