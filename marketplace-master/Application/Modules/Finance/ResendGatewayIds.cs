using Application.Sellers;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Services;
using V2.Application.Modules.Finance.Queries;

namespace Application.Modules.Finance
{
    public class ResendGatewayIds
    {
        private readonly IBuyerRepository buyerRepository;
        private readonly BuyerService buyerService;
        private readonly IPaymentService paymentService;
        private readonly SellerAppService sellerAppService;
        private readonly ISellerRepository sellerRepository;
        private readonly IResetGatewayIds resetGatewayIds;

        public ResendGatewayIds(
            IBuyerRepository buyerRepository,
            ISellerRepository sellerRepository,
            IPaymentService paymentService,
            BuyerService buyerService,
            SellerAppService sellerAppService,
            IResetGatewayIds resetGatewayIds)
        {
            this.buyerService = buyerService;
            this.buyerRepository = buyerRepository;
            this.paymentService = paymentService;
            this.resetGatewayIds = resetGatewayIds;
            this.sellerAppService = sellerAppService;
            this.sellerRepository = sellerRepository;
        }

        public void Run()
        {
            resetGatewayIds.RunQuery();
            ReRegisterBuyers(buyerRepository, buyerService);
            ReRegisterSellers(sellerRepository, sellerAppService);
        }

        protected void ReRegisterBuyers(IBuyerRepository repository, BuyerService service)
        {
            var list = repository.List(x => x.Address);

            foreach (var buyer in list)
                service.Update(buyer, null);
        }

        protected void ReRegisterSellers(ISellerRepository repository, SellerAppService service)
        {
            var list = repository.List();

            foreach (var seller in list)
                service.RegisterGateway(seller.Id);
        }
    }
}