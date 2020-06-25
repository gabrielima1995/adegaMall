using Application.Modules.BatchImport.Models;
using Application.Sellers;
using Domain.Entities;
using PaymentIntegrator.Core.Common;
using System;
using System.Collections.Generic;

namespace Application.Modules.BatchImport.Sellers
{
    public class SellerDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Genre? Genre { get; set; }
        public bool Enabled { get; set; }
        public string AdditionalInfo1 { get; set; }
        public string AdditionalInfo2 { get; set; }
        public string AboutMe { get; set; }
        public string PhoneNumber { get; set; }
        public string Address_Street { get; set; }
        public string Address_Number { get; set; }
        public string Address_AdditionalInfo { get; set; }
        public string Address_Neighborhood { get; set; }
        public string Address_City { get; set; }
        public string Address_State { get; set; }
        public string Address_ZipCode { get; set; }
        public string AccountableName { get; set; }
        public string AccountableDocumentNumber { get; set; }
        public Bank Bank_Bank { get; set; }
        public AccountType Bank_AccountType { get; set; }
        public string Bank_Branch { get; set; }
        public string Bank_Number { get; private set; }
        public string AlternativeName { get; set; }
        public string FacebookProfile { get; set; }
        public string LinkedInProfile { get; set; }
        public string InstagramProfile { get; set; }
        public string GooglePlusProfile { get; set; }
        public bool? GatewayTransferEnabled { get; set; }
        public bool? OverrideGlobalPaymentFees { get; set; }
        public bool? CustomIsLiable { get; set; }
        public decimal? CustomPaymentFeePercent { get; set; }
        public decimal? CustomPaymentFeeAmount { get; set; }
        public IList<CustomFieldValueDto> CustomFields { get; set; } = new List<CustomFieldValueDto>(0);

        public SellerRegisterViewModel AsSellerRegisterViewModel()
        {
            var password = Guid.NewGuid().ToString();

            return new SellerRegisterViewModel
            {
                Name = FullName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Password = password,
                ConfirmPassword = password
            };
        }
    }
}
