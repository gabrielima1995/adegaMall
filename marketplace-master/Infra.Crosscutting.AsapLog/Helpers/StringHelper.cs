using System;
using System.Text;

namespace Infra.Crosscutting.AsapLog.Helpers
{
    public static class StringHelper
    {
        public static string ToBase64Ascii(this string content) =>
            Convert.ToBase64String(Encoding.ASCII.GetBytes(content));
    }
}