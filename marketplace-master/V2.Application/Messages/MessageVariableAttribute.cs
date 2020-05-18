using System;

namespace V2.Application.Messages
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MessageVariableAttribute : Attribute
    {
        public string TemplateVariable { get; set; }

        public string DescriptionLabel { get; set; }

        public string RazorTemplate { get; set; } = "@Raw(Model.#TEMPLATE_VARIABLE#)";

        public string RazorTemplateResource { get; set; }

        public bool HideFromUI { get; set; }
    }
}