using Application.Config;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Validation.Specifications
{
    public class CompleteBuyerSpecification
    {
        private readonly bool ignoreModeration;

        public CompleteBuyerSpecification(bool ignoreModeration = false)
        {
            this.ignoreModeration = ignoreModeration;
        }

        public bool IsSatisfiedBy(LoginProfileOrBuyer loginProfileOrBuyer) =>
            HasModeration(loginProfileOrBuyer);

        public bool CanRequestQuote(LoginProfileOrBuyer loginProfileOrBuyer) =>
            HasModeration(loginProfileOrBuyer);

        public bool HasModeration(LoginProfileOrBuyer loginProfileOrBuyer)
        {
            var required = MaaSConfig.Current.Buyer.Moderation.Enabled && !ignoreModeration;
            return required ? !loginProfileOrBuyer.RequireModeration : true;
        }

        public class LoginProfileOrBuyer
        {
            private readonly LoginProfile loginProfile;
            private readonly Buyer buyer;
            private readonly CompleteBuyerSpecificationDto specificationDto;

            public LoginProfileOrBuyer(LoginProfile loginProfile)
                => this.loginProfile = loginProfile;

            public LoginProfileOrBuyer(Buyer buyer)
                => this.buyer = buyer;

            public LoginProfileOrBuyer(CompleteBuyerSpecificationDto specificationDto)
                => this.specificationDto = specificationDto;

            public bool RequireModeration
                => loginProfile?.RequireModeration ?? buyer?.RequireModeration ?? specificationDto?.RequireModeration ?? false;

            public static implicit operator LoginProfileOrBuyer(LoginProfile loginProfile)
                => new LoginProfileOrBuyer(loginProfile);

            public static implicit operator LoginProfileOrBuyer(Buyer buyer)
                => new LoginProfileOrBuyer(buyer);

            public static implicit operator LoginProfileOrBuyer(CompleteBuyerSpecificationDto specificationDto)
                => new LoginProfileOrBuyer(specificationDto);
        }
    }

    public class CompleteBuyerSpecificationDto
    {
        public CompleteBuyerSpecificationDto(bool requireModeration)
        {
            RequireModeration = requireModeration;
        }

        public bool RequireModeration { get; }
    }
}