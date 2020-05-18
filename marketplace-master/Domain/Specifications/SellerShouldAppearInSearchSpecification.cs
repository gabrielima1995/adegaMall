using Application.Config;
using Domain.Modules.StoreModule.CompleteProfile;
using Newtonsoft.Json;
using System;

namespace Domain.Specifications
{
    public class SellerForAppearInSearchSpecification
    {
        public bool Enabled { get; }
        public string SellerCompleteProfileJson { get; }

        public SellerForAppearInSearchSpecification(bool enabled, string sellerCompleteProfileJson)
        {
            Enabled = enabled;
            SellerCompleteProfileJson = sellerCompleteProfileJson;
        }
    }

    public class SellerShouldAppearInSearchSpecification : ISpecification<SellerForAppearInSearchSpecification>
    {
        public bool IsSatisfiedBy(SellerForAppearInSearchSpecification input)
        {
            if (!input.Enabled)
                return false;

            if (!String.IsNullOrWhiteSpace(input.SellerCompleteProfileJson))
                return JsonConvert.DeserializeObject<SellerCompleteProfile>(input.SellerCompleteProfileJson).IsComplete;
            else
                return false;
        }
    }
}