using Domain.DTO;
using Domain.Entities.StoreModule;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Helpers
{
    public static class CartHelper
    {
        private const string CookieName_CartAddItem = "__CART_ADDITEM";
        private const string CookieName_CartAddCustomField = "__CART_ADDCUSTOMFIELDS";

        public static IEnumerable<string> GetCartErrors(ModelStateDictionary modelState, bool excludeProductErrors = false)
        {
            if (modelState == null || modelState.IsValid)
                return new string[0];

            Guid parsedGuid;

            var errors = modelState
                .Where(x => string.IsNullOrEmpty(x.Key) || !Guid.TryParse(x.Key, out parsedGuid))
                .SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage))
                .ToList();

            return errors;
        }

        public static IEnumerable<string> GetItemErrors(CartItem item, ModelStateDictionary modelState)
        {
            if (item == null || modelState == null || modelState.IsValid)
                return new string[0];

            var errors = new List<string>();
            var productId = item.Product?.Id.ToString();
            var sellerId = item.Product?.Store?.SellerId;

            foreach (var key in modelState.Keys)
            {
                if (key == productId || key == sellerId)
                {
                    errors.AddRange(modelState[key].Errors.Select(x => x.ErrorMessage));
                }
            }

            return errors;
        }

        public static void StoreItemForLogin(Guid id, int quantity, DateTime? rentalStartDateTime, DateTime? rentalEndDateTime, int multiplier, int? variant, Guid? productAvailabilityId, Dictionary<string,string> customFields = null)
        {
            var value = Base64.Encode($"{id},{quantity},{rentalStartDateTime},{rentalEndDateTime},{multiplier},{variant},{productAvailabilityId}");

            HttpContext.Current.Session[CookieName_CartAddItem] = value;
            HttpContext.Current.Session[CookieName_CartAddCustomField] = customFields;
        }

        public static bool RetrieveItemAfterLogin(out Guid id, out int quantity, out DateTime? rentalStartDateTime, out DateTime? rentalEndDateTime, out int multiplier, out int? variant, out Guid? productAvailabilityId, out CartCustomFieldsViewModel cartCustomFields)
        {
            id = Guid.Empty;
            quantity = 0;
            rentalStartDateTime = rentalEndDateTime = null;
            multiplier = 0;
            variant = null;
            productAvailabilityId = null;
            cartCustomFields = new CartCustomFieldsViewModel();

            var sessionCart = HttpContext.Current.Session[CookieName_CartAddItem];
            var customFields = HttpContext.Current.Session[CookieName_CartAddCustomField];
            if (sessionCart != null)
            {
                try
                {
                    var raw = sessionCart.ToString();
                    HttpContext.Current.Session.Remove(CookieName_CartAddItem);
                    HttpContext.Current.Session.Remove(CookieName_CartAddCustomField);
                    var parts = Base64.Decode(raw).Split(',');

                    id = Guid.Parse(parts[0]);
                    quantity = int.Parse(parts[1]);

                    if (parts.Length > 2)
                    {
                        if (!string.IsNullOrWhiteSpace(parts[2]))
                            rentalStartDateTime = DateTime.Parse(parts[2]);

                        if (!string.IsNullOrWhiteSpace(parts[3]))
                            rentalEndDateTime = DateTime.Parse(parts[3]);
                    }

                    if (parts.Length > 4)
                    {
                        multiplier = int.Parse(parts[4]);
                    }

                    if (parts.Length > 5)
                    {
                        if (!string.IsNullOrWhiteSpace(parts[5]))
                            variant = int.Parse(parts[5]);
                    }

                    if (parts.Length > 6)
                    {
                        if (!string.IsNullOrWhiteSpace(parts[6]))
                            productAvailabilityId = Guid.Parse(parts[6]);
                    }

                    if(customFields != null)
                    {
                        cartCustomFields.CustomFields = customFields as Dictionary<string, string>;
                    }

                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}