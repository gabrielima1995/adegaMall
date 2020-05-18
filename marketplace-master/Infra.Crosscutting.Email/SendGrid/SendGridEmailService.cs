using Exceptions;
using SendGrid;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infra.Crosscutting.Email.SendGrid
{
    public class SendGridEmailService : IEmailService
    {
        public void Deliver(EmailMessage message)
        {
            Deliver(message, SendGridEmailConfig.Get());
        }

        public void Deliver(EmailMessage message, IEmailConfig config)
        {
            DeliverAsync(message, config).Wait();
        }

        public async Task DeliverAsync(EmailMessage message)
        {
            await DeliverAsync(message, SendGridEmailConfig.Get());
        }

        public async Task DeliverAsync(EmailMessage message, IEmailConfig config)
        {
            var subject = EnsureValidSubject(message.Subject);

            var sendGridMessage = new SendGridMessage
            {
                From = new MailAddress(config.FromAddress, config.FromDisplayName),
                Subject = subject,
                Html = message.Body
            };

            var toAddresses = message.Destination.Split(new char[] { ',', ';' })
                .Select(x => x.Trim())
                .Where(x => x.Length > 0).ToList();

            foreach (var addr in toAddresses)
                sendGridMessage.AddTo(addr);

            if (message.Cc != null)
            {
                foreach (var item in message.Cc)
                {
                    sendGridMessage.AddCc(item);
                }
            }

            if (message.Bcc != null)
            {
                foreach (var item in message.Bcc)
                {
                    sendGridMessage.AddBcc(item);
                }
            }

            var credentials = new NetworkCredential(config.UserName, config.Password);
            var transportWeb = new Web(credentials);

            try
            {
                await transportWeb.DeliverAsync(sendGridMessage);
            }
            catch (InvalidApiRequestException ex)
            {
                if (ex.Errors.Any())
                {
                    throw new InvalidOperationException("SendGrid errors: " + string.Join(Environment.NewLine, ex.Errors), ex);
                }

                throw;
            }
        }

        private string EnsureValidSubject(string subject)
        {
            if (subject == null)
                subject = "";

            if (subject.Length > 78)
                subject = subject.Substring(0, 78);

            subject = subject.Replace('\r', ' ').Replace('\n', ' ');

            return subject;
        }
    }
}