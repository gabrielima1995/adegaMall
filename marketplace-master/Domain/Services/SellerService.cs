using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Events;
using Domain.Events.SellerProfileUpdated;
using Domain.Interfaces.Repositories;
using Framework;
using System.Collections.Generic;

namespace Domain.Services
{
    public class SellerService
    {
        private readonly PersonService _personService;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISellerRepository _sellerRepository;

        public SellerService(
            PersonService personService,
            IPortfolioRepository portfolioRepository,
            IProductRepository productRepository,
            ISellerRepository sellerRepository)
        {
            _personService = personService;
            _portfolioRepository = portfolioRepository;
            _productRepository = productRepository;
            _sellerRepository = sellerRepository;
        }

        public Seller GetById(string id)
        {
            return _sellerRepository.GetById(id);
        }

        public Seller GetByNickname(string nickname)
        {
            return _sellerRepository.GetByNickname(nickname);
        }

        public Seller GetDetailsForAdmin(string sellerId)
        {
            return _sellerRepository.GetDetailsForAdmin(sellerId);
        }

        public SellerPublicProfile GetPublicProfile(string sellerId)
        {
            var requireActiveSubscription = MaaSConfig.Current.Subscription.IsRequired && !MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var model = new SellerPublicProfile
            {
                Seller = _sellerRepository.GetPublicProfileInfos(sellerId, requireActiveSubscription)
            };

            if (MaaSConfig.Current.Portfolio.Enabled)
                model.Portfolio = _portfolioRepository.GetForPublicPage(sellerId);

            if (MaaSConfig.Current.Product.Enabled)
                model.Products = _productRepository.ListTopForSeller(sellerId);

            return model;
        }

        public Seller GetWithSubscription(string sellerId)
        {
            return _sellerRepository.GetWithSubscription(sellerId);
        }

        public List<Seller> ListForAdmin()
        {
            return _sellerRepository.ListForAdmin();
        }

        public bool ExistsByEmail(string email, string ownerUserId = null)
        {
            return _personService.ExistsByEmail(email, ownerUserId);
        }

        public bool HasValidSubscription(string sellerId)
        {
            if (!MaaSConfig.Current.Subscription.IsRequired)
                return true;

            var seller = _sellerRepository.GetWithSubscription(sellerId);
            return seller != null && seller.HasSubscription && seller.IsValidSubscription;
        }

        public OperationResult<Seller> SetEnabledStatus(string sellerId, bool enabled)
        {
            var seller = _sellerRepository.GetById(sellerId);

            if (enabled != seller.Enabled)
            {
                seller.Enabled = enabled;
                seller.AddDomainEvent(new SellerUpdatedEvent(seller));

                if (seller.Enabled)
                    seller.AddDomainEvent(new SellerEnabledEvent(seller));

                var operation = _sellerRepository.SaveChanges();
                if (operation.IsFailure) return new OperationResult<Seller>(operation);
            }

            return seller;
        }

        public OperationResult<Seller> ApproveModeration(Seller seller)
        {
            if (seller.RequireModeration == true || seller.RequireModeration == null)
            {
                seller.RequireModeration = false;
                seller.AddDomainEvent(new SellerUpdatedEvent(seller));
                seller.AddDomainEvent(new SellerProfileUpdated(seller.Id));
                if (seller.Enabled)
                    seller.AddDomainEvent(new SellerModerationApprovedEvent(seller));

                var operation = _sellerRepository.SaveChanges();
                if (operation.IsFailure) return new OperationResult<Seller>(operation);
            }

            return seller;
        }

        public OperationResult SetCustomPaymentFees(string sellerId, bool overrideGlobalPaymentFees, decimal? customPaymentFeePercent, decimal? customPaymentFeeAmount, bool customIsLiable)
        {
            var seller = _sellerRepository.GetById(sellerId);
            seller.OverrideGlobalPaymentFees = overrideGlobalPaymentFees;
            seller.CustomIsLiable = customIsLiable;

            if (customPaymentFeeAmount.HasValue)
                seller.CustomPaymentFeeAmount = customPaymentFeeAmount.Value;

            if (customPaymentFeePercent.HasValue)
                seller.CustomPaymentFeePercent = customPaymentFeePercent.Value;

            return _sellerRepository.Update(seller, true);
        }
    }
}