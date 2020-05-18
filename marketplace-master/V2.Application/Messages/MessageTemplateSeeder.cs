using Application.Config;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace V2.Application.Messages
{
    public class MessageTemplateSeeder
    {
        private readonly object _lock = new object();
        private readonly object _lock2 = new object();

        private readonly MessagesContext Context = MessagesContext.Current;

        private string[] _resourceNames;
        private Assembly _assembly;

        private readonly IMessageTemplateRepository _messageTemplateRepository;

        public MessageTemplateSeeder(
            IMessageTemplateRepository messageTemplateRepository)
        {
            _messageTemplateRepository = messageTemplateRepository;
        }

        private OperationResult<MessageTemplate> GetMessageTemplate(MessageSpecification spec, string lang)
        {
            var templateId = spec.GetTemplateId(lang);

            lock (_lock2)
            {
                if (!Context.Templates.ContainsKey(templateId))
                {
                    var resourceQualifiedName = _resourceNames.FirstOrDefault(i => i.Contains(templateId));

                    if (resourceQualifiedName == null)
                        throw new Exception($"Error loading message template '{spec.GetType()}'. Not found in manifest resource."); ;

                    string template = null;

                    try
                    {
                        using (var stream = _assembly.GetManifestResourceStream(resourceQualifiedName))
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                template = reader.ReadToEnd();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error loading message template '{spec.GetType()}'. " + ex.Message);
                    }

                    return MessageTemplate.New(spec.TemplateKey, lang, GlobalizationHelper.GetString(spec.DefaultSubjectLabel, lang), template);
                }
            }

            return Context.Templates[templateId];
        }

        public void PopulateDatabase()
        {
#if DEBUG
            if (Context == null)
                return;
#endif
            lock (_lock)
            {

                _assembly = Assembly.GetAssembly(typeof(MessageTemplateSeeder));
                _resourceNames = _assembly.GetManifestResourceNames();

                if (MaaSConfig.Current == null) return;

                var langs = MaaSConfig.Current.Localization.GetSupportedLanguages();
                var results = new List<OperationResult>();

                foreach (var lang in langs)
                {
                    foreach (var spec in Context.Specifications.Values)
                    {
                        results.Add(
                            GetMessageTemplate(spec, lang)
                                .Then(messageTemplate =>
                                {
                                    if (messageTemplate.Id == 0)
                                    {
                                        return _messageTemplateRepository.Add(messageTemplate, true);
                                    }

                                    return OperationResult.Success;
                                }));
                    }
                }

                Context.Reload();

                if (results.Exists(i => !i.OK))
                {
                    var errors = results.Where(i => !i.OK).Select(i => i.Errors.ToSimpleString());

                    throw new Exception(string.Join("\r\n", errors));
                }
            }
        }
    }
}