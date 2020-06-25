using Domain.DTO;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin
{
    public class BaseController : Controller
    {
        private const JsonRequestBehavior allowGet = JsonRequestBehavior.AllowGet;

        protected LoginProfile GetLoginProfile() => User.Identity.GetLoginProfile();
        protected string GetUserId() => GetLoginProfile().UserId;
        protected string GetPersonId() => GetLoginProfile().PersonId;

        protected JsonResult JsonSuccess() => Json(Envelope.Ok(), allowGet);
        protected JsonResult JsonSuccess(object data) => Json(Envelope.Ok(data), allowGet);
        protected JsonResult JsonError(string error) => Json(Envelope.Fail(error), allowGet);
        protected JsonResult JsonErrorFromModelState() => Json(Envelope.Fail(string.Join(" ", ModelState.SelectMany(entry => entry.Value.Errors.Select(error => error.ErrorMessage)))), allowGet);

        private class Envelope
        {
            private Envelope()
            {
            }

            public bool IsSuccess { get; private set; }
            public bool IsFailure { get; private set; }
            public object Data { get; private set; }
            public string Error { get; private set; }

            public static Envelope Ok() => new Envelope
            {
                IsSuccess = true
            };

            public static Envelope Ok(object data) => new Envelope
            {
                IsSuccess = true,
                Data = data
            };

            public static Envelope Fail(string error) => new Envelope
            {
                IsSuccess = false,
                IsFailure = true,
                Error = error
            };
        }
    }
}