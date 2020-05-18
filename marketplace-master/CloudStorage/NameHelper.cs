using System;
using System.IO;
using System.Text;

namespace CloudStorage
{
    internal class NameHelper
    {
        public static string NormalizeContainerName(string input)
        {
            return NormalizePart(input);
        }

        public static string NormalizeBlobName(string input)
        {
            foreach (var ch in Path.GetInvalidFileNameChars())
                input = input.Replace(ch.ToString(), "");

            var modifier = Guid.NewGuid().ToString().Substring(0, 8);
            var fileName = NormalizePart(Path.GetFileNameWithoutExtension(input));
            var extension = Path.GetExtension(input);

            return $"{modifier}-{fileName}{extension}";
        }

        private static string NormalizePart(string input)
        {
            input = ReplaceDiacritics(input);
            input = Hyphenize(input);

            return input;
        }

        private static string ReplaceDiacritics(string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormKD);
            var removal = Encoding.GetEncoding(
                Encoding.ASCII.CodePage,
                new EncoderReplacementFallback(""),
                new DecoderReplacementFallback(""));

            var bytes = removal.GetBytes(normalized);
            return Encoding.ASCII.GetString(bytes);
        }

        private static string Hyphenize(string input)
        {
            if (input == null) return "";

            const int maxlen = 80;
            int len = input.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = input[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(c);
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }
    }
}
