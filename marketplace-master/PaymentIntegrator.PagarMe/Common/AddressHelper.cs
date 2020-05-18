using PagarMe;
using PaymentIntegrator.Core.Common;

namespace PaymentIntegrator.PagarMe.Common
{
    public static class AddressHelper
    {
        public static Address CreateToPagarMe(this IAddress address, PagarMeClient client)
        {
            if (address == null) return null;

            return new Address(client.GetService())
            {
                Street = address.Street,
                StreetNumber = address.StreetNumber,
                Complementary = address.Complementary,
                Neighborhood = address.Neighborhood,
                City = address.City,
                State = address.State,
                Zipcode = address.ZipCode
            };
        }
    }
}