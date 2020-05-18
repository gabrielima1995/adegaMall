using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Moip.Helpers;
using System;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip.Customers
{
    public class MoipCustomer
    {
        public static object Create(ICustomer customer, IAddress address)
        {
            PhoneHelper.ExtractNumbers(customer.PhoneNumber, out string areaCode, out string number);

            string ownId = customer.Name.Replace(" ", "").ToLower();
            ownId = ownId.Substring(0, Math.Min(ownId.Length, 40)) + "_" + Guid.NewGuid().Substring(10);
            string birthDate = (customer.DateOfBirth ?? new DateTime(1901, 1, 1)).ToString("yyyy-MM-dd");
            var document = Regex.Replace(customer.DocumentNumber, @"\D+", "");

            var newCustomer = new
            {
                ownId,
                fullname = customer.Name,
                email = customer.Email,
                birthDate,
                taxDocument = new
                {
                    type = document.Length == 11 ? "CPF" : "CNPJ",
                    number = document,
                },
                phone = new
                {
                    countryCode = "55",
                    areaCode,
                    number,
                },
                shippingAddress = new
                {
                    city = address.City,
                    complement = address.Complementary ?? "",
                    district = address.Neighborhood,
                    street = address.Street,
                    streetNumber = address.StreetNumber,
                    zipCode = address.ZipCode,
                    state = address.State,
                    country = "BRA",
                }
            };

            return newCustomer;
        }
    }
}