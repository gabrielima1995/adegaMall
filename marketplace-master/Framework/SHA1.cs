using System.Text;

namespace Framework
{
    public static class SHA1
    {
        public static string GenerateHash(string value)
        {
            using (var hasher = System.Security.Cryptography.SHA1.Create())
            {
                var encoding = new UTF8Encoding();

                var array = hasher.ComputeHash(encoding.GetBytes(value));

                var result = new StringBuilder();

                foreach (var item in array)
                {
                    // Convertendo para Hexadecimal
                    result.Append(item.ToString("x2"));
                }

                return result.ToString();
            }
        }
    }
}