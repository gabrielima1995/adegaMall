using Application.Config;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Framework;
using Infra.Crosscutting.Globalization;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace V2.Application.Modules.UserAccounts
{
    public class RegisterAdditionalProfile : IRequest<OperationResult<UserAndProfile>>
    {
        public RegisterAdditionalProfile(string userId, string originalPersonId, PersonType newProfileType)
        {
            UserId = userId;
            OriginalPersonId = originalPersonId;
            NewProfileType = newProfileType;
        }

        public string UserId { get; }
        public string OriginalPersonId { get; }
        public PersonType NewProfileType { get; }
    }

    public class RegisterAdditionalProfileHandler : IRequestHandler<RegisterAdditionalProfile, OperationResult<UserAndProfile>>
    {
        private readonly IRepository<ApplicationUser> userRepository;
        private readonly IPersonRepository personRepository;
        private readonly IMediator mediator;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AlternativeNameGenerator alternativeNameGenerator;
        private readonly NicknameGenerator nicknameGenerator;
        private readonly FriendlyUrlGenerator friendlyUrlGenerator;

        public RegisterAdditionalProfileHandler(
            IRepository<ApplicationUser> userRepository,
            IPersonRepository personRepository,
            IMediator mediator, UserManager<ApplicationUser> userManager,
            AlternativeNameGenerator alternativeNameGenerator,
            NicknameGenerator nicknameGenerator,
            FriendlyUrlGenerator friendlyUrlGenerator)
        {
            this.userRepository = userRepository;
            this.personRepository = personRepository;
            this.mediator = mediator;
            this.userManager = userManager;
            this.alternativeNameGenerator = alternativeNameGenerator;
            this.nicknameGenerator = nicknameGenerator;
            this.friendlyUrlGenerator = friendlyUrlGenerator;
        }

        public async Task<OperationResult<UserAndProfile>> Handle(RegisterAdditionalProfile request, CancellationToken cancellationToken)
        {
            var user = userRepository.Get(theUser => theUser.Id == request.UserId);
            if (user == null) return new ErrorDictionary("", GlobalizationHelper.GetString("ADDITIONAL_PROFILE_USER_NOT_FOUND"));

            var originalPerson = personRepository.GetById(request.OriginalPersonId);
            if (originalPerson == null) new ErrorDictionary("", GlobalizationHelper.GetString("ADDITIONAL_PROFILE_PERSON_NOT_FOUND"));

            if (user.Id != originalPerson.UserId) new ErrorDictionary("", GlobalizationHelper.GetString("ADDITIONAL_PROFILE_POSSIBLE_FRAUD_ATTEMPT"));

            var allUserProfiles = await mediator.Send(new ListProfilesForUser(request.UserId));
            if (allUserProfiles.Profiles.Any(profile => profile.Type == request.NewProfileType)) new ErrorDictionary("", GlobalizationHelper.GetString("ADDITIONAL_PROFILE_TYPE_DUPLICATE"));

            var newProfile = BuildNewProfile(user, originalPerson, request.NewProfileType);
            var personResult = personRepository.Add(newProfile, saveChanges: true);
            if (personResult.IsFailure) return personResult.Errors;

            var role = GetRole(newProfile);
            var roleResult = userManager.AddToRole(user.Id, role);
            if (!roleResult.Succeeded) return new ErrorDictionary(roleResult.Errors);

            return new UserAndProfile(user, newProfile);
        }

        private string GetRole(Person profile)
        {
            if (profile is Buyer) return AppRole.Buyer;
            if (profile is Seller) return AppRole.Seller;
            throw new ArgumentOutOfRangeException($"A role has not been defined for profile type '{profile.GetType().Name}'.");
        }

        private Person BuildNewProfile(ApplicationUser user, Person originalPerson, PersonType newProfileType)
        {
            var newProfile =
                newProfileType == PersonType.Buyer ? new Buyer(user.Id, user.Email, originalPerson.FullName, MaaSConfig.Current.Buyer.Moderation.Enabled) as Person :
                newProfileType == PersonType.Seller ? new Seller(user.Id, user.Email, MaaSConfig.Current.Seller.Moderation.Enabled) :
                throw new InvalidOperationException(GlobalizationHelper.GetString("ADDITIONAL_PROFILE_UNSUPPORTED_TYPE"));

            newProfile.FullName = originalPerson.FullName ?? newProfile.Email;
            newProfile.AlternativeName = originalPerson.AlternativeName ?? originalPerson.FullName;
            newProfile.PhoneNumber = originalPerson.PhoneNumber;
            newProfile.Lang = originalPerson.Lang;
            newProfile.PreferredTimeZoneId = originalPerson.PreferredTimeZoneId;

            if (newProfile is Seller seller)
            {
                alternativeNameGenerator.Generate(seller);
                nicknameGenerator.Generate(seller);
                friendlyUrlGenerator.Generate(seller);
                seller.CalculateScore();
            }

            return newProfile;
        }
    }
}
