using Domain.Core;
using Domain.Events.SellerProfileUpdated;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using System;
using V2.Application.Modules.StoreModule.EventHandlers;

namespace V2.Application.Modules.StoreModule
{
    public class SellerProfileValidationService
    {
        private readonly TimeSpan delayForRunLater = TimeSpan.FromMinutes(1);
        private readonly IBackgroundWorker backgroundWorker;
        private readonly IDomainEventDispatcher eventDispatcher;
        private readonly ISellerRepository sellerRepository;
        private readonly EvaluateSellerCompletionWhenSellerProfileIsUpdated evaluateSellerCompletion;

        public SellerProfileValidationService(
            IBackgroundWorker backgroundWorker,
            IDomainEventDispatcher eventDispatcher,
            ISellerRepository sellerRepository,
            EvaluateSellerCompletionWhenSellerProfileIsUpdated evaluateSellerCompletion)
        {
            this.backgroundWorker = backgroundWorker;
            this.eventDispatcher = eventDispatcher;
            this.sellerRepository = sellerRepository;
            this.evaluateSellerCompletion = evaluateSellerCompletion;
        }

        public void RunLater()
        {
            backgroundWorker.Schedule<SellerProfileValidationService>(x => x.RunInternal(), delayForRunLater);
        }

        public void RunInternal()
        {
            var sellersId = sellerRepository.ListSellerIdsPendingCheckForProfileComplete();

            foreach (var sellerId in sellersId)
            {
                eventDispatcher.Dispatch(new SellerProfileUpdated(sellerId));
            }
        }

        public void RunForAllSellers()
        {
            var sellersId = sellerRepository.ListSellerIds();

            foreach (var sellerId in sellersId)
            {
                evaluateSellerCompletion.Handle(new SellerProfileUpdated(sellerId));
            }
        }
    }
}
