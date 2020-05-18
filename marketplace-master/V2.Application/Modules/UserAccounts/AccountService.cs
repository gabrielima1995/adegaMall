using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Messages;
using Framework;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;
using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;
using V2.Application.Modules.UserAccounts.Messages.Specs;

namespace V2.Application.Modules.UserAccounts
{
    public class AccountService
    {
        private readonly IEmailSender _messageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(
            IEmailSender messageService,
            UserManager<ApplicationUser> userManager)
        {
            _messageService = messageService;
            _userManager = userManager;
        }

        public async Task<OperationResult> ForgotPasswordAsync(ForgotPasswordViewModel model, UrlHelper urlHelper)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new OperationResult("", CultureInfo.CurrentCulture.GetStringLocalized("ACCOUNT_NOT_FOUND_MESSAGE"));

            if (string.IsNullOrEmpty(user.PasswordHash))
                return new OperationResult("", CultureInfo.CurrentCulture.GetStringLocalized("RESET_NOT_PASSWORD_ERROR_MESSAGE"));

            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

            var messageModel = new ResetPasswordMessageModel
            {
                Url = MaaSConfig.Current.Environment.AppUrl.TrimEnd('/') + urlHelper.Action("ResetPassword", "Account", new { userId = user.Id, code }),
                UserName = user.UserName
            };

            return MessagesContext.Current.Generate(new ResetPasswordMessageSpecification(), messageModel)
                .Then(message =>
                {
                    message
                        .ToAddress(user.Email)
                        .FromAddress(MaaSConfig.Current.Environment.Smtp.DefaultFromAddress);

                    return new OperationResult<EmailMessage>(message);
                })
                .Then(message => _messageService.Send(message));
        }

        public async Task<OperationResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new OperationResult("", CultureInfo.CurrentCulture.GetStringLocalized("ACCOUNT_NOT_FOUND_MESSAGE"));

            if (string.IsNullOrEmpty(user.PasswordHash))
                return new OperationResult("", CultureInfo.CurrentCulture.GetStringLocalized("RESET_NOT_PASSWORD_ERROR_MESSAGE"));

            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            return result.ToOperationResult();
        }
    }
}