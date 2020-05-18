using Application.Config;
using Application.Modules.BatchImport.Models;
using Application.Modules.BatchImport.Sellers;
using BatchImport;
using BatchImport.Fields;
using Infra.BatchImportAdapter.Fields;
using Infra.BatchImportAdapter.Internals;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Infra.BatchImportAdapter.Sellers
{
    internal class SellerModelAndParser : DataModelAndParser<SellerDto>
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        private List<IField> fields;
        private MappingCache mappingCache;
        private readonly IEnumerable<CustomFieldDto> customFields;

        public SellerModelAndParser(IEnumerable<CustomFieldDto> customFields)
        {
            this.customFields = customFields;
        }

        public override ReadOnlyCollection<IField> ListFields()
        {
            if (fields == null)
            {
                #region Configure all fields

                fields = new List<IField>
                {
                    new StringField(
                        "FullName", L("NAME_LABEL"), S("NAME"), isMandatory: true, maxLength: 100, minLength: 5),

                        new EmailField(
                        "Email", L("EMAIL_LABEL"), S("EMAIL"), isMandatory: true),

                        new YesNoField(
                        "Enabled", L("ENABLED"), S("ENABLED"))
                };

                if (config.Seller.AlternativeName.Enabled)
                    fields.Add(new StringField(
                        "AlternativeName", L("ALTERNATIVE_LABEL"), S("ALTERNATIVE_NAME"), isMandatory: config.Seller.AlternativeName.Mandatory, maxLength: 100));

                if (config.Seller.DocumentNumber.Enabled)
                    fields.Add(new CpfOrCnpjField(
                        "DocumentNumber", L("DOCUMENT_NUMBER_LABEL"), S("DOCUMENT_NUMBER"), isMandatory: config.Seller.DocumentNumber.Mandatory));

                if (config.Seller.DateOfBirth.Enabled)
                    fields.Add(new DateField(
                        "DateOfBirth", L("DATE_OF_BIRTH_LABEL"), S("DATE_OF_BIRTH"), isMandatory: config.Seller.DateOfBirth.Mandatory));

                if (config.Seller.Genre.Enabled)
                    fields.Add(new GenreField(
                        "Genre", L("GENRE_LABEL"), S("GENRE"), isMandatory: config.Seller.Genre.Mandatory));

                if (config.Seller.AdditionalInfo1.Enabled)
                    fields.Add(new StringField(
                        "AdditionalInfo1", L("ADDITIONAL_INFO_1_LABEL"), "", isMandatory: config.Seller.AdditionalInfo1.Mandatory, maxLength: 1000));

                if (config.Seller.AdditionalInfo2.Enabled)
                    fields.Add(new StringField(
                        "AdditionalInfo2", L("ADDITIONAL_INFO_2_LABEL"), "", isMandatory: config.Seller.AdditionalInfo2.Mandatory, maxLength: 1000));

                if (config.Seller.AboutMe.Enabled)
                    fields.Add(new StringField(
                        "AboutMe", L("ABOUT_ME_LABEL"), S("ABOUT_ME"), isMandatory: config.Seller.AboutMe.Mandatory, maxLength: 4000));

                if (config.Seller.Phone.Enabled)
                    fields.Add(new BrazilianPhoneNumberField(
                        "PhoneNumber", L("PHONE_LABEL"), S("PHONE_NUMBER"), isMandatory: config.Seller.Phone.Mandatory));

                if (config.Seller.Address.Enabled)
                {
                    var mandateAddress = config.Seller.Address.Mandatory;

                    fields.Add(new StringField(
                        "Address_Street", L("ADDRESS_LABEL"), S("ADDRESS_STREET"), isMandatory: mandateAddress, maxLength: 100));

                    fields.Add(new StringField(
                        "Address_Number", L("ADDRESS_NUMBER_LABEL"), S("ADDRESS_NUMBER"), isMandatory: mandateAddress, maxLength: 10));

                    fields.Add(new StringField(
                        "Address_AdditionalInfo", L("ADDRESS_ADDITIONAL_INFO_LABEL"), S("ADDRESS_ADDITIONAL_INFO"), maxLength: 30));

                    fields.Add(new StringField(
                        "Address_Neighborhood", L("NEIGHBORHOOD_LABEL"), S("ADDRESS_NEIGHBORHOOD"), isMandatory: mandateAddress, maxLength: 30));

                    fields.Add(new StringField(
                        "Address_City", L("CITY"), S("ADDRESS_CITY"), isMandatory: mandateAddress, maxLength: 30));

                    fields.Add(new StringField(
                        "Address_State", L("STATE"), S("ADDRESS_STATE"), isMandatory: mandateAddress, maxLength: 30));

                    fields.Add(new BrazilianZipCodeField(
                        "Address_ZipCode", L("ZIPCODE_LABEL"), S("ADDRESS_ZIP_CODE"), isMandatory: mandateAddress));
                }

                if (false /*desativado por enquanto - o cadastro de gateway foi modificado (campos e processo mudaram) e ainda não foi readaptado para a importação*/ && config.Payment.Enabled)
                {
                    fields.Add(new StringField(
                        "AccountableName", L("ACCOUNTABLE_NAME_LABEL"), S("ACCOUNTABLE_NAME"), isMandatory: true));

                    fields.Add(new CpfOrCnpjField(
                        "AccountableDocumentNumber", L("ACCOUNTABLE_DOCUMENT_NUMBER_LABEL"), S("ACCOUNTABLE_DOCUMENT_NUMBER"), isMandatory: true));

                    fields.Add(new BrazilianBankCodeField(
                        "Bank_Bank", L("BANK_LABEL"), S("BANK_BANK"), isMandatory: true));

                    fields.Add(new BrazilianBankAccountTypeField(
                        "Bank_AccountType", L("BANK_ACCOUNT_TYPE_LABEL"), S("BANK_ACCOUNT_TYPE"), isMandatory: true));

                    fields.Add(new BrazilianBankAccountBranchField(
                        "Bank_Branch", L("BANK_ACCOUNT_BRANCH_LABEL"), S("BANK_ACCOUNT_BRANCH"), isMandatory: true));

                    fields.Add(new BrazilianBankAccountNumberField(
                        "Bank_Number", L("BANK_ACCOUNT_NUMBER_LABEL"), S("BANK_ACCOUNT_NUMBER"), isMandatory: true));
                }

                if (config.Seller.FacebookProfile.Enabled)
                    fields.Add(new UrlField(
                        "FacebookProfile", L("FACEBOOK_PROFILE_URL"), S("FACEBOOK_PROFILE"), isMandatory: config.Seller.FacebookProfile.Mandatory));

                if (config.Seller.LinkedInProfile.Enabled)
                    fields.Add(new UrlField(
                        "LinkedInProfile", L("LINKEDIN_PROFILE_URL"), S("LINKEDIN_PROFILE"), isMandatory: config.Seller.LinkedInProfile.Mandatory));

                if (config.Seller.InstagramProfile.Enabled)
                    fields.Add(new UrlField(
                        "InstagramProfile", L("INSTAGRAM_PROFILE_URL"), S("INSTAGRAM_PROFILE"), isMandatory: config.Seller.InstagramProfile.Mandatory));

                if (customFields.Any())
                    foreach (var f in CustomFieldsAdapter.Convert(customFields))
                        fields.Add(new CustomFieldWrapper(f));

                #endregion
            }

            return fields.AsReadOnly();
        }

        public override SellerDto ParseRecord(IEnumerable<IField> fields, IDataRecord record)
        {
            if (mappingCache == null)
                mappingCache = MappingCache.Build<SellerDto>(fields.ToList(), customFields.ToList());

            var seller = new SellerDto();

            foreach (var item in mappingCache.PropertyMappings)
            {
                var value = record[item.FieldIndex];
                var parsed = item.Field.Parse(value);
                item.Property.SetValue(seller, parsed, null);
            }

            if (mappingCache.CustomFieldMappings.Any())
            {
                var customFields = new CustomFieldValueDto[mappingCache.CustomFieldMappings.Count];

                foreach (var item in mappingCache.CustomFieldMappings)
                {
                    var value = record[item.FieldIndex];
                    var parsed = item.Field.Parse(value);
                    customFields[item.CustomFieldIndex] = new CustomFieldValueDto(item.CustomField.Id, parsed);
                }

                seller.CustomFields = customFields;
            }

            return seller;
        }

        #region Helpers

        /// <summary>
        /// Returns the localized text for the specified label.
        /// </summary>
        private string L(string label) => Localization.Get(label);

        /// <summary>
        /// Returns the localized sample data for the specified label.
        /// </summary>
        private string S(string label) => Localization.Get("BATCH_IMPORT_SAMPLE_SELLER_" + label);

        #endregion
    }
}
