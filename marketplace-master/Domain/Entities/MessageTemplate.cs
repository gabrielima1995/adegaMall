using Domain.DTO;
using Framework;
using System;

namespace Domain.Entities
{
    public class MessageTemplate : BaseEntity<int>
    {
        private string _bodyHash;

        private string _subjectHash;

        public string Key { get; private set; }

        public string Lang { get; private set; }

        public string SubjectTemplate { get; private set; }

        public string BodyTemplate { get; private set; }

        public bool Enabled { get; private set; }

        public string TemplateId => Key + "_" + Lang;

        private MessageTemplate()
        {
            Enabled = true;
        }

        private OperationResult<MessageTemplate> Validate()
        {
            StringHelper.Sanitize(this);

            if (string.IsNullOrWhiteSpace(SubjectTemplate))
            {
                return OperationResult<MessageTemplate>.Fail("SubjectTemplate", "SubjectTemplate required.");
            }

            if (string.IsNullOrWhiteSpace(BodyTemplate))
            {
                return OperationResult<MessageTemplate>.Fail("BodyTemplate", "BodyTemplate required.");
            }

            if (string.IsNullOrWhiteSpace(Lang))
            {
                return OperationResult<MessageTemplate>.Fail("Lang", "Lang required.");
            }

            return this;
        }

        public static OperationResult<MessageTemplate> New(string key, string lang, string subjectTemplate, string bodyTemplate)
        {
            return new MessageTemplate
            {
                Key = key,
                Lang = lang,
                SubjectTemplate = subjectTemplate,
                BodyTemplate = bodyTemplate
            }
            .Validate();
        }

        public static OperationResult<MessageTemplate> Create(MessageTemplateCreating model)
        {
            return New(model.Key, model.Lang, model.SubjectTemplate, model.BodyTemplate);
        }

        public OperationResult<MessageTemplate> Update(MessageTemplateEditing model)
        {
            SubjectTemplate = model.SubjectTemplate;
            BodyTemplate = model.BodyTemplate;
            Enabled = model.Enabled;

            return Validate();
        }

        public string GetSubjectHash()
        {
            if (string.IsNullOrEmpty(_subjectHash))
            {
                _subjectHash = "-subject-" + MD5.Encode(SubjectTemplate);
            }

            return _subjectHash;
        }

        public string GetBodyHash()
        {
            if (string.IsNullOrEmpty(_bodyHash))
            {
                _bodyHash = "-body-" + MD5.Encode(BodyTemplate);
            }

            return _bodyHash;
        }
    }
}