using Application.Config;
using Domain.Entities;
using Domain.Events;
using Domain.Events.QuoteRequests;
using Domain.Interfaces;
using Domain.Messages;
using Framework;
using System.Linq;

namespace V2.Application.Modules.QuoteRequests.EventHandlers
{
    public class SendEmailToAdminsWhenQuoteWithoutSellerWasRegistered : IDomainEventHandler<NewQuoteGroupRegisteredEvent>
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        private readonly IRepository<AdminPerson> adminRepository;
        private readonly IEmailSender emailSender;

        public SendEmailToAdminsWhenQuoteWithoutSellerWasRegistered(
            IRepository<AdminPerson> adminRepository,
            IEmailSender emailSender)
        {
            this.adminRepository = adminRepository;
            this.emailSender = emailSender;
        }

        public void Handle(NewQuoteGroupRegisteredEvent domainEvent)
        {
            if (domainEvent.HasSeller)
                return;

            var admins = adminRepository.Query().ToList();

            foreach (var admin in admins)
            {
                var message = GetEmailMessage(admin);
                SendEmail(message);
            }
        }

        private EmailMessage GetEmailMessage(AdminPerson admin)
        {
            var subject = $"{config.ApplicationName}: Orçamento sem vendedores";
            var body = GetMessageBody(admin);
            return new EmailMessage(config.Environment.Smtp.DefaultFromAddress, admin.Email, subject, body);
        }

        private string GetMessageBody(AdminPerson admin)
        {
            var url = config.Environment.AppUrl + "admin/quotes/withoutseller";

            return (
                "<p>Olá, {ADMIN_NAME}</p>" +
                "<p>Sua plataforma recebeu um pedido de orçamento sem vendedores que possam atendê-lo.</p>" +
                $"<p>O pedido foi listado em <a href=\"{url}\">{url}</a></p>" +
                "<p>Acesse o painel agora mesmo para encaminhar a um vendedor que possa atendê-lo, ou para comunicar o cliente.</p>" +
                "<p>Equipe de Marketplace do Ideia no Ar</p>"
            ).Replace("{ADMIN_NAME}", admin.FullName);
        }

        private void SendEmail(EmailMessage message)
        {
            emailSender.Send(message);
        }
    }
}