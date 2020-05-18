using Domain.Entities;
using Framework;
using Framework.Geography;

namespace Domain.Interfaces
{
    public interface IGeocodingService
    {
        OperationResult<GeoCoordinate> GetCoordinates(Address address);
    }
}
