using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Framework.ApiProviders
{
    public class ApiResponse<TResult>
        where TResult : class
    {
        public TResult Result { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }

        public List<ApiErrorMessage> Errors { get; private set; }
        public bool IsFailure => Errors != null && Errors.Any();
        public bool IsSuccess => !IsFailure;

        public ApiResponse(TResult result, HttpStatusCode statusCode)
        {
            Result = result;
            StatusCode = statusCode;
        }

        public ApiResponse(ApiErrorMessage error, HttpStatusCode statusCode)
            : this(new List<ApiErrorMessage>(1) { error }, statusCode)
        {
        }

        public ApiResponse(List<ApiErrorMessage> errors, HttpStatusCode statusCode)
        {
            Errors = errors;
            StatusCode = statusCode;
        }

        public static async Task<ApiResponse<TResult>> SuccessAsync(HttpResponseMessage response,
            Func<HttpResponseMessage, Task<TResult>> successResponseProcessor)
        {
            try
            {
                if (typeof(TResult) == typeof(EmptyResult))
                    return new ApiResponse<TResult>(new EmptyResult() as TResult, response.StatusCode);

                var result = await successResponseProcessor.Invoke(response)
                    .ConfigureAwait(false);

                return new ApiResponse<TResult>(result, response.StatusCode);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TResult>(new ApiErrorMessage($"({response.StatusCode}) {response.RequestMessage} {ex.Message}"), response.StatusCode);
            }
        }

        public static async Task<ApiResponse<TResult>> FailureAsync(HttpResponseMessage response,
            Func<HttpResponseMessage, Task<List<ApiErrorMessage>>> errorResponseProcessor)
        {
            try
            {
                var errors = await errorResponseProcessor.Invoke(response)
                    .ConfigureAwait(false);

                return new ApiResponse<TResult>(errors, response.StatusCode);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TResult>(new ApiErrorMessage($"({response.StatusCode}) {response.RequestMessage} {ex.Message}"), response.StatusCode);
            }
        }

        public static ApiResponse<TResult> Failure(HttpRequestMessage request, Exception ex)
        {
            return new ApiResponse<TResult>(new ApiErrorMessage($"(0) {request} {ex.Message}"), 0);
        }
    }
}