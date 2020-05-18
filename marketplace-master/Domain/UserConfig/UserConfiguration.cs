using Application.Config;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.UserConfig
{
    public class UserConfigurationRaw
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR(MAX)")]
        public string RawConfig { get; set; }
    }

    public class UserConfiguration
    {
        public static UserConfiguration Current { get; set; }

        public ChatConfiguration ChatConfig { get; set; } = new ChatConfiguration();

        public BlogConfiguration BlogConfig { get; set; } = new BlogConfiguration();
    }

    public class ChatConfiguration
    {
        public string Script { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// Adds html script tags if necessary
        /// </summary>
        /// <returns></returns>
        public string GetScript()
        {
            if (Script.Contains("<script") || Script.Contains("script>"))
                return Script;

            else return "<script>" + Script + "</script>";
        }
    }

    public class BlogConfiguration
    {
        public string Address { get; set; }

        public bool Enabled { get; set; }

        public string GetBlogAddress()
        {
            var address = Address ?? MaaSConfig.Current.Blog.Url;

            if (address == null || address.StartsWith(@"http://") || address.StartsWith(@"https://"))
                return address;

            return "http://" + address;
        }
    }
}
