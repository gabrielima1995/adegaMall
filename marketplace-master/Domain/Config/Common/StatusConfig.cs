using Domain.Config.Common;
using Framework;

namespace Application.Config.Common
{
    public class StatusConfig : IConfigNode
    {
        public bool Enabled { get; set; }

        public StatusConfig(bool enabled = true)
        {
            Enabled = enabled;
        }

        public static implicit operator StatusConfig(bool enabled)
        {
            return new StatusConfig(true);
        }

        public ErrorDictionary CheckErrors()
        {
            return new ErrorDictionary();
        }

        public ErrorDictionary CheckWarnings()
        {
            return new ErrorDictionary();
        }

        public void Sanitize()
        {
            ConfigNodeHelper.SanitizeSelfAndChildren(this);
        }
    }
}