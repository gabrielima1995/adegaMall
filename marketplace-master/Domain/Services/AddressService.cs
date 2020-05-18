using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;

namespace Domain.Addresses
{
    public class AddressService : ServiceBase<Address>
    {
        private readonly IRepository<ShippingAddress> _shippingAddressRepository;

        public AddressService(
            IRepository<Address> repository,
            IRepository<ShippingAddress> shippingAddressRepository)
            : base(repository)
        {
            _shippingAddressRepository = shippingAddressRepository;
        }

        public void Insert(ShippingAddress shippingAddress)
        {
            _shippingAddressRepository.Insert(shippingAddress);
            _shippingAddressRepository.SaveChanges();
        }
    }
}