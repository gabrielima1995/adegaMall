using Infra.Crosscutting.Globalization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.StoreModule
{
    public enum ShippingMethod
    {
        [DescriptionLocalized("SHIPPING_METHOD_NO_SHIPPING")]
        NoShipping = 0,

        [DescriptionLocalized("SHIPPING_METHOD_ACTUAL_NO_SHIPPING")]
        ActualNoShipping = 5,

        [DescriptionLocalized("SHIPPING_METHOD_PRIVATE_FLEET")]
        PrivateFleet = 1,

        [DescriptionLocalized("SHIPPING_METHOD_ON_SITE_PICKUP")]
        OnSitePickup = 2,

        /// <summary>
        /// SEDEX
        /// </summary>
        [DescriptionLocalized("SHIPPING_METHOD_POSTAL_EXPRESS")]
        PostalExpress = 3,

        /// <summary>
        /// PAC
        /// </summary>
        [DescriptionLocalized("SHIPPING_METHOD_POSTAL_REGULAR")]
        PostalRegular = 4,

        /// <summary>
        /// AsapLog
        /// </summary>
        [DescriptionLocalized("SHIPPING_METHOD_ASAPLOG")]
        AsapLog = 10
    }

    public static class ShippingMethodExtensions
    {
        public static bool IsPhysicalDelivery(this ShippingMethod method) => method.NotIn(ShippingMethod.NoShipping, ShippingMethod.ActualNoShipping);
        public static bool IsShippingOut(this ShippingMethod method) => method.In(ShippingMethod.PrivateFleet, ShippingMethod.PostalExpress, ShippingMethod.PostalRegular, ShippingMethod.AsapLog);
    }

    public static class ShippingMethodHelpers
    {
        public static bool Has(this IEnumerable<ShippingMethod> enabledMethods, ShippingMethod methodInQuestion)
            => enabledMethods != null && enabledMethods.Contains(methodInQuestion);

        public static bool HasOnly(this IEnumerable<ShippingMethod> enabledMethods, ShippingMethod methodInQuestion)
            => enabledMethods != null && enabledMethods.Count() == 1 && enabledMethods.Single() == methodInQuestion;

        public static bool HasAll(this IEnumerable<ShippingMethod> enabledMethods, IEnumerable<ShippingMethod> methodsInQuestion)
            => enabledMethods != null && methodsInQuestion != null && methodsInQuestion.All(m => enabledMethods.Contains(m));

        public static bool HasAll(this IEnumerable<ShippingMethod> enabledMethods, params ShippingMethod[] methodsInQuestion)
            => HasAll(enabledMethods, methodsInQuestion.ToList());

        public static bool HasAny(this IEnumerable<ShippingMethod> enabledMethods, IEnumerable<ShippingMethod> methodsInQuestion)
            => enabledMethods != null && methodsInQuestion != null && methodsInQuestion.Any(m => enabledMethods.Contains(m));

        public static bool HasAny(this IEnumerable<ShippingMethod> enabledMethods, params ShippingMethod[] methodsInQuestion)
            => HasAny(enabledMethods, methodsInQuestion.ToList());

        public static bool HasAnyBesides(this IEnumerable<ShippingMethod> enabledMethods, ShippingMethod methodInQuestion)
            => enabledMethods != null && enabledMethods.Any(m => m != methodInQuestion);

        public static bool HasAnyBesides(this IEnumerable<ShippingMethod> enabledMethods, IEnumerable<ShippingMethod> methodsInQuestion)
            => enabledMethods != null && enabledMethods.Except(methodsInQuestion).Any();

        public static bool HasAnyBesides(this IEnumerable<ShippingMethod> enabledMethods, params ShippingMethod[] methodsInQuestion)
            => HasAnyBesides(enabledMethods, methodsInQuestion.ToList());

        public static bool HasNot(this IEnumerable<ShippingMethod> enabledMethods, ShippingMethod methodInQuestion)
            => enabledMethods == null || !enabledMethods.Contains(methodInQuestion);

        public static bool HasNone(this IEnumerable<ShippingMethod> enabledMethods, IEnumerable<ShippingMethod> methodsInQuestion)
            => enabledMethods == null || methodsInQuestion == null || methodsInQuestion.All(m => !enabledMethods.Contains(m));

        public static bool HasNone(this IEnumerable<ShippingMethod> enabledMethods, params ShippingMethod[] methodsInQuestion)
            => HasNone(enabledMethods, methodsInQuestion.ToList());
    }
}