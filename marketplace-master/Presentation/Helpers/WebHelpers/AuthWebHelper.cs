using System.Web;

namespace Infra.Crosscutting.WebHelpers
{
    public static class AuthWebHelper
    {
        private static HttpRequest GetRequest()
        {
            return HttpContext.Current.Request;
        }

        public static string GetAuthToken()
        {
            //obtem o token pelo header
            var authorization = GetRequest().Headers["Authorization"];

            if (authorization != null)
            {
                var values = authorization.Split(' ');

                if (values[0].ToLower() == "bearer")
                {
                    return values[1];
                }
            }

            return null;
        }
    }
}
