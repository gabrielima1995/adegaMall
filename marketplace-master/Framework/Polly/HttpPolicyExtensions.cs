using Polly;
using System;
using System.Net;
using System.Net.Http;

namespace Framework.Polly
{
    public static class HttpPolicyExtensions
    {
        private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate = (response) =>
            (int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout;

        public static PolicyBuilder<HttpResponseMessage> HandleTransientHttpError()
        {
            return Policy<HttpResponseMessage>.Handle<HttpRequestException>().OrTransientHttpStatusCode();
        }

        private static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
        }

        public static PolicyBuilder<HttpResponseMessage> OrTransientHttpError(this PolicyBuilder policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
        }

        public static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder<HttpResponseMessage> policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
        }

        public static PolicyBuilder<HttpResponseMessage> OrTransientHttpError(this PolicyBuilder<HttpResponseMessage> policyBuilder)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
        }
    }
}