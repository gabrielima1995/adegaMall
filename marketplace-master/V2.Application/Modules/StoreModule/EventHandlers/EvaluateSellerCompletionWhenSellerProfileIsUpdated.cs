using Application.Config;
using Domain.Events;
using Domain.Events.SellerProfileUpdated;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.CompleteProfile;
using V2.Application.Modules.Sellers;

namespace V2.Application.Modules.StoreModule.EventHandlers
{
    public class EvaluateSellerCompletionWhenSellerProfileIsUpdated : IDomainEventHandler<SellerProfileUpdated>
    {
        private readonly ISellerRepository repository;
        private readonly ISellerCompleteProfileQueries queries;

        public EvaluateSellerCompletionWhenSellerProfileIsUpdated(
            ISellerRepository repository,
            ISellerCompleteProfileQueries queries)
        {
            this.repository = repository;
            this.queries = queries;
        }

        public void Handle(SellerProfileUpdated sellerDomainEvent)
        {
            var seller = repository.GetById(sellerDomainEvent.SellerId);
            var completeProfile = seller.SellerCompleteProfileMapped;

            var data = queries.GetDataForSeller(sellerDomainEvent.SellerId);
            completeProfile.EvaluateAndUpdateCompletion(data, MaaSConfig.Current);

            seller.SellerCompleteProfileMapped = completeProfile;
            seller.SellerCompleteProfileVersion = completeProfile.Version;

            repository.SaveChanges();
        }
    }
}
