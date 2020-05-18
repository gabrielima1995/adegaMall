using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace V2.Application.Messages
{
    public abstract class MessageSpecification
    {
        protected MessageSpecification(string module, Type messageModelType, string templateKey = null)
        {
            Module = module;
            MessageModelType = messageModelType;
            TemplateKey = templateKey ?? GetTemplateKeyByClassName();
        }

        public string Module { get; private set; }
        public string TemplateKey { get; private set; }
        public Type MessageModelType { get; set; }
        public abstract string TitleLabel { get; }
        public abstract string DescriptionLabel { get; }
        public abstract string DefaultSubjectLabel { get; }

        public Dictionary<string, MessageVariableAttribute> ListMessageVariablesFromModel()
        {
            return MessageModelType.GetProperties()
                .Select(propertyInfo => new
                {
                    PropertyName = propertyInfo.Name,
                    Attrs = propertyInfo.GetCustomAttributes<MessageVariableAttribute>(true)
                })
                .Where(i => i.Attrs.Any())
                .ToDictionary(i => i.PropertyName, i => i.Attrs.First());
        }

        public Dictionary<string, MessageConditionalVariableAttribute> ListConditionalVariablesFromModel()
        {
            return MessageModelType.GetProperties()
                .Select(propertyInfo => new
                {
                    PropertyName = propertyInfo.Name,
                    Attributes = propertyInfo.GetCustomAttributes<MessageConditionalVariableAttribute>(true)
                })
                .Where(i => i.Attributes.Any())
                .ToDictionary(i => i.PropertyName, i => i.Attributes.First());
        }

        public string GetTemplateId(string lang)
        {
            return TemplateKey + "_" + lang;
        }

        public IList<VariableSpecification> ListVariableSpecifications()
        {
            return ListMessageVariablesFromModel()
                .Where(i => !i.Value.HideFromUI)
                .Select(i =>
                    new VariableSpecification(
                        i.Value.TemplateVariable,
                        i.Value.DescriptionLabel))
                .ToList();
        }

        public IList<VariableSpecification> ListConditionalVariableSpecifications()
        {
            return ListConditionalVariablesFromModel()
                .Select(i =>
                    new VariableSpecification(
                        i.Value.TemplateVariable,
                        i.Value.DescriptionLabel))
                .ToList();
        }

        private string GetTemplateKeyByClassName()
        {
            var type = GetType().UnderlyingSystemType;
            var className = type.Name;

            if (className.EndsWith("Specification"))
            {
                return className.Substring(0, className.LastIndexOf("Specification"));
            }

            throw new ArgumentException("Class name must end with 'Specification'");
        }
    }

    public abstract class MessageSpecification<TModel> : MessageSpecification
        where TModel : MessageModel
    {
        protected MessageSpecification(string module, string templateKey = null)
            : base(module, typeof(TModel), templateKey)
        {
        }
    }
}