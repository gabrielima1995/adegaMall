using System;

namespace Framework
{
    public static class PasswordGenerator
    {
        public static string NewUserPassword() => GenerateToken(32);
        public static string NewSecurityStamp() => GenerateToken(128);

        private static string GenerateToken(int len)
        {
            var token = string.Empty;
            var random = new Random();

            for (int a = 0; a < len; a++)
                token += chars[random.Next(0, chars.Length-1)];

            return token;
        }

        private static readonly char[] chars = new char[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
    }
}
