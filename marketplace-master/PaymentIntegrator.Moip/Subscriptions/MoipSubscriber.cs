using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Moip.Helpers;
using System;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip.Subscriptions
{
    public static class MoipSubscriber
    {
        public static object CreateSubscriber(ICustomer customer, IAddress address)
        {
            PhoneHelper.ExtractNumbers(customer.PhoneNumber, out string phone_area_code, out string phone_number);

            var docNumber = Regex.Replace(customer.DocumentNumber, @"\D+", "");
            if (docNumber.Length > 11 && !string.IsNullOrWhiteSpace(customer.AccountableDocument))
                docNumber = Regex.Replace(customer.AccountableDocument, @"\D+", "");

            if (customer.DateOfBirth == null)
                throw new Exception("A data de nascimento deve estar preenchida para criar um registro no meio de pagamento.");

            if (address == null || address.Street == null || address.StreetNumber == null || address.City == null || address.Neighborhood == null || address.State == null || address.ZipCode == null)
                throw new Exception("O endereço deve estar completamente preenchido para criar um registro no meio de pagamento.");

            var subscriber = new
            {
                code = customer.GatewayId,
                fullname = customer.Name,
                email = customer.Email,
                cpf = docNumber,
                phone_area_code,
                phone_number,
                birthdate_day = customer.DateOfBirth.Value.Day,
                birthdate_month = customer.DateOfBirth.Value.Month,
                birthdate_year = customer.DateOfBirth.Value.Year,
                address = new
                {
                    street = address.Street,
                    number = address.StreetNumber,
                    city = address.City,
                    district = address.Neighborhood,
                    state = address.State,
                    country = "BRA",
                    zipcode = (address.ZipCode).Replace(" ", "").Replace("-", ""),
                },
            };

            return subscriber;
        }
    }
}