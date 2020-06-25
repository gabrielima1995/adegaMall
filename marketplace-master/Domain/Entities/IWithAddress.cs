namespace Domain.Entities
{
    public interface IWithAddress : IWithCity
    {
        string Address { get; set; }

        string AddressNeighborhood { get; set; }

        string AddressNumber { get; set; }

        string AddressZipCode { get; set; }

        string AddressAdditionalInfo { get; set; }
    }
}
