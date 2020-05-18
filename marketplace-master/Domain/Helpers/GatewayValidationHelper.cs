using Application.Config;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Helpers
{
    public static class GatewayValidationHelper
    {
        public static bool IsValidationUser(LoginProfile user)
        {
            return user.Email == MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;
        }

        public static bool IsValidationUser(Seller user)
        {
            return user.Email == MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;
        }

        public static bool IsUndergoingValidation()
        {
            return MaaSConfig.Current.Payment.GatewayIsPagarme() &&
                MaaSConfig.Current.Payment.PagarMe.UndergoingValidation &&
                MaaSConfig.Current.Payment.Enabled;
        }

        public static bool ShowProfileInvisibleMessage(LoginProfile user)
        {
            if (IsUndergoingValidation())
                return (IsValidationUser(user) && !user.HasActiveSubscription);

            else return !user.HasActiveSubscription;
        }

        public static bool ShowSubscriptionMenu(LoginProfile user)
        {
            if (IsUndergoingValidation())
                return IsValidationUser(user);

            else return true;
        }

        public static bool ShowInSearchResults(Seller seller)
        {
            if (IsUndergoingValidation())
                return !IsValidationUser(seller);

            else return false;
        }
    }
}