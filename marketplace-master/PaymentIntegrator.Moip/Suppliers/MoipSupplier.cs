using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Moip.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip.Suppliers
{
    internal static class MoipSupplier
    {
        private static readonly Regex AreaCodeRegex = new Regex(@"\(?([1-9]{2})\)?\s?([0-9\- \.]*)$", RegexOptions.Compiled);

        private static object GetPhone(string phone)
        {
            var match = AreaCodeRegex.Match(phone);
            string areaCode = match.Groups[1].Value;
            string number = Regex.Replace(match.Groups[2].Value, "[^0-9]", string.Empty);

            return new
            {
                countryCode = "55",
                areaCode,
                number,
            };
        }

        private static object GetAddress(IAddress address)
        {
            return new
            {
                city = address.City,
                complement = address.Complementary ?? "",
                district = address.Neighborhood,
                street = address.Street,
                streetNumber = address.StreetNumber.Trim(),
                zipCode = address.ZipCode.Trim(),
                state = address.State.Trim(),
                country = "BRA",
            };
        }

        public static object Create(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable, IAddress accountableAddress)
        {
            //Extract area code and number from full phone number

            var supplierDocDigits = Regex.Replace(supplier.DocumentNumber, "[^0-9]", string.Empty);
            bool isCompany = supplierDocDigits.Length == 14;

            string personName = accountable.Name;

            //Split first and last name from full name
            string name = personName.Split(' ').First(x => x.Length > 1);
            string lastName = personName.Substring(personName.IndexOf(name) + name.Length).Trim();

            //Parse birth date format
            string birthDate = (supplier.DateOfBirth ?? new DateTime(1901, 1, 1)).ToString("yyyy-MM-dd");

            //Assemble structures for serialization
            var newPhone = GetPhone(supplier.Phone);
            var newAddress = GetAddress(address);
            var newAccountablePhone = GetPhone(accountable.PhoneNumber);
            var newAccountableAddress = GetAddress(accountableAddress);

            object obj;

            //Construct final supplier object for request
            if (isCompany)
            {
                obj = new
                {
                    email = new
                    {
                        address = supplier.Email
                    },
                    person = new
                    {
                        name,
                        lastName,
                        taxDocument = new
                        {
                            type = "CPF",
                            number = DocumentHelper.FormatDocument(accountable.DocumentNumber),
                        },
                        birthDate,
                        phone = newAccountablePhone,
                        address = newAccountableAddress,
                    },
                    company = new
                    {
                        name = supplier.AlternativeName,
                        businessName = supplier.Name,
                        taxDocument = new
                        {
                            type = "CNPJ",
                            number = DocumentHelper.FormatDocument(supplier.DocumentNumber),
                        },
                        phone = newPhone,
                        address = newAddress,
                    },
                    type = "MERCHANT",
                    transparentAccount = true,
                };
            }
            else
            {
                obj = new
                {
                    email = new
                    {
                        address = supplier.Email
                    },
                    person = new
                    {
                        name,
                        lastName,
                        taxDocument = new
                        {
                            type = "CPF",
                            number = DocumentHelper.FormatDocument(accountable.DocumentNumber),
                        },
                        birthDate,
                        phone = newAccountablePhone,
                        address = newAccountableAddress,
                    },
                    type = "MERCHANT",
                    transparentAccount = true,
                };
            }

            return obj;
        }
    }
}