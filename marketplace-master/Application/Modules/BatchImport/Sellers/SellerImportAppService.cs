using Application.Config;
using Application.Modules.BatchImport.Events;
using Application.Modules.BatchImport.Models;
using Application.Sellers;
using Domain.Core;
using Domain.Entities;
using Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using V2.Application.Modules.General.CustomFields;

namespace Application.Modules.BatchImport.Sellers
{
    public class SellersImportAppService
    {
        private readonly SellerAppService sellerAppService;
        private readonly IDomainEventDispatcher eventDispatcher;
        private readonly ICustomFieldAppService customFieldAppService;

        private readonly IBatchImportService importService;
        private readonly IBatchImportQueries importQueries;

        public SellersImportAppService(
            SellerAppService sellerAppService,
            IDomainEventDispatcher eventDispatcher,
            ICustomFieldAppService customFieldAppService,
            IBatchImportService importService,
            IBatchImportQueries importQueries)
        {
            this.sellerAppService = sellerAppService;
            this.eventDispatcher = eventDispatcher;
            this.customFieldAppService = customFieldAppService;
            this.importService = importService;
            this.importQueries = importQueries;
        }

        public async Task GenerateSellerTemplateAsync(BatchImportSourceType sourceType, Stream output)
        {
            await importService.GenerateSellerTemplateAsync(sourceType, output);
        }

        public async Task<ImportResult<SellerDto>> ImportSellersAsync(BatchImportSourceType sourceType, Stream input)
        {
            var result = await importService.ImportSellersAsync(sourceType, input);

            if (!result.IsValid)
                return result;

            var emails = await importQueries.ListUserEmailsAsync();

            foreach (var dto in result.ValidRecords)
            {
                var alreadyExists = emails.Contains(dto.Email);

                if (!ProcessSeller(dto, alreadyExists))
                {
                    // TODO: add to a list of records with import problems.
                }
            }

            return result;
        }

        private bool ProcessSeller(SellerDto dto, bool alreadyExists)
        {
            Seller seller;

            if (!alreadyExists)
            {
                var registerResult = sellerAppService.RegisterAsync(dto.AsSellerRegisterViewModel()).GetAwaiter().GetResult();
                if (registerResult.IsFailure)
                    return false;

                seller = registerResult.Result.Profile;
            }

            else seller = sellerAppService.GetSellerByEmail(dto.Email);

            var profile = MapToSellerProfileViewModel(seller, dto);

            var updateProfileResult = sellerAppService.Update(profile, seller.UserId);
            if (!updateProfileResult.OK)
                return false;

            if (MaaSConfig.Current.CustomFields.Seller.Enabled)
            {
                customFieldAppService.LoadFieldDefinitionsFor(profile);

                var cfValidationResult = customFieldAppService.ValidateModel(profile);
                if (!cfValidationResult.OK)
                    return false;

                if (profile.HasCustomFields())
                    customFieldAppService.StoreCustomFields(profile);
            }

            eventDispatcher.Dispatch(new SellerImportedEvent(updateProfileResult.Result));

            return true;
        }

        private SellerProfileViewModel MapToSellerProfileViewModel(Seller seller, SellerDto dto)
        {
            var profile = new SellerProfileViewModel(seller)
            {
                DateOfBirth = dto.DateOfBirth,
                Genre = dto.Genre,
                DocumentNumber = dto.DocumentNumber,
                AdditionalInfo1 = dto.AdditionalInfo1,
                AdditionalInfo2 = dto.AdditionalInfo2,
                Address = dto.Address_Street,
                AddressNumber = dto.Address_Number,
                AddressAdditionalInfo = dto.Address_AdditionalInfo,
                AddressNeighborhood = dto.Address_Neighborhood,
                AddressZipCode = dto.Address_ZipCode,
                CityAndState = $"{dto.Address_City} {dto.Address_State}".Trim(),
                AlternativeName = dto.AlternativeName,
                AboutMe = dto.AboutMe,
                FacebookProfile = dto.FacebookProfile,
                LinkedInProfile = dto.LinkedInProfile,
                InstagramProfile = dto.InstagramProfile,
                GooglePlusProfile = dto.GooglePlusProfile
            };

            if (dto.CustomFields.Any())
                using (CultureContext.Invariant)
                    profile.CustomFields = dto.CustomFields.ToDictionary(
                        cf => cf.Id.ToString(),
                        cf => cf.Value?.ToString());


            return profile;
        }
    }
}
