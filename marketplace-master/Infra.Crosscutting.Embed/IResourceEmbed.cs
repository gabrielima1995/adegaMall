namespace Infra.Crosscutting.Embed
{
    public interface IResourceEmbed
    {
        bool IsResource(string url);

        string CreateEmbed(string url, bool autoPlay = false);

        string GetThumbnail(string url);
    }
}