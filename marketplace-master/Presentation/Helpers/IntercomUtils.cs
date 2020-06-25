using Application.Config;
using Domain.DTO;
using Microsoft.AspNet.Identity;
using System.Security.Cryptography;
using System.Web;

namespace Presentation.Helpers
{
    public static class IntercomSettings
    {
        private const string _secret = "H1r2Yd-i6vs6ZPLwivtprFMoe30IGXcwBMueKGOC";

        private static LoginProfile LoginProfile => HttpContext.Current.User.Identity.GetLoginProfile();

        public static string AppId => "g8ultybe";

        public static string UserName => LoginProfile.Name.Replace("'", "\'");

        public static string UserEmail => LoginProfile.Email;

        public static string UserHash => CreateToken(LoginProfile.Email);

        public static string MarketplaceName => MaaSConfig.Current.ApplicationName.Replace("'", "\'");

        public static string MarketplaceUrl => MaaSConfig.Current.Environment.AppUrl;

        private static string CreateToken(string userEmail)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(_secret);
            byte[] messageBytes = encoding.GetBytes(userEmail);

            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                var sb = new System.Text.StringBuilder();

                for (var i = 0; i <= hashmessage.Length - 1; i++)
                    sb.Append(hashmessage[i].ToString("X2"));

                return sb.ToString();
            }
        }
    }
}