using Application.Config;
using System.Collections.Generic;

namespace Domain.Messages
{
    // TODO: Mover para V2.Application
    public class EmailMessage
    {
        public string FromName => MaaSConfig.Current.Environment.Smtp.DefaultFromName;

        public string From { get; set; }

        public List<string> To { get; set; } = new List<string>();

        public string Subject { get; set; }

        public string Body { get; set; }

        public EmailMessage()
        {
        }

        public EmailMessage(string from, IEnumerable<string> to, string subject, string body)
            : this()
        {
            this
                .FromAddress(from)
                .ToAddresses(to)
                .WithSubject(subject)
                .WithBody(body);
        }

        public EmailMessage(string from, string to, string subject, string body)
            : this(from, new[] { to }, subject, body)
        {
        }

        public static EmailMessage New()
        {
            return new EmailMessage();
        }

        private string GetSafeSubject(string input)
        {
            if (input == null) return input;
            input = input.Replace('\r', ' ').Replace('\n', ' ');
            if (input.Length > 128) input = input.Substring(0, 128);
            return input;
        }

        public EmailMessage FromAddress(string from)
        {
            From = from;

            return this;
        }

        public EmailMessage ToAddress(string to)
        {
            To.Add(to);

            return this;
        }

        public EmailMessage ToAddresses(IEnumerable<string> to)
        {
            To.AddRange(to);

            return this;
        }

        public EmailMessage WithSubject(string subject)
        {
            Subject = GetSafeSubject(subject);

            return this;
        }

        public EmailMessage WithBody(string body)
        {
            Body = body;

            return this;
        }
    }
}