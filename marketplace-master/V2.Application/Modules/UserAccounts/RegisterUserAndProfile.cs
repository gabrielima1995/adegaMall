using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace V2.Application.Modules.UserAccounts
{
    public class RegisterUserAndProfile : IRequest<OperationResult>
    {
        public RegisterUserAndProfile(UserAndProfile userAndProfile, string password)
        {
            UserAndProfile = userAndProfile;
            Password = password;
        }

        public RegisterUserAndProfile(UserAndProfile userAndProfile, UserLoginInfo loginInfo)
        {
            UserAndProfile = userAndProfile;
            LoginInfo = loginInfo;
        }

        public UserAndProfile UserAndProfile { get; }
        public string Password { get; }
        public UserLoginInfo LoginInfo { get; }
    }

    public class RegisterUserAndProfileHandler : IRequestHandler<RegisterUserAndProfile, OperationResult>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPersonRepository personRepository;

        public RegisterUserAndProfileHandler(UserManager<ApplicationUser> userManager, IPersonRepository personRepository)
        {
            this.userManager = userManager;
            this.personRepository = personRepository;
        }

        public async Task<OperationResult> Handle(RegisterUserAndProfile request, CancellationToken cancellationToken)
        {
            var password = request.Password ?? PasswordGenerator.NewUserPassword();
            var createAuthResult = await userManager.CreateAsync(request.UserAndProfile.User, password);
            if (!createAuthResult.Succeeded)
                return new ErrorDictionary(createAuthResult.Errors);

            if (request.LoginInfo != null)
            {
                request.UserAndProfile.User.LoginMode = LoginMode.Provider;
                var addLoginResult = userManager.AddLogin(request.UserAndProfile.User.Id, request.LoginInfo);
                if (!addLoginResult.Succeeded)
                    return new ErrorDictionary(addLoginResult.Errors);
            }

            var personResult = personRepository.Add(request.UserAndProfile.Profile, saveChanges: true);
            if (personResult.IsFailure)
                return personResult;

            var role = GetRole(request.UserAndProfile.Profile);
            var roleResult = userManager.AddToRole(request.UserAndProfile.User.Id, role);
            if (!roleResult.Succeeded)
                return new ErrorDictionary(roleResult.Errors);

            return OperationResult.Success;
        }

        private string GetRole(Person profile)
        {
            if (profile is AdminPerson)
                return AppRole.Admin;

            if (profile is Buyer)
                return AppRole.Buyer;

            if (profile is Seller)
                return AppRole.Seller;

            if (profile is SellerAssistant)
                return AppRole.SellerAssistant;

            throw new ArgumentOutOfRangeException($"A role has not been defined for profile type '{profile.GetType().Name}'.");
        }
    }
}
