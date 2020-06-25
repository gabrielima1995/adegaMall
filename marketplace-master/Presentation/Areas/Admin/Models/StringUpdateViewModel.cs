using Infra.Crosscutting.Globalization;

namespace Presentation.Areas.Admin.Models
{
    public class StringUpdateViewModel
    {
        public string Label { get; set; }

        public Lang Lang { get; set; }

        public string Value { get; set; }
    }
}