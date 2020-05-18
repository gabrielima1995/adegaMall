using Domain.Core;
using Domain.Entities;
using Domain.Events;
using Framework;
using MediatR;
using Microsoft.AspNet.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace V2.Application.Modules.UserAccounts
{
    public class RegisterBuyerUserAndProfile : IRequest<OperationResult>
    {
        public RegisterBuyerUserAndProfile(BuyerUserAndProfile userAndProfile, string password)
        {
            UserAndProfile = userAndProfile;
            Password = password;
        }

        public RegisterBuyerUserAndProfile(BuyerUserAndProfile userAndProfile, UserLoginInfo loginInfo)
        {
            UserAndProfile = userAndProfile;
            LoginInfo = loginInfo;
        }

        public BuyerUserAndProfile UserAndProfile { get; }
        public string Password { get; }
        public UserLoginInfo LoginInfo { get; }
    }

    public class RegisterBuyerUserAndProfileHandler : IRequestHandler<RegisterBuyerUserAndProfile, OperationResult>
    {
        private readonly IMediator mediator;
        private readonly IDomainEventDispatcher dispatcher;

        public RegisterBuyerUserAndProfileHandler(
            IMediator mediator,
            IDomainEventDispatcher dispatcher)
        {
            this.mediator = mediator;
            this.dispatcher = dispatcher;
        }

        public async Task<OperationResult> Handle(RegisterBuyerUserAndProfile request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new RegisterUserAndProfile(request.UserAndProfile, request.Password));
            if (result.IsFailure)
                return result.Errors;

            dispatcher.Dispatch(new BuyerRegisteredEvent(request.UserAndProfile.Profile));
            return result;
        }
    }
}
