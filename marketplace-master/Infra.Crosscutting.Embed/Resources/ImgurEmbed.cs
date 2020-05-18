using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Embed.Resources
{
    public class ImgurEmbed : ResourceEmbed
    {
        private const string Template = "<iframe width=\"660\" height=\"370\" src=\"{0}#embed\" frameborder=\"0\" scrolling=\"no\" allowfullscreen=\"\"></iframe>";
        private const string REGEX_IS_IMGUR = @"https?:\/\/(.*imgur\.com\/.*)";

        public ImgurEmbed()
            : base(Template)
        {
        }

        private static string GetImgur(string url)
        {
            var api = "http://api.imgur.com/oembed.json?url={0}";
            string result;

            var request = (HttpWebRequest)WebRequest.Create(string.Format(api, url));

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }

        public override bool IsResource(string url)
        {
            return Regex.IsMatch(url, REGEX_IS_IMGUR);
        }

        public override string CreateEmbed(string url, bool autoPlay = false)
        {
            if (url.Contains(".gifv"))
            {
                return ProcessTemplate(url);
            }

            try
            {
                var content = GetImgur(url);
                dynamic json = JsonConvert.DeserializeObject<object>(content);

                if (json.type.ToString() == "rich")
                {
                    return json.html.ToString();
                }

                if (json.type.ToString() == "photo")
                {
                    return new ImageEmbed().CreateEmbed(json.url.ToString());
                }
            }
            catch (Exception) { }

            return new UnknownEmbed().CreateEmbed(url);
        }

        public override string GetThumbnail(string url)
        {
            string[] videoFormats = { ".gifv", ".webm", ".mp4" };

            foreach (var format in videoFormats)
            {
                if (url.Contains(format))
                {
                    return url.Replace(format, "h.jpg");
                }
            }

            return EmbedHelper.Config.DefaultImageThumbnail;
        }
    }
}