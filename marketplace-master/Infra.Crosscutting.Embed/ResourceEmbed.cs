namespace Infra.Crosscutting.Embed
{
    public abstract class ResourceEmbed : IResourceEmbed
    {
        private readonly string _template;

        protected ResourceEmbed(string template)
        {
            _template = template;
        }

        public abstract bool IsResource(string url);

        public abstract string CreateEmbed(string url, bool autoPlay = false);

        public abstract string GetThumbnail(string url);

        protected string ProcessTemplate(params object[] values)
        {
            return string.Format(_template, values);
        }
    }
}