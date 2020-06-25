using Application.Config;
using Domain.DTO;
using Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using V2.Application.Messages;

namespace Presentation.Areas.Admin.Models
{
    public class MessageTemplateEditingViewModel
    {
        private MessageSpecification specification;

        public string Key { get; set; }

        [Display(Name = "Idioma")]
        public string Lang { get; set; }

        [Display(Name = "Assunto")]
        public string SubjectTemplate { get; set; }

        [Display(Name = "Mensagem")]
        public string BodyTemplate { get; set; }

        [Display(Name = "Ativar o envio deste email (deixe marcado para ativar)")]
        public bool Enabled { get; set; }

        [Display(Name = "Variáveis")]
        public Dictionary<string, string> BodyVariables { get; set; }

        [Display(Name = "Variáveis condicionais")]
        public Dictionary<string, string> BodyConditionalVariables { get; set; }

        public IList<string> EnabledLangs { get; set; }

        public string TitleLabel => specification.TitleLabel;

        public string DescriptionLabel => specification.DescriptionLabel;

        public MessageTemplateEditingViewModel()
        {
        }

        public MessageTemplateEditingViewModel(
            MessageTemplate messageTemplate,
            MessagesContext context,
            MaaSConfig config)
        {
            Key = messageTemplate.Key;
            Lang = messageTemplate.Lang;
            SubjectTemplate = messageTemplate.SubjectTemplate;
            BodyTemplate = messageTemplate.BodyTemplate;
            Enabled = messageTemplate.Enabled;

            SetExtras(context, config);
        }

        public MessageTemplateEditingViewModel(
            MessageTemplateEditing messageTemplate,
            MessagesContext context,
            MaaSConfig config)
        {
            Key = messageTemplate.Key;
            Lang = messageTemplate.Lang;
            SubjectTemplate = messageTemplate.SubjectTemplate;
            BodyTemplate = messageTemplate.BodyTemplate;
            Enabled = messageTemplate.Enabled;

            SetExtras(context, config);
        }

        private void SetExtras(MessagesContext context, MaaSConfig config)
        {
            specification = context.Specifications[Key];
            BodyVariables = specification.ListVariableSpecifications().ToDictionary(i => i.Name, i => i.DescriptionLabel);
            BodyConditionalVariables = specification.ListConditionalVariableSpecifications().ToDictionary(i => i.Name, i => i.DescriptionLabel);
            EnabledLangs = config.Localization.GetEnabledLanguages();
        }
    }
}