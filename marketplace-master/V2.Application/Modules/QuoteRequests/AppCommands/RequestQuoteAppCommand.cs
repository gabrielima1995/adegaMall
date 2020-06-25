using Application.Config;
using Domain.Addresses;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.Quotes;
using Domain.QuoteRequests.Commands;
using Domain.Services;
using Domain.Validation.Specifications;
using Framework;
using Framework.Geography;
using Infra.Crosscutting.Globalization;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Common;
using V2.Application.Helpers;
using V2.Application.Messages;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.Search.Sellers;
using V2.Application.Modules.UserAccounts;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.QuoteRequests.AppCommands
{
    public class RequestQuoteAppCommand : IWithCustomFields
    {
        public TransientBuyer Buyer { get; set; }
        public TransientAddress Address { get; set; }
        public string SellerId { get; set; }
        public string BuyerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public DateTime? Date { get; set; }
        public QuotePreference Preference { get; set; }
        public decimal? Amount { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public GeoBounds LocationBounds { get; set; }
        public GeoCoordinate LocationCoords { get; set; }
        public IEnumerable<FileUpload> Attachments { get; set; } = new List<FileUpload>();
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public Guid Id { get; set; }
        public CustomFieldEntityType EntityType => CustomFieldEntityType.Quote;
        public List<string> SearchSellerIds { get; set; } = new List<string>();
        public bool IsGeneralQuoteRequest { get; set; }
        public bool IsShortQuote { get; set; }
        public Guid? PortfolioItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class RequestQuoteAppCommandHandler
    {
        private readonly AddressService _addressService;
        private readonly BuyerService _buyerService;
        private readonly QuoteService _quoteService;
        private readonly FileUploadHelper _fileUploadHelper;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ICustomFieldAppService _customFieldAppService;
        private readonly ISellerSearchService _searchService;
        private readonly IMediator mediator;

        public RequestQuoteAppCommandHandler(
            AddressService addressService,
            BuyerService buyerService,
            FileUploadHelper fileUploadHelper,
            QuoteService quoteService,
            IBuyerRepository buyerRepository,
            ICustomFieldAppService customFieldAppService,
            ISellerSearchService searchService,
            IMediator mediator)
        {
            _addressService = addressService;
            _buyerService = buyerService;
            _fileUploadHelper = fileUploadHelper;
            _quoteService = quoteService;
            _buyerRepository = buyerRepository;
            _customFieldAppService = customFieldAppService;
            _searchService = searchService;
            this.mediator = mediator;
        }

        internal OperationResult Handle(RequestQuoteAppCommand command)
        {
            var config = MaaSConfig.Current;

            #region Validate custom fields

            var customFieldsValidation = ValidateCustomFields(command);
            if (customFieldsValidation.IsFailure) return customFieldsValidation;

            #endregion Validate custom fields

            #region Get or register buyer

            var buyerResult = GetOrRegisterBuyerId(command.Buyer);
            if (buyerResult.IsFailure) return buyerResult;

            var buyer = _buyerRepository.Get(x => x.Id == buyerResult.Result);

            if (string.IsNullOrEmpty(buyer.PhoneNumber) && !string.IsNullOrEmpty(command.Phone))
            {
                buyer.PhoneNumber = command.Phone;
                _buyerRepository.Update(buyer, true);
            }

            command.BuyerId = buyer.Id;

            #endregion Get or register buyer

            #region Check request quote specification

            var isSatisfied = new CompleteBuyerSpecification().CanRequestQuote(buyer);

            if (!isSatisfied)
                return new OperationResult("", GlobalizationHelper.GetString("BUYER_REQUEST_QUOTE_DISABLED"));

            #endregion Check request quote specification

            #region Get seller ids

            var sellerIdsResult = GetSellerIdsForNewQuote(command);
            if (sellerIdsResult.IsFailure && !config.Quote.GeneralQuote.RegisterQuoteEvenWhenNoSellers)
                return sellerIdsResult;

            #endregion Get seller ids

            #region Register address

            var addressResult = RegisterAddress(command.Address);
            if (addressResult.IsFailure) return addressResult;

            #endregion Register address

            #region Upload attachments

            var attachmentsResult = UploadAttachments(command.Attachments);
            if (attachmentsResult.IsFailure) return attachmentsResult;

            #endregion Upload attachments

            #region Register quote

            var domainCommand = new RequestQuoteCommand(
                command.Id, command.Title, command.Description, command.Phone,
                command.Date, command.Preference, command.Amount,
                command.CategoryId, command.SubCategoryId,
                buyerResult.Result, sellerIdsResult.Result,
                addressResult.Result, attachmentsResult.Result,
                command.IsGeneralQuoteRequest, command.IsShortQuote,
                command.PortfolioItemId, command.Quantity);

            var result = _quoteService.RegisterQuoteRequest(domainCommand);
            if (result.IsFailure) return result;

            #endregion Register quote

            #region Store custom fields

            if (result.OK && command.HasCustomFields())
            {
                if (result.Result.IsGroup)
                    _customFieldAppService.StoreCustomFieldsForQuoteGroup(result.Result.GroupId, command.CustomFields);
                else
                    _customFieldAppService.StoreCustomFields(command);
            }

            #endregion Store custom fields

            return OperationResult.Success;
        }

        private OperationResult<string> GetOrRegisterBuyerId(TransientBuyer buyer)
        {
            var entity = _buyerRepository.GetByEmail(buyer.Email);
            if (entity == null)
            {
                var userAndProfile = BuyerUserAndProfile.Create(UserOrPersonId.Generate(), buyer.Email, buyer.Name,
                    MaaSConfig.Current.Buyer.Moderation.Enabled);

                var registration = mediator.Send(new RegisterBuyerUserAndProfile(userAndProfile, buyer.Password)).GetAwaiter().GetResult();
                if (registration.IsFailure)
                    return registration.Errors;

                entity = userAndProfile.Profile;
            }

            return entity.Id;
        }

        private OperationResult<List<string>> GetSellerIdsForNewQuote(RequestQuoteAppCommand command)
        {
            var sellerIds = new List<string>();

            if (command.SellerId != null)
                sellerIds.Add(command.SellerId);
            else if (command.SearchSellerIds.Count >= 2)
                sellerIds.AddRange(command.SearchSellerIds);
            else
            {
                var searchResult = _searchService.Search(new Search.CommonSearchRequest
                {
                    Bounds = command.LocationBounds,
                    CategoryId = command.CategoryId,
                    SubCategoryId = command.SubCategoryId,
                    PageSize = 100,
                    Location = command.LocationCoords
                });

                if (searchResult.Items.Any())
                {
                    var resultIds = searchResult.Items.Select(item => item.Id);
                    sellerIds.AddRange(resultIds);
                }
            }

            if (!sellerIds.Any())
                return new ErrorDictionary("NoEligibleSellers", GlobalizationHelper.GetString("QUOTE_NO_SELLERS_ERROR_MESSAGE"));

            return sellerIds;
        }

        private OperationResult<Guid> RegisterAddress(TransientAddress address)
        {
            var addressRecord = _addressService.Create(() => new Address
            {
                Street = address.Street,
                Number = address.Number,
                AdditionalInfo = address.Complement,
                Neighborhood = address.Neighborhood,
                CityAndState = address.CityAndState,
                ZipCode = address.ZipCode
            });

            return addressRecord.Id;
        }

        private OperationResult<IEnumerable<string>> UploadAttachments(IEnumerable<FileUpload> filesToUpload)
        {
            if (filesToUpload == null || !filesToUpload.Any())
                return new List<string>();

            var uploadResult = _fileUploadHelper.Upload(CloudFileType.QuoteAttachment, filesToUpload);
            if (!uploadResult.OK) return uploadResult.Errors;

            return uploadResult.Result.Select(x => x.ToString()).ToList();
        }

        private OperationResult ValidateCustomFields(RequestQuoteAppCommand command)
        {
            if (command.IsShortQuote || !MaaSConfig.Current.CustomFields.Quotes.Enabled || command.CustomFields == null)
                return OperationResult.Success;

            var errors = new ErrorDictionary();

            _customFieldAppService.LoadFieldDefinitionsFor(command);

            foreach (var cfield in command.CustomFields)
                if (string.IsNullOrWhiteSpace(cfield.Value) && command.CustomFieldDefinitions[new Guid(cfield.Key)].IsMandatory)
                    errors.Add("", $"Campo {command.CustomFieldDefinitions[new Guid(cfield.Key)].Name} não preenchido");

            return errors;
        }
    }
}