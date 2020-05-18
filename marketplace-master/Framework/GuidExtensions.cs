namespace System
{
    public static class GuidExtensions
    {
        public static string Substring(this Guid guid, int length = 8) =>
            guid.ToString().ToLower().Substring(0, length);
    }
}
