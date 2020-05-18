using Domain.Entities;
using Domain.Modules.StoreModule.CompleteProfile;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain.DTO
{
    public class LoginProfile
    {
        public string UserId { get; private set; }
        public string PersonId { get; private set; }
        public LoginProfilePersonType CurrentPersonType { get; set; }

        public string Email { get; private set; }
        public string Avatar { get; private set; }
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public string WhatsAppNumber { get; private set; }
        public LoginMode LoginMode { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool? RequireModeration { get; private set; }
        public string PreferredTimeZoneId { get; private set; }

        public string CustomerGatewayId { get; private set; }
        public bool IntegratorCustomerRegistered => !string.IsNullOrEmpty(CustomerGatewayId);

        public bool HasCategories { get; private set; }
        public bool HasPortfolio { get; private set; }
        public string SupplierGatewayId { get; private set; }
        public bool IntegratorSupplierRegistered => !string.IsNullOrEmpty(SupplierGatewayId);
        public SubscriptionStatus SubscriptionStatus { get; private set; }
        public bool HasActiveSubscription => SubscriptionStatus.IsActiveSubscription();
        public SellerCompleteProfile SellerCompleteProfile { get; set; } = new SellerCompleteProfile();

        public string AssistantUserId { get; private set; }
        public string AssistantPersonId { get; private set; }
        public ReadOnlyCollection<string> AssistantPermissions { get; set; } = new List<string>().AsReadOnly();
    }

    public enum LoginProfilePersonType
    {
        Unset = 0,
        MasterAdmin, Admin,
        Buyer, Seller, SellerAssistant
    }

    public static class LoginProfileExtensions
    {
        /// <summary>
        /// Retorna true se o usuário não é um SellerAssistant, ou se é um SellerAssistant com as permissões indicadas.
        /// </summary>
        public static bool IsNotAssistantOrHasPermissions(this LoginProfile profile, params string[] permissions)
        {
            return !profile.IsSellerAssistant() || permissions.Any(p => profile.AssistantPermissions.Contains(p));
        }

        public static string ActualUserId(this LoginProfile profile) => profile.AssistantUserId ?? profile.UserId;
        public static string ActualPersonId(this LoginProfile profile) => profile.AssistantPersonId ?? profile.PersonId;

        public static bool IsAdmin(this LoginProfile profile) => profile.CurrentPersonType == LoginProfilePersonType.Admin;
        public static bool IsAdminOrGestao(this LoginProfile profile) => profile.CurrentPersonType.In(LoginProfilePersonType.Admin, LoginProfilePersonType.MasterAdmin);
        public static bool IsBuyer(this LoginProfile profile) => profile.CurrentPersonType == LoginProfilePersonType.Buyer;
        public static bool IsBuyerOrSeller(this LoginProfile profile) => profile.CurrentPersonType.In(LoginProfilePersonType.Buyer, LoginProfilePersonType.Seller);
        public static bool IsBuyerOrSellerOrSellerAssistant(this LoginProfile profile) => profile.CurrentPersonType.In(LoginProfilePersonType.Buyer, LoginProfilePersonType.Seller, LoginProfilePersonType.SellerAssistant);
        public static bool IsGestao(this LoginProfile profile) => profile.CurrentPersonType == LoginProfilePersonType.MasterAdmin;
        public static bool IsSeller(this LoginProfile profile) => profile.CurrentPersonType == LoginProfilePersonType.Seller;
        public static bool IsSellerAssistant(this LoginProfile profile) => profile.CurrentPersonType == LoginProfilePersonType.SellerAssistant;
        public static bool IsSellerOrSellerAssistant(this LoginProfile profile) => profile.CurrentPersonType.In(LoginProfilePersonType.Seller, LoginProfilePersonType.SellerAssistant);
    }
}