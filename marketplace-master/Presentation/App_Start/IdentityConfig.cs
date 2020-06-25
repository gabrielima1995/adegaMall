using Application.Config;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Messages;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Presentation.Configuration;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation
{
    public class IdentityEmailService : IIdentityMessageService
    {
        private readonly IEmailSender _emailSender;

        public IdentityEmailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public Task SendAsync(IdentityMessage message)
        {
            _emailSender.Send(new EmailMessage(
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                message.Destination,
                message.Subject,
                message.Body));

            return Task.CompletedTask;
        }
    }

    //public class SmsService : IIdentityMessageService
    //{
    //    public Task SendAsync(IdentityMessage message)
    //    {
    //        // Plug in your SMS service here to send a text message.
    //        return Task.FromResult(0);
    //    }
    //}

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            RegisterDataProtectionProvider(this);
        }

        private static void RegisterDataProtectionProvider(ApplicationUserManager manager)
        {
            manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                AuthenticationConfigurationExtensions.DataProtectionProvider.Create("ASP.NET Identity"));
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = UnityConfig.Resolve<ApplicationUserManager>();

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            //manager.SmsService = new SmsService();

            manager.EmailService = UnityConfig.Resolve<IdentityEmailService>();

            RegisterDataProtectionProvider(manager);

            return manager;
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}