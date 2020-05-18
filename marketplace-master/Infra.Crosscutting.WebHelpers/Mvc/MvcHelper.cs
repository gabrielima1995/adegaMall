using Framework;

namespace System.Web.Mvc
{
    public static class MvcHelper
    {
        public static void AddErrors(this ModelStateDictionary modelState, OperationResult operationResult, bool ignoreKey = false)
        {
            foreach (var error in operationResult.Errors)
            {
                modelState.AddModelError(ignoreKey ? "" : error.Key, error.Value);
            }
        }
    }
}