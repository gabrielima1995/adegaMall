using Domain.DTO;
using Framework;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPostalService
    {
        OperationResult<decimal> GetShippingCost(ParcelInfo parcelInfo);

        Task<OperationResult<decimal>> GetShippingCostAsync(ParcelInfo parcelInfo);
    }
}
