using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Configuration;
using System.Web;

namespace Application.Auth
{
    public class ApiTokenAuthorizedProvider
    {
        private readonly IApiTokenRepository _apiTokenRepository;
        private ApiToken _auth;

        public ApiToken Auth => GetAuthorization().Result;

        public ApiTokenAuthorizedProvider(
            IApiTokenRepository apiTokenRepository)
        {
            _apiTokenRepository = apiTokenRepository;
        }

        public OperationResult<ApiToken> GetAuthorization()
        {
            var errors = new ErrorDictionary();

            if (_auth == null)
            {
                try
                {
                    #region Obtem token

                    // obtem o token de autorização da request
                    var token = GetAuthToken();

                    if (token != null)
                    {
                        if (!IsMasterToken(token))
                        {
                            _auth = _apiTokenRepository.GetByToken(token);

                            if (_auth == null || _auth.IsRevoked)
                                errors.Add("auth", "Authorization denied");
                        }
                    }
                    else
                    {
                        errors.Add("auth", "Authorization not found");
                    }

                    #endregion Obtem token
                }
                catch (Exception)
                {
                    errors.Add("auth", "An error occurred while obtaining authorization");
                }
            }

            return new OperationResult<ApiToken>(_auth, errors);
        }

        private bool IsMasterToken(string token) => token == ConfigurationManager.AppSettings["INAMASTERAPITOKEN"].ToString();

        public AuthorizationStatus GetAuthStatus()
        {
            #region Validação da autorização

            // obtem a autorização
            var authResult = GetAuthorization();

            // se a autorização não for inválida
            if (!authResult.OK)
            {
                // Unauthorized
                return AuthorizationStatus.Unauthorized;
            }

            #endregion Validação da autorização

            return AuthorizationStatus.OK;
        }

        private static HttpRequest GetRequest()
        {
            return HttpContext.Current.Request;
        }

        private static string GetAuthToken()
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