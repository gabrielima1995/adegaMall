using Domain.DTO;
using Domain.Entities;
using Framework;
using NewIntegrator.Stripe.PaymentIntents;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.LeadSales
{
    public interface ILeadSalesAppService
    {
        OperationResult<CreditsBundle> AddCredits(string sellerId, int creditsAmount, int validityDays);

        OperationResult RemoveCredits(string sellerId, int removeAmount);

        OperationResult UnlockQuote(string sellerId, string quoteId, int creditsCost);

        OperationResult CreateLeadSalesProfile(string sellerId);

        List<Guid> ListUnlockedQuotes(string sellerId);

        List<Guid> ListUnlockedGroups(string sellerId);

        bool IsQuoteUnlocked(string sellerId, string quoteId);

        LeadSalesProfile GetLeadSalesProfile(string sellerId);

        List<CreditsPack> ListCreditPacks();

        CreditsPack GetCreditsPack(string id);

        OperationResult<CreditsPackTransaction> PurchasePack(string packId, string sellerId, StripePaymentIntent payment);

        OperationResult<CreditsPackTransaction> PurchasePack(string packId, string sellerId, PaymentOptions payment);
    }
}