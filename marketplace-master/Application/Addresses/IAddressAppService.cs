using Domain.DTO;
using System;

namespace Application.Addresses
{
    public interface IAddressAppService
    {
        void AddNewShippingAddress(AddressRegisterModel model);
        void DeleteShippingAddressAndAddress(Guid shippingAddressId);
    }
}