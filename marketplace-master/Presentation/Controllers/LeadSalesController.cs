using Application.Config;
using Application.Sellers;
using Domain.DTO;
using Domain.Interfaces.Repositories;
using NewIntegrator.Stripe.PaymentIntents;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using V2.Application.Modules.LeadSales;
using V2.Application.Modules.SellerAssistantModule;

namespace Presentation.Controllers
{
    [AuthorizeSellerOrSellerAssistant]
    public class LeadSalesController : BaseController
    {
        private readonly ISellerRepository sellerRepository;
        private readonly ILeadSalesAppService leadSalesAppService;
        private readonly SellerAppService _sellerAppService;

        public LeadSalesController(
            ISellerRepository sellerRepository,
            ILeadSalesAppService leadSalesAppService,
            SellerAppService sellerAppService)
        {
            this.sellerRepository = sellerRepository;
            this.leadSalesAppService = leadSalesAppService;
            this._sellerAppService = sellerAppService;
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.LeadsEdit)]
        public ActionResult UnlockQuote(string quoteId)
        {
            if (quoteId == null)
                throw new Exception("Invalid quote id");

            var seller = sellerRepository.GetWithLeadSalesProfile(GetPersonId());

            if (seller != null && seller.LeadSalesProfile != null)
                leadSalesAppService.UnlockQuote(seller.Id, quoteId, 1);

            return RedirectToAction("Details", "Quotes", new { id = new Guid(quoteId), justUnlocked = true });
        }

        [HttpGet]
        [RequireAssistantPermissions(SellerAssistantPermissions.LeadsView)]
        public ActionResult ViewPacks()
        {
            var list = leadSalesAppService.ListCreditPacks();
            list = list.OrderBy(x => x.Price).ToList();

            return View("CreditPacks", list);
        }

        [HttpGet]
        [RequireAssistantPermissions(SellerAssistantPermissions.LeadsEdit)]
        public ActionResult PurchaseCredits(string packId)
        {
            var pack = leadSalesAppService.GetCreditsPack(packId);

            if (pack == null)
                return HttpNotFound();

            var model = new PurchaseCreditsPackVM()
            {
                Pack = pack,
                CreditsPackId = pack.Id.ToString(),
                Payment = new PaymentOptions()
                {
                    InstallmentOptions = ListInstallmentOptions(pack.Price),
                    Method = PaymentIntegrator.Core.PaymentMethods.PaymentMethod.CreditCard,
                }
            };

            return View("PurchasePack", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.LeadsEdit)]
        public ActionResult PurchaseCredits(PurchaseCreditsPackVM model)
        {
            if (ModelState.IsValid)
            {
                var sellerId = GetPersonId();

                var seller = sellerRepository.GetById(sellerId);
                if(seller.CustomerGatewayId == null)
                    _sellerAppService.RegisterGateway(seller.Id);

                var result = leadSalesAppService.PurchasePack(model.CreditsPackId, sellerId, model.Payment);

                if (result.OK)
                {
                    seller = sellerRepository.GetWithLeadSalesProfile(sellerId);
                    return View("PurchaseConfirmation", new PurchaseConfirmationVM(result.Result, seller));
                }

                ModelState.AddModelError("", result.Errors.First().Value);
            }

            return PurchaseCredits(model.CreditsPackId);
        }

        #region Stripe

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.LeadsEdit)]
        public async Task<ActionResult> PurchaseCreditsStripe(StripePurchaseCreditsPackRequest model)
        {
            if (ModelState.IsValid)
            {
                var pack = leadSalesAppService.GetCreditsPack(model.credits_pack_id);
                StripePaymentIntent intent = null;

                try
                {
                    // Cria intenção de pagamento
                    if (model.payment_method_id != null)
                    {
                        var service = new StripePaymentIntentService();
                        intent = await service.CreateManualAsync(model.payment_method_id, (long)(pack.Price * 100), MaaSConfig.Current.Payment.Stripe.Currency, pack.Name);
                    }
                    // Confirma intenção de pagamento
                    else
                    {
                        var service = new StripePaymentIntentService();
                        intent = await service.ConfirmManualAsync(model.payment_intent_id);
                    }
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        success = false,
                        error = e.Message
                    });
                }

                if (intent.Status == StripePaymentIntentStatus.RequiresAction)
                {
                    // Tell the client to handle the action
                    return Json(new
                    {
                        requires_action = true,
                        payment_intent_client_secret = intent.ClientSecret
                    });
                }

                if (intent.Status == StripePaymentIntentStatus.Succeeded)
                {
                    // The payment didn’t need any additional actions and completed!
                    // Handle post-payment fulfillment
                    var sellerId = GetPersonId();
                    var result = leadSalesAppService.PurchasePack(model.credits_pack_id, sellerId, intent);

                    if (result.OK)
                    {
                        var seller = sellerRepository.GetWithLeadSalesProfile(sellerId);

                        return Json(new
                        {
                            success = true,
                            html = RenderRazorViewToString("PurchaseConfirmation",
                                new PurchaseConfirmationVM(result.Result, seller))
                        });
                    }

                    return Json(new
                    {
                        success = false,
                        error = result.Errors.ToSimpleString()
                    });
                }

                return Json(new
                {
                    success = false,
                    error = "Invalid PaymentIntent status"
                });
            }

            return Json(new
            {
                success = false,
                error = "Invalid model"
            });
        }

        #endregion Stripe

        private List<InstallmentOption> ListInstallmentOptions(decimal orderAmount)
        {
            var config = MaaSConfig.Current;

            int maxInstallments = config.Payment.PagarMe.Installments.MaxInstallments;

            if (!config.Payment.PagarMe.CreditCard.Enabled ||
                config.Payment.PagarMe.Installments.MaxInstallments == 1 || maxInstallments == 1 ||
                orderAmount < config.Payment.PagarMe.Installments.MinOrderAmount)
            {
                return new List<InstallmentOption>(1)
                {
                    new InstallmentOption
                    {
                        InstallmentsCount = 1,
                        BaseAmount = orderAmount,
                        InterestAmount = 0M
                    }
                };
            }

            var options = new List<InstallmentOption>();

            for (var i = 1; i <= maxInstallments; i++)
            {
                var opt = new InstallmentOption
                {
                    InstallmentsCount = i,
                    BaseAmount = Math.Round(orderAmount / i, 2),
                    InterestAmount = 0M
                };

                if (i == 1 || opt.Amount >= config.Payment.PagarMe.Installments.MinInstallmentAmount)
                    options.Add(opt);
            }

            return options;
        }
    }
}