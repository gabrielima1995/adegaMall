using Domain.Core;
using Domain.Entities;
using Domain.Events;
using Framework;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace V2.Application.Modules.UserAccounts
{
    public class RegisterSellerAssistantUserAndProfile : IRequest<OperationResult>
    {
        public RegisterSellerAssistantUserAndProfile(SellerAssistantUserAndProfile userAndProfile)
        {
            UserAndProfile = userAndProfile;
        }

        public SellerAssistantUserAndProfile UserAndProfile { get; }
    }

    public class RegisterSellerAssistantUserAndProfileHandler : IRequestHandler<RegisterSellerAssistantUserAndProfile, OperationResult>
    {
        private readonly IMediator mediator;
        private readonly IRepository<ApplicationUser> userRepository;
        private readonly IDomainEventDispatcher dispatcher;

        public RegisterSellerAssistantUserAndProfileHandler(
            IMediator mediator,
            IRepository<ApplicationUser> userRepository,
            IDomainEventDispatcher dispatcher)
        {
            this.mediator = mediator;
            this.userRepository = userRepository;
            this.dispatcher = dispatcher;
        }

        public async Task<OperationResult> Handle(RegisterSellerAssistantUserAndProfile request, CancellationToken cancellationToken)
        {
            var profile = request.UserAndProfile.Profile;

            if (userRepository.Query().Any(i => i.UserName == profile.Email))
                return new ErrorDictionary(nameof(profile.Email), Thread.CurrentThread.GetStringLocalized("EMAIL_ALREADY_EXISTS_ERROR_MESSAGE"));

            var password = PasswordGenerator.NewUserPassword();
            var result = await mediator.Send(new RegisterUserAndProfile(request.UserAndProfile, password));
            if (result.IsFailure)
                return result.Errors;

            dispatcher.Dispatch(new SellerAssistantRegisteredEvent(request.UserAndProfile.Profile, password));

            return result;
        }
    }
}
