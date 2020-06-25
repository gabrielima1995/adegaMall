using Application.Config;
using Domain.Interfaces;
using Domain.Messages;
using Framework;
using System.Net;
using System.Net.Mail;

namespace V2.Infrastructure.Messages
{
    public class EmailSender : IEmailSender
    {
        public OperationResult Send(EmailMessage emailMessage)
        {
            var msg = new MailMessage
            {
                From = new MailAddress(emailMessage.From, emailMessage.FromName),
                Subject = GetSafeSubject(emailMessage.Subject),
                Body = emailMessage.Body,
                IsBodyHtml = true
            };

            foreach (var to in emailMessage.To)
                msg.To.Add(new MailAddress(to));

            var smtpConfig = MaaSConfig.Current.Environment.Smtp;
            var client = new SmtpClient(smtpConfig.Host, smtpConfig.Port)
            {
                Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password)
            };

            client.Send(msg);
            return OperationResult.Success;
        }

        private string GetSafeSubject(string input)
        {
            if (input == null) return input;
            input = input.Replace('\r', ' ').Replace('\n', ' ');
            if (input.Length > 128) input = input.Substring(0, 128);
            return input;
        }
    }


    public class BackgroundEmailSender : IEmailSender
    {
        private readonly IBackgroundWorker backgroundWorker;

        public BackgroundEmailSender(IBackgroundWorker backgroundWorker)
        {
            this.backgroundWorker = backgroundWorker;
        }

        public OperationResult Send(EmailMessage emailMessage)
        {
            backgroundWorker.Enqueue<EmailSender>(sender => sender.Send(emailMessage));
            return OperationResult.Success;
        }
    }
}