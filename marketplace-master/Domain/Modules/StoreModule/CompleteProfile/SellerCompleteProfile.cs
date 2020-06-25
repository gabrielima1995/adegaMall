using Application.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Modules.StoreModule.CompleteProfile
{
    public class SellerCompleteProfile
    {
        public const string CurrentVersion = "4";

        public string Version { get; private set; } = CurrentVersion;

        [JsonProperty]
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Cadastro inicial do Seller com os CustomFields.
        /// </summary>
        public Registration Registration { get; private set; } = new Registration();

        /// <summary>
        /// Compra da assinatura, pelo seller,  da plataforma.
        /// </summary>
        public MembershipPlan MembershipPlan { get; private set; } = new MembershipPlan();

        /// <summary>
        /// Cadastro dos dados bancários.
        /// </summary>
        public GatewayProfile Gateway { get; private set; } = new GatewayProfile();

        /// <summary>
        /// Produtos cadastrados pelo seller.
        /// </summary>
        public ProductsProfile Products { get; private set; } = new ProductsProfile();

        /// <summary>
        /// Itens do portfólio criados pelo seller.
        /// </summary>
        public PortfoliotemsProfile PortfolioItems { get; private set; } = new PortfoliotemsProfile();

        /// <summary>
        /// Categorias do portfólio criadas pelo seller.
        /// </summary>
        public PortfolioCategoriesProfile PortfolioCategories { get; private set; } = new PortfolioCategoriesProfile();

        /// <summary>
        /// Moderação do seller feita pelo Admin.
        /// </summary>
        public Moderation Moderation { get; private set; } = new Moderation();

        public void EvaluateAndUpdateCompletion(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            IsComplete = true;
            Registration.IsComplete = true;
            MembershipPlan.IsComplete = true;
            Moderation.IsComplete = true;
            Products.IsComplete = true;
            PortfolioItems.IsComplete = true;
            PortfolioCategories.IsComplete = true;
            Gateway.IsComplete = true;

            RegistrationFields(data, config);
            MembershipPlanField(data, config);
            ModerationFields(data, config);
            ProductsFields(data, config);
            PortfolioItemsFields(data, config);
            PortfolioCategoriesFields(data, config);
            GatewayFields(data, config);

            if (!Registration.IsComplete ||
                !MembershipPlan.IsComplete ||
                !Products.IsComplete ||
                !PortfolioItems.IsComplete ||
                !PortfolioCategories.IsComplete ||
                !Gateway.IsComplete)
                IsComplete = false;
        }

        private void GatewayFields(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            Gateway.NeedsRegistration = config.Payment.Enabled && config.Product.Enabled;
            if (Gateway.NeedsRegistration)
            {
                if (data.SupplierGatewayId == null)
                    Gateway.IsComplete = false;
            }
        }

        private void PortfolioCategoriesFields(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            PortfolioCategories.NeedsRegistration = (config.Quote.Enabled && config.Quote.RequirePortfolioForGeneralQuote) || config.Portfolio.Enabled;
            if (PortfolioCategories.NeedsRegistration)
            {
                if (data.PortfolioCategoriesCount == 0)
                {
                    PortfolioCategories.IsComplete = false;
                    PortfolioCategories.Quantity = data.PortfolioCategoriesCount;
                }
                else
                {
                    PortfolioCategories.Quantity = data.PortfolioCategoriesCount;
                }
            }
            else
            {
                PortfolioCategories.Quantity = data.PortfolioCategoriesCount;
            }
        }

        private void PortfolioItemsFields(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            PortfolioItems.NeedsRegistration = (config.Quote.Enabled && config.Quote.RequirePortfolioForGeneralQuote) || (config.Portfolio.Enabled && config.Portfolio.PortfolioItems.IsRequired);
            if (PortfolioItems.NeedsRegistration)
            {
                if (data.PortfolioItemsCount == 0)
                {
                    PortfolioItems.IsComplete = false;
                    PortfolioItems.Quantity = data.PortfolioItemsCount;
                }
                else
                {
                    PortfolioItems.Quantity = data.PortfolioItemsCount;
                }
            }
            else
            {
                PortfolioItems.Quantity = data.PortfolioItemsCount;
            }
        }

        private void ProductsFields(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            Products.NeedsRegistration = config.Product.Enabled && !config.Product.HideStoreMenusFromSeller;
            if (Products.NeedsRegistration)
            {
                if (data.ProductsCount == 0)
                {
                    Products.IsComplete = false;
                    Products.Quantity = data.ProductsCount;
                }
                else
                {
                    Products.Quantity = data.ProductsCount;
                }
            }
            else
            {
                Products.Quantity = data.ProductsCount;
            }
        }

        private void ModerationFields(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            Moderation.NeedsModeration = config.Seller.Moderation.Enabled;
            if (Moderation.NeedsModeration)
            {
                if (!data.IsApprovedInModeration)
                {
                    Moderation.IsComplete = false;
                }
            }
        }

        private void MembershipPlanField(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            MembershipPlan.NeedsRegistration = config.Subscription.IsRequired;
            if (MembershipPlan.NeedsRegistration)
            {
                if (!data.HasActiveSubscription)
                {
                    // TODO deixar um jeito de fazer com que o a assinatura continue mesmo após o cancelamento e cancelar efetivamente no termíno dela.
                    //var endSubscriptionData = data.PlanStartDate.GetValueOrDefault().AddDays(data.PlanDays);
                    //if (endSubscriptionData <= DateTime.UtcNow)
                        MembershipPlan.IsComplete = false;
                }
            }
        }

        private void RegistrationFields(DataForSellerCompleteProfileEvaluationDto data, MaaSConfig config)
        {
            Registration.NeedsAvatar = config.Seller.Avatar.IsRequired;
            Registration.NeedsWhatsAppNumber = config.Seller.WhatsAppNumber.IsRequired;
            Registration.NeedsAddress = config.Seller.Address.IsRequired;
            Registration.NeedsDocumentNumber = config.Seller.DocumentNumber.IsRequired;
            Registration.NeedsDateOfBirth = config.Seller.DateOfBirth.IsRequired;

            if (Registration.NeedsAvatar)
            {
                if (data.Avatar != null)
                {
                    Registration.HasAvatar = true;
                }
                else
                {
                    Registration.HasAvatar = false;
                    Registration.IsComplete = false;
                }
            }

            if (Registration.NeedsWhatsAppNumber)
            {
                if (data.WhatsAppNumber != null)
                {
                    Registration.HasWhatsAppNumber = true;
                }
                else
                {
                    Registration.HasWhatsAppNumber = false;
                    Registration.IsComplete = false;
                }
            }

            if (Registration.NeedsAddress)
            {
                if (data.Address_Id != null)
                {
                    Registration.HasAddress = true;
                }
                else
                {
                    Registration.HasAddress = false;
                    Registration.IsComplete = false;
                }
            }

            if (Registration.NeedsDocumentNumber)
            {
                if (data.DocumentNumber != null)
                {
                    Registration.HasDocumentNumber = true;
                }
                else
                {
                    Registration.HasDocumentNumber = false;
                    Registration.IsComplete = false;
                }
            }

            if (Registration.NeedsDateOfBirth)
            {
                if (data.DateOfBirth != null)
                {
                    Registration.HasDateOfBirth = true;
                }
                else
                {
                    Registration.HasDateOfBirth = false;
                    Registration.IsComplete = false;
                }
            }

            Registration.PendingCustomFields.Clear();
            foreach (var customField in data.PendingCustomFields)
            {
                if (customField.Value == null)
                {
                    var errorMessageLabel = $"MANDATORY_CUSTOM_FIELD_NOT_PROVIDED {customField.Name}";
                    Registration.PendingCustomFields.Add(new PendingCustomField(customField.Id, customField.Name, errorMessageLabel));
                }
            }

            if (Registration.PendingCustomFields.Any())
                Registration.IsComplete = false;
        }
    }

    public class DataForSellerCompleteProfileEvaluationDto
    {
        public string Avatar { get; set; }
        public string WhatsAppNumber { get; set; }
        public Guid Address_Id { get; set; }
        public DateTime? PlanStartDate { get; set; }
        public int PlanDays { get; set; }
        public string DocumentNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string SupplierGatewayId { get; set; }
        public bool IsApprovedInModeration { get; set; }
        public bool HasActiveSubscription { get; set; }
        public int ProductsCount { get; set; }
        public int PortfolioItemsCount { get; set; }
        public int PortfolioCategoriesCount { get; set; }
        public SellerCompleteProfile CompleteProfile { get; protected set; } = new SellerCompleteProfile();
        public List<PendingCustomField> PendingCustomFields { get; set; } = new List<PendingCustomField>();

        public class PendingCustomField
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class Subscription
        {
            public DateTime? PlanStartDate { get; set; }
            public int PlanDays { get; set; }
        }
    }
}
