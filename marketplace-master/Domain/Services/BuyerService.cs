using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Utils;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Threading;

namespace Domain.Services
{
    public class BuyerService
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly ICloudStorage _cloudStorage;
        private readonly IGeocodingService _geocodingService;
        private readonly IPaymentService _paymentService;
        private readonly PersonService _personService;

        private MaaSConfig Config => MaaSConfig.Current;

        public BuyerService(
            IBuyerRepository buyerRepository,
            ICloudStorage cloudStorage,
            IGeocodingService geocodingService,
            IPaymentService paymentService,
            PersonService personService)
        {
            _buyerRepository = buyerRepository;
            _cloudStorage = cloudStorage;
            _geocodingService = geocodingService;
            _paymentService = paymentService;
            _personService = personService;
        }

        public bool ExistsByEmail(string email, string ownerUserId = null) => _personService.ExistsByEmail(email, ownerUserId);
        public Buyer GetById(string buyerId) => _buyerRepository.GetById(buyerId);
        public Buyer GetByEmail(string email) => _buyerRepository.GetByEmail(email);

        public Buyer GetDetailsForAdmin(string buyerId)
        {
            var includeOrders = MaaSConfig.Current.Product.Enabled;
            var includeQuotes = MaaSConfig.Current.Quote.Enabled;
            return _buyerRepository.GetDetailsForAdmin(buyerId, includeOrders, includeQuotes);
        }

        public Buyer GetProfile(string buyerId) => _buyerRepository.GetProfile(buyerId);

        public Buyer GetWithAddresses(string buyerId) => _buyerRepository.GetWithAddresses(buyerId);

        public OperationResult RegisterInPaymentServiceIfNotRegistered(Buyer buyer, Address address)
        {
            return RegisterInPaymentServiceIfNotRegisteredInternal(buyer, address, true);
        }

        public OperationResult<Buyer> SetEnabledStatus(string buyerId, bool enabled)
        {
            var buyer = _buyerRepository.GetById(buyerId);

            if (enabled != buyer.Enabled)
            {
                buyer.Enabled = enabled;
                buyer.AddDomainEvent(new BuyerUpdatedEvent(buyer));

                if (buyer.Enabled)
                    buyer.AddDomainEvent(new BuyerEnabledEvent(buyer));

                var operation = _buyerRepository.SaveChanges();
                if (operation.IsFailure) return new OperationResult<Buyer>(operation);
            }

            return buyer;
        }

        public OperationResult<Buyer> ApproveModeration(Buyer buyer)
        {
            if (buyer.RequireModeration == true)
            {
                buyer.RequireModeration = false;
                buyer.AddDomainEvent(new BuyerUpdatedEvent(buyer));
                buyer.AddDomainEvent(new BuyerModerationApprovedEvent(buyer));

                var operation = _buyerRepository.SaveChanges();
                if (operation.IsFailure) return new OperationResult<Buyer>(operation);
            }

            return buyer;
        }

        public OperationResult Update(Buyer buyer, FileUpload profileImage)
        {
            StringHelper.Sanitize(buyer);

            var validation = ValidateForUpdate(buyer);
            if (!validation.OK) return validation;

            if (buyer.Address != null)
            {
                var geoResult = _geocodingService.GetCoordinates(buyer.Address);
                if (!geoResult.OK) return new ErrorDictionary(nameof(buyer.Address), GlobalizationHelper.GetString("ADDRESS_CANNOT_BE_GEOCODED"));
                buyer.Address.SetLocation(geoResult.Result.Lat, geoResult.Result.Lng);
            }

            if (profileImage != null && profileImage.ContentLength > 0)
            {
                try
                {
                    var uri = _cloudStorage.UploadFile(CloudFileType.UserImage.ToString(), profileImage.FileName, profileImage.ContentType, profileImage.InputStream);
                    buyer.Avatar = uri.ToString();
                }
                catch (Exception ex)
                {
                    return OperationResult.Fail("", ex.Message);
                }
            }

            var paymentResgistrationResult = RegisterInPaymentServiceIfNotRegisteredInternal(buyer, buyer.Address, false);
            if (!paymentResgistrationResult.OK) return paymentResgistrationResult;

            buyer.AddDomainEvent(new BuyerUpdatedEvent(buyer));

            var updateResult = _buyerRepository.Update(buyer, saveChanges: true);
            if (!updateResult.OK) return updateResult;

            return OperationResult.Success;
        }

        public OperationResult ValidateForUpdate(Buyer buyer)
        {
            var errors = new ErrorDictionary();

            if (buyer == null)
                errors.Add(nameof(buyer), GlobalizationHelper.FormatString("REQUIRED_ERROR", nameof(buyer)));
            else
            {
                if (ExistsByEmail(buyer.Email, buyer.UserId))
                    errors.Add(nameof(buyer.Email), GlobalizationHelper.GetString("EMAIL_ALREADY_EXISTS_ERROR_MESSAGE"));
            }

            if (!DocumentNumberValidator.IsValidForBuyer(buyer.DocumentNumber))
                return new OperationResult<Buyer>(
                    key: nameof(buyer.DocumentNumber),
                    errors: Thread.CurrentThread.FormatStringLocalized("NOT_VALID_FIELD_ERROR", Thread.CurrentThread.GetStringLocalized("DOCUMENT_NUMBER_LABEL")));

            return errors;
        }

        private OperationResult RegisterInPaymentServiceIfNotRegisteredInternal(Buyer buyer, Address address, bool saveChanges)
        {
            if (Config.Product.Enabled && Config.Payment.Enabled)
            {
                if (!buyer.IntegratorCustomerRegistered)
                {
                    var validationResult = _paymentService.RegisterAsCustomer(buyer, address);
                    if (!validationResult.OK) return validationResult;

                    var updateResult = _buyerRepository.Update(buyer, saveChanges);
                    if (!updateResult.OK) return updateResult;
                }
            }

            return OperationResult.Success;
        }

        public OperationResult AugumentProfile(BuyerUserAndProfile userAndProfile, FileUpload avatar)
        {
            var profile = userAndProfile.Profile;
            var registerGateway = false;

            if (profile.Address != null)
            {
                var geoResult = _geocodingService.GetCoordinates(profile.Address);
                if (geoResult.OK)
                {
                    profile.Address.SetLocation(geoResult.Result.Lat, geoResult.Result.Lng);
                    registerGateway = true;
                }
            }

            if (avatar != null && avatar.ContentLength > 0)
            {
                var uri = _cloudStorage.UploadFile(CloudFileType.UserImage.ToString(), avatar.FileName, avatar.ContentType, avatar.InputStream);
                profile.Avatar = uri.ToString();
            }

            if (registerGateway)
                RegisterInPaymentServiceIfNotRegisteredInternal(profile, profile.Address, false);

            var saveResult = _buyerRepository.SaveChanges();
            if (saveResult.IsFailure)
                return saveResult.Errors;

            return OperationResult.Success;
        }
    }
}