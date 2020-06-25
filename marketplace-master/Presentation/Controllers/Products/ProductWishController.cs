using Presentation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.ProductWishModule;

namespace Presentation.Controllers.Products
{
    [AuthorizeBuyer]
    [RoutePrefix("ProductWish")]
    public class ProductWishController : BaseController
    {
        private readonly ProductWishAppService _appService;

        public ProductWishController(
            ProductWishAppService appService)
        {
            _appService = appService;
        }

        [HttpPost]
        [Route("AddOrRemove/{productId}")]
        public ActionResult AddOrRemove(Guid productId)
        {
            var buyerId = GetPersonId();
            var result = _appService.AddOrRemove(buyerId, productId);

            if (result.OK)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, errors = result.Errors.Select(i => i.Value) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Verify")]
        public ActionResult Verify(IEnumerable<Guid> productIds)
        {
            var buyerId = GetPersonId();
            var products = _appService.VerifyProductsWish(buyerId, productIds);
            return Json(new { products }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("AddItemAndProceedToLogin")]
        public ActionResult AddItemAndProceedToLogin(Guid productId)
        {
            WishlistHelper.StoreItemForLogin(productId);

            return Redirect(Url.ActionLocalized("Login", "Account", new { returnUrl = Request.UrlReferrer?.PathAndQuery }));
        }
    }
}