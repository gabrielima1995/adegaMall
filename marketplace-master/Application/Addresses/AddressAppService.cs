using Domain;
using Domain.Addresses;
using Domain.DTO;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Interfaces;
using Geocoding;
using System;

namespace Application.Addresses
{
    public class AddressAppService : IAddressAppService
    {
        private readonly AddressService _addressService;
        private readonly PersonService _personService;
        private readonly IGeocoder _geocoder;

        public AddressAppService(
            AddressService addressService,
            PersonService personService,
            IGeocoder geocoder)
        {
            _addressService = addressService;
            _personService = personService;
            _geocoder = geocoder;
        }

        public void AddNewShippingAddress(AddressRegisterModel model)
        {
            var person = _personService.Get(i => i.Id == model.PersonId);

            var shippingAddress = new ShippingAddress
            {
                Address = model.GetAddress(),
                Person = person
            };

            _geocoder.UpdateLocation(shippingAddress.Address);
            _addressService.Insert(shippingAddress);
        }

        public void DeleteShippingAddressAndAddress(Guid addressId)
        {
            _addressService.Delete(x => x.Id == addressId);
        }
    }
}