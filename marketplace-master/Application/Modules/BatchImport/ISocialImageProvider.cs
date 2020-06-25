namespace Application.Modules.BatchImport
{
    public interface ISocialImageProvider
    {
        bool CanHandleUrl(string profileUrl);

        byte[] GetImage(string profileUrl);
    }
}
