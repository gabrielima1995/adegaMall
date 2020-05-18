using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;

namespace V2.Application.Modules.UserAccounts.Messages.Specs
{
    public class ResetPasswordMessageSpecification : MessageSpecification<ResetPasswordMessageModel>
    {
        public override string TitleLabel => "RESET_PASSWORD_MESSAGE_TITLE";

        public override string DescriptionLabel => "RESET_PASSWORD_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RESET_PASSWORD_SUBJECT";

        public ResetPasswordMessageSpecification() : base("Account")
        {
        }
    }
}