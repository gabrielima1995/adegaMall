using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using Infrastructure;
using NewIntegrator.Stripe.PaymentIntents;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2.Application.Modules.LeadSales
{
    public class LeadSalesAppService : ILeadSalesAppService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ILeadSalesQueries _leadSalesQueries;
        private readonly ICreditsPackRepository _creditsPackRepository;
        private readonly IPaymentOperations _paymentOperations;
        private readonly IPaymentMethodOperations _paymentMethodOperations;

        public LeadSalesAppService(
            ISellerRepository sellerRepository, IQuoteRepository quoteRepository, 
            ILeadSalesQueries leadSalesQueries, ICreditsPackRepository creditsPackRepository,
            IPaymentOperations paymentOperations, IPaymentMethodOperations paymentMethodOperations)
        {
            _sellerRepository = sellerRepository;
            _quoteRepository = quoteRepository;
            _leadSalesQueries = leadSalesQueries;
            _creditsPackRepository = creditsPackRepository;
            _paymentOperations = paymentOperations;
            _paymentMethodOperations = paymentMethodOperations;
        }

        public OperationResult<CreditsBundle> AddCredits(string sellerId, int creditsAmount, int validityDays)
        {
            var seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);

            if (seller == null)
                return OperationResult<CreditsBundle>.Fail("Error", "Seller not found");
            if (creditsAmount <= 0)
                return OperationResult<CreditsBundle>.Fail("Error", "Credits amount must be greater than 0");
            if (MaaSConfig.Current.Quote.LeadSales.CreditsExpire && validityDays <= 0)
                return OperationResult<CreditsBundle>.Fail("Error", "Validity must be at least one day");

            if (seller.LeadSalesProfile == null)
            {
                CreateLeadSalesProfile(sellerId);
                seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);
            }

            var bundle = seller.LeadSalesProfile.AddCredits(creditsAmount, DateTime.UtcNow.ToEndOfDay().AddDays(validityDays));

            var result = _sellerRepository.Update(seller, true);
            if (result.OK)
                return bundle;
            else
                return OperationResult<CreditsBundle>.Fail("Error", result.Errors.First().Value);
        }

        public OperationResult RemoveCredits(string sellerId, int removeAmount)
        {
            var seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);

            if (seller.LeadSalesProfile == null)
                return OperationResult.Fail("Seller has no Lead Sales Profile");

            if (seller.LeadSalesProfile.TotalAvailableCredits < removeAmount)
                return OperationResult.Fail("Not enough credits to remove");

            seller.LeadSalesProfile.RemoveCredits(removeAmount);

            return OperationResult.Success;
        }

        public OperationResult UnlockQuote(string sellerId, string quoteId, int creditsCost)
        {
            var quote = _quoteRepository.GetById(new Guid(quoteId));

            if (quote == null)
                return OperationResult.Fail("Quote does not exist");

            var seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);
            if (seller.LeadSalesProfile == null)
            {
                CreateLeadSalesProfile(sellerId);
                seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);
            }

            if (seller.LeadSalesProfile.TotalAvailableCredits < creditsCost)
                return OperationResult.Fail("Insufficient credits");

            if (seller.LeadSalesProfile.IsQuoteUnlocked(quote.Id))
                return OperationResult.Fail("Quote already unlocked");

            RemoveCredits(sellerId, creditsCost);
            seller.LeadSalesProfile.UnlockQuote(quote);
            _sellerRepository.Update(seller, true);
            return OperationResult.Success;
        }

        public OperationResult CreateLeadSalesProfile(string sellerId)
        {
            var seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);
            if (seller.LeadSalesProfile == null)
            {
                seller.LeadSalesProfile = new LeadSalesProfile(new Guid(sellerId));

                return _sellerRepository.Update(seller, true);
            }
            return OperationResult.Fail("Lead Sales Profile already exists for this seller");
        }

        public List<Guid> ListUnlockedQuotes(string sellerId)
        {
            Seller seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);

            if (!MaaSConfig.Current.Quote.LeadSales.Enabled)
            {
                return new List<Guid>();
            }

            if (seller.LeadSalesProfile == null)
            {
                CreateLeadSalesProfile(sellerId);
                return new List<Guid>();
            }

            return seller.LeadSalesProfile.QuoteRequestsUnlocked?.Select(x => x.UnlockedQuoteId).ToList() ?? new List<Guid>();
        }

        public bool IsQuoteUnlocked(string sellerId, string quoteId)
        {
            return _leadSalesQueries.IsQuoteUnlocked(sellerId, quoteId);
        }

        public LeadSalesProfile GetLeadSalesProfile(string sellerId)
        {
            return _sellerRepository.GetWithCompleteLeadSalesProfile(sellerId).LeadSalesProfile;
        }

        public List<CreditsPack> ListCreditPacks()
        {
            return _creditsPackRepository.List();
        }

        public CreditsPack GetCreditsPack(string id)
        {
            return _creditsPackRepository.GetById(new Guid(id));
        }

        public OperationResult<CreditsPackTransaction> PurchasePack(string packId, string sellerId, PaymentOptions payment)
        {
            var pack = _creditsPackRepository.GetById(new Guid(packId));
            var seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);

            if (pack == null)
                return OperationResult<CreditsPackTransaction>.Fail("Error", "Invalid pack");

            if (seller == null)
                return OperationResult<CreditsPackTransaction>.Fail("Error", "Invalid seller");

            var paymentItem = new CreditsPackPaymentItemVM()
            {
                Amount = pack.Price,
                Price = pack.Price,
                SubTotal = pack.Price,
                Item = pack.Name,
                Quantity = 1,
            };

            var creditCardResult = _paymentMethodOperations.CreateCreditCard(seller, payment.CreditCard);

            if (!creditCardResult.OK)
                return OperationResult<CreditsPackTransaction>.Fail("Error", "Error processing Credit Card");

            var paymentOptions = new PaymentOptionsAdapter(payment);
            paymentOptions.CreditCardResult = creditCardResult.Result;

            var paymentResult = _paymentOperations.Pay(paymentItem, seller, paymentOptions);

            if (!paymentResult.OK)
                return OperationResult<CreditsPackTransaction>.Fail("Error", $"Gateway payment error: {paymentResult.Errors.First().Value}");

            var transaction = SimpleMapper.Map<ITransaction, CreditsPackTransaction>(paymentResult.Result);

            return SaveTransaction(transaction, seller, pack);
        }

        #region Stripe

        public OperationResult<CreditsPackTransaction> PurchasePack(string packId, string sellerId, StripePaymentIntent payment)
        {
            var pack = _creditsPackRepository.GetById(new Guid(packId));
            var seller = _sellerRepository.GetWithLeadSalesProfile(sellerId);

            if (pack == null)
                return OperationResult<CreditsPackTransaction>.Fail("Error", "Invalid pack");

            if (seller == null)
                return OperationResult<CreditsPackTransaction>.Fail("Error", "Invalid seller");

            var transaction = new CreditsPackTransaction
            {
                TransactionId = payment.Id,
                Status = "Paid",
                Message = payment.Status.ToString(),
                Amount = payment.Amount / 100M,
                Installments = 1
            };

            return SaveTransaction(transaction, seller, pack);
        }

        #endregion Stripe

        private OperationResult<CreditsPackTransaction> SaveTransaction(CreditsPackTransaction transaction, Seller seller, CreditsPack pack)
        {
            if (!transaction.IsPaid())
                return OperationResult<CreditsPackTransaction>.Fail("Error", "Payment not accepted by gateway");

            //Get the credit bundle created and associate it with the purchase transaction
            var addCreditsResult = AddCredits(seller.Id, pack.CreditsAmount, pack.ValidityDays.GetValueOrDefault(MaaSConfig.Current.Quote.LeadSales.CreditsDefaultDuration));
            if (addCreditsResult.OK)
                transaction.CreditsBundle = addCreditsResult.Result;

            //Store transaction in seller's Lead Sales Profile
            seller.LeadSalesProfile.CreditsPackTransactions.Add(transaction);
            _sellerRepository.Update(seller, true);

            return transaction;
        }

        public List<Guid> ListUnlockedGroups(string sellerId)
        {
            return _leadSalesQueries.ListUnlockedGroups(sellerId);
        }
    }
}