using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using V2.Application.Helpers;

namespace V2.Application.Messages
{
    public class MessagesContext
    {
        private readonly Regex regexEmail = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

        private static readonly object _lock = new object();

        private Dictionary<string, MessageTemplate> _templates;
        private Dictionary<string, MessageSpecification> _specifications;
        private readonly IMessageTemplateRepository _messageTemplateRepository;
        private readonly ITemplateEngine _templateEngine;
        private readonly IEmailSender _messageSender;

        private MessagesContext(IMessageTemplateRepository messageTemplateRepository, ITemplateEngine templateEngine, IEmailSender messageSender)
        {
            _messageTemplateRepository = messageTemplateRepository;
            _templateEngine = templateEngine;
            _messageSender = messageSender;

            Reload();
        }

        public static MessagesContext Current;

        public IReadOnlyDictionary<string, MessageTemplate> Templates => _templates;

        public IReadOnlyDictionary<string, MessageSpecification> Specifications => _specifications;

        public static void Load(IMessageTemplateRepository messageTemplateRepository, ITemplateEngine templateEngine, IEmailSender messageSender)
            => Current = new MessagesContext(messageTemplateRepository, templateEngine, messageSender);

        public void Reload()
        {
            _templates = _messageTemplateRepository
                .List()
                .ToDictionary(i => i.TemplateId, i => i);

            _specifications = ReflectiveEnumerator
                .GetEnumerableOfType<MessageSpecification>()
                .ToDictionary(i => i.TemplateKey, i => i);
        }

        public void UpdateCache(MessageTemplate template)
            => _templates[template.TemplateId] = template;

        public OperationResult<MessageTemplate> GetTemplate<TModel>(MessageSpecification<TModel> spec, string lang)
            where TModel : MessageModel
        {
            var templateId = spec.GetTemplateId(lang);

            lock (_lock)
            {
                if (Current.Templates.ContainsKey(templateId))
                    return Current.Templates[templateId];
            }

            return new ErrorDictionary("", $"Error loading message template '{spec.GetType()}'. Message template not found in MessagesContext.");
        }

        public OperationResult<EmailMessage> Generate<TMessageSpec, TModel>(TModel model, string lang = null)
            where TMessageSpec : MessageSpecification<TModel>, new()
            where TModel : MessageModel
            => Generate(new TMessageSpec(), model, lang);

        public OperationResult GenerateAndSend<TMessageSpec, TModel>(TModel model, string from, IEnumerable<string> to, string lang = null)
            where TMessageSpec : MessageSpecification<TModel>, new()
            where TModel : MessageModel
            => GenerateAndSend(new TMessageSpec(), model, from, to, lang);

        public OperationResult<EmailMessage> Generate<TModel>(MessageSpecification<TModel> messageSpecification, TModel model, string lang = null)
            where TModel : MessageModel
        {
            if (lang == null) lang = CultureInfo.CurrentUICulture.GetCurrentLang();

            var titleResult = _templateEngine.BuildTitle(messageSpecification, model, lang);
            if (!titleResult.OK) return titleResult.Errors;

            var bodyResult = _templateEngine.BuildBody(messageSpecification, model, lang);
            if (!bodyResult.OK) return bodyResult.Errors;

            return EmailMessage.New()
                .WithSubject(titleResult.Result)
                .WithBody(bodyResult.Result);
        }

        public OperationResult GenerateAndSend<TModel>(MessageSpecification<TModel> messageSpecification, TModel model, string from, IEnumerable<string> to, string lang = null)
            where TModel : MessageModel
        {
            if (lang == null) lang = CultureInfo.CurrentUICulture.GetCurrentLang();
            if (lang != null) lang = lang.ToLower();

            var messageTemp = Templates[messageSpecification.TemplateKey + "_" + lang];

            var enabled = messageTemp.Enabled;

            if (!enabled)
                return OperationResult.Success;

            if (regexEmail.IsMatch(from))
                from = regexEmail.Match(from).Value;

            return Generate(messageSpecification, model, lang)
                .Then(result =>
                {
                    result
                        .FromAddress(from)
                        .ToAddresses(to);

                    return new OperationResult<EmailMessage>(result);
                })
                .Then(result => _messageSender.Send(result));
        }
    }
}