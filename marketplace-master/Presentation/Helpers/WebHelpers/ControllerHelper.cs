using Framework;
using Infra.Crosscutting.Globalization;
using System.Collections.Generic;
using System.Net;

namespace System.Web.Mvc
{
    public static class ControllerHelper
    {
        public static ActionResult NoContent(this ControllerBase controller)
        {
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public static ActionResult NotFound(this ControllerBase controller)
        {
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public static ActionResult InternalServerError(this ControllerBase controller)
        {
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        private static void SetMessageData(ControllerBase controller, string key, string message, bool tempData)
        {
            var dict = tempData
                ? controller.TempData
                : (IDictionary<string, object>)controller.ViewData;

            if (dict.ContainsKey(key))
                dict.Remove(key);

            dict.Add(key, message);
        }

        public static void SetSuccessMessage(this ControllerBase controller, bool tempData = false)
        {
            SetSuccessMessage(controller, GlobalizationHelper.GetString("CHANGES_SAVED_MESSAGE"), tempData);
        }

        public static void SetSuccessMessage(this ControllerBase controller, string message, bool tempData = false)
        {
            SetMessageData(controller, MessageKeys.SUCCESS, GlobalizationHelper.GetString(message), tempData);
        }

        public static void SetWarningMessage(this ControllerBase controller, string message, bool tempData = false)
        {
            SetMessageData(controller, MessageKeys.WARNING, GlobalizationHelper.GetString(message), tempData);
        }

        public static void SetErrorMessage(this ControllerBase controller, string message, bool tempData = false)
        {
            SetMessageData(controller, MessageKeys.ERROR, GlobalizationHelper.GetString(message), tempData);
        }

        public static void AddErrors(this Controller controller, OperationResult operationResult, bool ignoreKey = false)
        {
            controller.ModelState.AddErrors(operationResult, ignoreKey);
        }
    }
}