using Application.Config;
using Domain.Entities;
using Domain.Validation.Helpers;
using FluentValidation;
using Infra.Crosscutting.Globalization;

namespace Domain.Validation
{
    public class BuyerCheckoutValidation : AbstractValidator<Buyer>
    {
        private readonly bool _integratorDisabled = !MaaSConfig.Current.Payment.Enabled;

        public BuyerCheckoutValidation()
        {
            RuleFor(x => x.CustomerGatewayId)
                .NotNull()
                .Unless(x => _integratorDisabled)
                .WithMessage(GlobalizationHelper.GetString("CHECKOUT_BUYER_GATEWAY_NOT_REGISTERED"));

            RuleFor(x => x.DocumentNumber)
                .NotEmpty()
                .WithMessage(GlobalizationHelper.GetString("CHECKOUT_BUYER_NO_DOCUMENT"));

            RuleFor(x => x.Enabled)
                .Equal(true)
                .WithMessage(GlobalizationHelper.GetString("CHECKOUT_BUYER_NOT_ENABLED"));

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(GlobalizationHelper.GetString("CHECKOUT_BUYER_NO_FULL_NAME"));

            RuleFor(x => x.IntegratorCustomerRegistered)
                .Equal(true)
                .Unless(x => _integratorDisabled)
                .WithMessage(GlobalizationHelper.GetString("CHECKOUT_BUYER_GATEWAY_NOT_REGISTERED"));

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Must(PhoneValidatorHelper.BeValidPhoneNumber)
                .WithMessage(GlobalizationHelper.GetString("CHECKOUT_BUYER_NO_PHONE"));

            if (MaaSConfig.Current.Buyer.Moderation.Enabled)
                RuleFor(x => x.RequireModeration)
                    .NotEmpty()
                    .Equal(false)
                    .WithMessage(GlobalizationHelper.GetString("CHECKOUT_BUYER_REQUIRED_MODERATION"));
        }
    }
}
