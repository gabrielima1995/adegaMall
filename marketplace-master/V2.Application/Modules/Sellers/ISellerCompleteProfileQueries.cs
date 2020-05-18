using Domain.Modules.StoreModule.CompleteProfile;

namespace V2.Application.Modules.Sellers
{
    public interface ISellerCompleteProfileQueries
    {
        DataForSellerCompleteProfileEvaluationDto GetDataForSeller(string sellerId);
    }
}
