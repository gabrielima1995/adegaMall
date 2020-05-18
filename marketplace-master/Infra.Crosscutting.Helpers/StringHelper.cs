using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringHelper
    {
        public static bool Contains(this string input, string value, StringComparison comparisonType)
        {
            return !string.IsNullOrWhiteSpace(input) && input.IndexOf(value, comparisonType) > -1;
        }

        public static string ReplaceWithRegex(this string input, string pattern, string replacement)
        {
            var regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        public static bool ContainsWithRegex(this string input, string pattern)
        {
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        public static string Truncate(this string input, int size, string optionalSufixIfTruncated = null)
        {
            if (input == null || input.Length <= size)
                return input;

            input = input.Substring(0, size).Trim();

            if (optionalSufixIfTruncated != null)
                input += optionalSufixIfTruncated;

            return input;
        }

        public static string HtmlDecode(this string htmlEncoded) => WebUtility.HtmlDecode(htmlEncoded);

        public static string HtmlEncode(this string value) => WebUtility.HtmlEncode(value);

        public static string UrlDecode(this string value) => WebUtility.UrlDecode(value);

        public static string UrlEncode(this string value) => WebUtility.UrlEncode(value);

        /// <summary>
        /// Extrai números presentes em uma string
        /// </summary>
        /// <param name="value">Valor a ser verificado</param>
        /// <returns></returns>
        public static string ExtractDigits(this string value)
        {
            return Regex.Replace(value, "[^0-9]", string.Empty);
        }

        /// <summary>
        /// Substitui caracteres com acento pelo seu caractere base.
        /// </summary>
        /// <param name="value">String com caracteres com acento.</param>
        /// <returns>String com caracteres sem acento.</returns>
        public static string ReplaceAccents(this string value)
        {
            string normalized = value.Normalize(NormalizationForm.FormKD);

            Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                new EncoderReplacementFallback(""),
                new DecoderReplacementFallback(""));

            byte[] bytes = removal.GetBytes(normalized);

            return Encoding.ASCII.GetString(bytes);
        }

        public static void Sanitize(object obj)
        {
            if (obj == null) return;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in properties)
            {
                if (p.PropertyType != typeof(string)) { continue; }
                if (!p.CanWrite || !p.CanRead) { continue; }
                if (p.GetGetMethod(false) == null) { continue; }
                if (p.GetSetMethod(false) == null) { continue; }

                var val = (string)p.GetValue(obj, null);

                p.SetValue(obj,
                    string.IsNullOrWhiteSpace(val) ? null : val.Trim(),
                    null);
            }
        }

        public static string Hyphenize(string input)
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
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
    }
}