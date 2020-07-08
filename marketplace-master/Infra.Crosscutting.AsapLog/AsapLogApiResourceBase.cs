using Framework.ApiProviders;
using Framework.AsapLog;
using Framework.Json;
using Infra.Crosscutting.AsapLog.Helpers;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Crosscutting.AsapLog
{
    public abstract class AsapLogApiResourceBase : ApiResourceBase
    {
        private readonly IJsonProvider _jsonProvider;

        protected AsapLogApiResourceBase(
            HttpClient client,
            IJsonProvider jsonProvider)
            : base(client)
        {
            _jsonProvider = jsonProvider;
        }

        protected AuthenticationHeaderValue GetCredentials(AsapLogCredentials credentials)
        {
            var token = $"{credentials.Usuario}:{credentials.Senha}".ToBase64Ascii();
            return new AuthenticationHeaderValue("Basic", token);
        }

        protected override HttpContent CreateContent<TCommand>(TCommand command)
        {
            return new StringContent(_jsonProvider.ToJson(command), Encoding.UTF8, "application/json");
        }

        protected override async Task<TResult> ParseSuccessResponseAsync<TResult>(HttpResponseMessage response)
        {
            var stream = await response.Content.ReadAsStreamAsync()
                .ConfigureAwait(false);

            return _jsonProvider.FromStream<TResult>(stream);
        }

        protected override async Task<List<ApiErrorMessage>> ParseErrorResponseAsync(HttpResponseMessage response)
        {
            ApiErrorMessage message;

            if (response.StatusCode.GetHashCode() == 400 ||
                response.StatusCode.GetHashCode() == 422)
            {
                var content = await response.Content.ReadAsStreamAsync()
                    .ConfigureAwait(false);

                var error = _jsonProvider.FromStream<AsapLogErrorResponse>(content);

                message = new ApiErrorMessage(error.Mensagem);
            }
            else
            {
                message = new ApiErrorMessage($"({response.StatusCode}) {response.ReasonPhrase} {response.RequestMessage}");
            }

            return new List<ApiErrorMessage>(1) { message };
        }
    }
}