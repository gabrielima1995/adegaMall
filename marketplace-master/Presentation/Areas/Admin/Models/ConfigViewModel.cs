using Framework;

namespace Presentation.Areas.Admin.Models
{
    public class ConfigViewModel
    {
        public string MaaSConfig { get; set; }

        public ErrorDictionary Errors { get; set; }

        public ErrorDictionary Warnings { get; set; }

        public ConfigViewModel()
        {
            Errors = new ErrorDictionary();
            Warnings = new ErrorDictionary();
        }

        public ConfigViewModel(string maasConfig, ErrorDictionary errors = null, ErrorDictionary warnings = null)
        {
            MaaSConfig = maasConfig;
            Errors = errors ?? new ErrorDictionary();
            Warnings = warnings ?? new ErrorDictionary();
        }
    }
}