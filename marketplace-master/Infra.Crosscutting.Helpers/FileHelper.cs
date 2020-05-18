using System.IO;
using System.Linq;

namespace System
{
    public static class FileHelper
    {
        public static string GetFileExtension(this string fileName, string mediaType)
        {
            switch (mediaType)
            {
                case "image/jpeg":
                case "image/jpg":
                    return ".jpg";

                case "image/png":
                    return ".png";

                case "image/gif":
                    return ".gif";

                case "image/bmp":
                    return ".bmp";

                default:
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        var index = fileName.LastIndexOf(".", StringComparison.Ordinal);

                        if (index > 0)
                        {
                            return fileName.Substring(index);
                        }
                    }

                    return "";
            }
        }

        public static bool IsImageFile(this string fileName)
        {
            var extensionsAllowed = ".jpg|.jpeg|.gif|.png|.webp".Split('|');

            return IsExtensionAllowed(fileName, extensionsAllowed);
        }

        public static bool IsVideoFile(this string fileName)
        {
            var extensionsAllowed = ".mpg|.avi|.mp4|.webm".Split('|');

            return IsExtensionAllowed(fileName, extensionsAllowed);
        }

        public static bool IsExtensionAllowed(this string fileName, string[] extensionsAllowed)
        {
            var extensionFile = Path.GetExtension(fileName);

            return extensionsAllowed.Any(item => extensionFile == item);
        }
    }
}