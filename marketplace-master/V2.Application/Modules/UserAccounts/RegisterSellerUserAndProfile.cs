using Domain.Core;
using Domain.Entities;
using Domain.Events;
using Domain.Events.SellerProfileUpdated;
using Domain.Sellers;
using Domain.Services;
using Framework;
using MediatR;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace V2.Application.Modules.UserAccounts
{
    public class RegisterSellerUserAndProfile : IRequest<OperationResult>
    {
        public RegisterSellerUserAndProfile(SellerUserAndProfile userAndProfile, string password)
        {
            UserAndProfile = userAndProfile;
            Password = password;
        }

        public RegisterSellerUserAndProfile(SellerUserAndProfile userAndProfile, UserLoginInfo loginInfo)
        {
            UserAndProfile = userAndProfile;
            LoginInfo = loginInfo;
        }

        public SellerUserAndProfile UserAndProfile { get; }
        public string Password { get; }
        public UserLoginInfo LoginInfo { get; }
    }

    public class RegisterSellerUserAndProfileHandler : IRequestHandler<RegisterSellerUserAndProfile, OperationResult>
    {
        private readonly IMediator mediator;
        private readonly IRepository<ApplicationUser> userRepository;
        private readonly SellerService_Obsolete sellerService;
        private readonly IDomainEventDispatcher dispatcher;
        private readonly AlternativeNameGenerator alternativeNameGenerator;
        private readonly NicknameGenerator nicknameGenerator;
        private readonly FriendlyUrlGenerator friendlyUrlGenerator;

        public RegisterSellerUserAndProfileHandler(
            IMediator mediator,
            IRepository<ApplicationUser> userRepository,
            SellerService_Obsolete sellerService,
            IDomainEventDispatcher dispatcher,
            AlternativeNameGenerator alternativeNameGenerator,
            NicknameGenerator nicknameGenerator,
            FriendlyUrlGenerator friendlyUrlGenerator)
        {
            this.mediator = mediator;
            this.userRepository = userRepository;
            this.sellerService = sellerService;
            this.dispatcher = dispatcher;
            this.alternativeNameGenerator = alternativeNameGenerator;
            this.nicknameGenerator = nicknameGenerator;
            this.friendlyUrlGenerator = friendlyUrlGenerator;
        }

        public async Task<OperationResult> Handle(RegisterSellerUserAndProfile request, CancellationToken cancellationToken)
        {
            var profile = request.UserAndProfile.Profile;

            if (userRepository.Query().Any(i => i.UserName == profile.Email))
                return new ErrorDictionary(nameof(profile.Email), Thread.CurrentThread.GetStringLocalized("EMAIL_ALREADY_EXISTS_ERROR_MESSAGE"));

            if (sellerService.Exists(i => i.Nickname == profile.Nickname))
                return new ErrorDictionary(nameof(profile.Nickname), Thread.CurrentThread.GetStringLocalized("NICKNAME_ALREADY_EXISTS_ERROR_MESSAGE"));

            alternativeNameGenerator.Generate(profile);
            nicknameGenerator.Generate(profile);
            friendlyUrlGenerator.Generate(profile);
            profile.CalculateScore();

            var result = await mediator.Send(new RegisterUserAndProfile(request.UserAndProfile, request.Password));
            if (result.IsFailure)
                return result.Errors;

            dispatcher.Dispatch(new SellerRegisteredEvent(request.UserAndProfile.Profile));
            dispatcher.Dispatch(new SellerProfileUpdated(request.UserAndProfile.Profile.Id));

            return result;
        }
    }
}
