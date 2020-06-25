using Framework;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using V2.Application.Messages;

namespace V2.Infrastructure.Messages
{
    public class DatabaseTemplateEngine : ITemplateEngine
    {
        private Assembly manifestAssembly;
        private string[] manifestResourceNames;

        private Assembly GetManifestsAssembly()
        {
            if (manifestAssembly == null)
                manifestAssembly = Assembly.GetAssembly(typeof(MessageSpecification));

            return manifestAssembly;
        }

        private string GetManifestResourceName(string templateResource, string lang)
        {
            if (manifestResourceNames == null)
                manifestResourceNames = GetManifestsAssembly().GetManifestResourceNames();

            return manifestResourceNames.FirstOrDefault(i => i.Contains("." + templateResource + "_" + lang));
        }

        private StringBuilder GetRazorTemplateResource(string templateResource, string lang)
        {
            var resourceQualifiedName = GetManifestResourceName(templateResource, lang);

            try
            {
                using (var stream = GetManifestsAssembly().GetManifestResourceStream(resourceQualifiedName))
                using (var reader = new StreamReader(stream))
                    return new StringBuilder(reader.ReadToEnd());
            }
            catch
            {
                throw new Exception($"'{resourceQualifiedName} resource not found'.");
            }
        }

        private OperationResult<StringBuilder> ReplaceVariables<TModel>(StringBuilder templateSource, MessageSpecification<TModel> spec, string lang)
            where TModel : MessageModel
        {
            var variables = spec.ListMessageVariablesFromModel();
            var conditionalVariables = spec.ListConditionalVariablesFromModel();

            templateSource = templateSource.Replace("@", "@@"); // escape character @

            foreach (var item in variables)
            {
                var propertyName = item.Key;
                var variable = item.Value;
                var razorTemplate = new StringBuilder(variable.RazorTemplate);

                if (!string.IsNullOrEmpty(variable.RazorTemplateResource))
                    razorTemplate = GetRazorTemplateResource(variable.RazorTemplateResource, lang);

                razorTemplate = razorTemplate.Replace("#TEMPLATE_VARIABLE#", propertyName);
                templateSource = templateSource.Replace(variable.TemplateVariable, razorTemplate.ToString());
            }

            if (conditionalVariables.Any())
            {
                var templateString = templateSource.ToString();

                foreach (var item in conditionalVariables)
                {
                    var propertyName = item.Key;
                    var variable = item.Value;
                    var reverseVariable = variable.GetReverse();

                    if (templateString.Contains(variable.OpeningTag) && templateString.Contains(variable.ClosingTag))
                    {
                        templateString = RemoveTagsAroundConditionalVariable(templateString, variable.OpeningTag);
                        templateString = RemoveTagsAroundConditionalVariable(templateString, variable.ClosingTag);

                        templateString = templateString.Replace(variable.OpeningTag, $"@if ({variable.BooleanExpression}) {{");
                        templateString = templateString.Replace(variable.ClosingTag, "}");
                    }
                    if (templateString.Contains(reverseVariable.OpeningTag) && templateString.Contains(reverseVariable.ClosingTag))
                    {
                        templateString = RemoveTagsAroundConditionalVariable(templateString, reverseVariable.OpeningTag);
                        templateString = RemoveTagsAroundConditionalVariable(templateString, reverseVariable.ClosingTag);

                        templateString = templateString.Replace(reverseVariable.OpeningTag, $"@if ({reverseVariable.BooleanExpression}) {{");
                        templateString = templateString.Replace(reverseVariable.ClosingTag, "}");
                    }
                }

                templateSource = new StringBuilder(templateString);
            }

            return templateSource;
        }

        private string RemoveTagsAroundConditionalVariable(string template, string conditionalVariable)
        {
            var leading = template.IndexOf($"<p>{conditionalVariable}");
            if (leading >= 0) template = template.Replace($"<p>{conditionalVariable}", conditionalVariable);

            var trailing = template.IndexOf($"{conditionalVariable}</p>");
            if (trailing >= 0) template = template.Replace($"{conditionalVariable}</p>", conditionalVariable);

            return template;
        }

        private OperationResult<StringBuilder> IncludeMasterTemplate<TModel>(StringBuilder templateSource, MessageSpecification<TModel> spec, string lang)
            where TModel : MessageModel
        {
            var masterTemplate = GetRazorTemplateResource("MasterTemplate", lang);
            masterTemplate = ReplaceVariables(masterTemplate, spec, lang).Result;
            return masterTemplate.Replace("#BODY_TEMPLATE#", templateSource.ToString());
        }

        public OperationResult<string> BuildTitle<TModel>(MessageSpecification<TModel> spec, TModel model, string lang)
            where TModel : MessageModel
        {
            var templateId = spec.GetTemplateId(lang);
            var hash = "";

            return MessagesContext.Current.GetTemplate(spec, lang)
                .Then(template =>
                {
                    hash = template.GetSubjectHash();
                    return ReplaceVariables(new StringBuilder(template.SubjectTemplate), spec, lang);
                })
                .Then(templateSource =>
                {
                    try
                    {
                        return Engine.Razor.RunCompile(templateSource.ToString(), templateId + hash, typeof(TModel), model, null);
                    }
                    catch (Exception ex)
                    {
                        return new OperationResult<string>("", ex.Message);
                    }
                });
        }

        public OperationResult<string> BuildBody<TModel>(MessageSpecification<TModel> spec, TModel model, string lang)
            where TModel : MessageModel
        {
            var templateId = spec.GetTemplateId(lang);
            var hash = "";

            return MessagesContext.Current.GetTemplate(spec, lang)
                .Then(template =>
                {
                    hash = template.GetBodyHash();
                    return ReplaceVariables(new StringBuilder(template.BodyTemplate), spec, lang);
                })
                .Then(templateSource => IncludeMasterTemplate(templateSource, spec, lang))
                .Then(templateSource =>
                {
                    try
                    {
                        return Engine.Razor.RunCompile(templateSource.ToString(), templateId + hash, typeof(TModel), model, null);
                    }
                    catch (Exception ex)
                    {
                        return new OperationResult<string>("", ex.Message);
                    }
                });
        }
    }
}