using System;
using System.Text.RegularExpressions;

namespace V2.Application.Messages
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MessageConditionalVariableAttribute : Attribute
    {
        private const string variableFormat = @"\[IF\:[a-z0-9_]+\]";
        private const string reverseVariableFormat = @"\[IFNOT\:[a-z0-9_]+\]";

        /// <param name="templateVariable">The name of the template variable, in the format [IF:VARIABLE_NAME].</param>
        /// <param name="descriptionLabel">The label to be used as a description for the admin user.</param>
        /// <param name="booleanExpression">The boolean expression to be used in the razor template, e.g. "Model.SomeProperty == 1".</param>
        public MessageConditionalVariableAttribute(string templateVariable, string descriptionLabel, string booleanExpression) : this(templateVariable, descriptionLabel, booleanExpression, false)
        {
        }

        private MessageConditionalVariableAttribute(string templateVariable, string descriptionLabel, string booleanExpression, bool reverse)
        {
            ThrowIfInvalidFormat(templateVariable, reverse);

            TemplateVariable = templateVariable;
            DescriptionLabel = descriptionLabel;
            BooleanExpression = booleanExpression;
            OpeningTag = templateVariable;
            ClosingTag = OpeningTag.Replace("[IF", "[/IF");
        }

        public string TemplateVariable { get; }
        public string DescriptionLabel { get; }
        public string BooleanExpression { get; }
        public string OpeningTag { get; }
        public string ClosingTag { get; }

        public MessageConditionalVariableAttribute GetReverse()
        {
            return new MessageConditionalVariableAttribute(
                TemplateVariable.Replace("[IF", "[IFNOT"), DescriptionLabel, $"!({BooleanExpression})", true);
        }

        private void ThrowIfInvalidFormat(string templateVariable, bool reverse)
        {
            var pattern = reverse ? reverseVariableFormat : variableFormat;
            if (!Regex.IsMatch(templateVariable, pattern, RegexOptions.IgnoreCase))
                throw new ArgumentException($"The template variable must be in the format [IF:VARIABLE_NAME] (e.g. [IF:IS_OFFLINE_PAYMENT]). The value was '{templateVariable}'.", nameof(templateVariable));
        }
    }
}