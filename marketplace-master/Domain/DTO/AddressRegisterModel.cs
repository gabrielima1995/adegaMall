using Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{

    public class AddressRegisterModel : IWithAddress
    {
        public string PersonId { get; set; }

        [DisplayNameLocalized("ADDRESS_LABEL")]
        [StringLengthLocalized(1024)]
        [RequiredLocalized]
        public string Address { get; set; }

        [DisplayNameLocalized("ADDRESS_NUMBER_LABEL")]
        [StringLengthLocalized(20)]
        [RequiredLocalized]
        public string AddressNumber { get; set; }

        [DisplayNameLocalized("NEIGHBORHOOD_LABEL")]
        [StringLengthLocalized(100)]
        [RequiredLocalized]
        public string AddressNeighborhood { get; set; }

        [DisplayNameLocalized("ZIPCODE_LABEL")]
        [StringLengthLocalized(15)]
        [RequiredLocalized]
        public string AddressZipCode { get; set; }

        [DisplayNameLocalized("CITY_AND_STATE_LABEL")]
        [StringLengthLocalized(100)]
        [RequiredLocalized]
        public string CityAndState { get; set; }

        [DisplayNameLocalized("ADDRESS_ADDITIONAL_INFO_LABEL")]
        [StringLengthLocalized(30)]
        public string AddressAdditionalInfo { get; set; }

        public Address GetAddress()
        {
            return new Address
            {
                Street = Address,
                Number = AddressNumber,
                Neighborhood = AddressNeighborhood,
                ZipCode = AddressZipCode,
                CityAndState = CityAndState,
                AdditionalInfo = AddressAdditionalInfo
            };
        }
    }
}
