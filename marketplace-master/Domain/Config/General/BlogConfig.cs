using Domain.Config.Common;
using Framework;

namespace Application.Config.General
{
    public class BlogConfig : IConfigNode
    {
        public string Url { get; set; }

        public ErrorDictionary CheckErrors()
        {
            var errors = new ErrorDictionary();
            errors.Merge(this.CheckChildErrors());
            return errors;
        }

        public ErrorDictionary CheckWarnings()
        {
            var warnings = new ErrorDictionary();
            warnings.Merge(this.CheckChildWarnings());
            return warnings;
        }

        public void Sanitize()
        {
            ConfigNodeHelper.SanitizeSelfAndChildren(this);
        }
    }
}
