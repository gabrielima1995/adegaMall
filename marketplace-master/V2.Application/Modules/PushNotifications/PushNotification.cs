using Application.Config;

namespace V2.Application.Modules.PushNotifications
{
    public class PushNotification
    {
        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                //Truncate long message titles
                if (value.Length > 64)
                    value = value.Substring(0, 64) + "...";

                _Title = value;
            }
        }

        private string _Body;

        public string Body
        {
            get { return _Body; }
            set
            {
                value = value.Trim();
                value = value.Replace('\n', ' ');

                //Truncate long message bodies
                if (value.Length > 100)
                    value = value.Substring(0, 100) + "...";

                _Body = value;
            }
        }

        private string _Link;

        public string Link
        {
            get { return _Link; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _Link = null;
                //Enforce valid In-App URLs or external HTTPS URLs
                else if (value.StartsWith(MaaSConfig.Current.Environment.AppUrl) || value.StartsWith("https"))
                    _Link = value;
                else
                {
                    value = value.TrimStart('/');
                    _Link = MaaSConfig.Current.Environment.AppUrl + value;
                }
            }
        }
    }
}